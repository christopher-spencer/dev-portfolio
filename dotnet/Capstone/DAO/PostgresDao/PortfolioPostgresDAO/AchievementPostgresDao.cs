using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class AchievementPostgresDao : IAchievementDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;

        public AchievementPostgresDao(string dbConnectionString, IImageDao imageDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
        }

        /*  
            **********************************************************************************************
                                                ACHIEVEMENT CRUD
            **********************************************************************************************
        */

        public Achievement GetAchievement(int achievementId)
        {
            if (achievementId <= 0)
            {
                throw new ArgumentException("Achievement ID must be greater than zero.");
            }

            string sql = "SELECT id, description, icon_id FROM achievements WHERE id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToAchievement(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the achievement.", ex);
            }

            return null;
        }

        public List<Achievement> GetAchievements()
        {
            List<Achievement> achievements = new List<Achievement>();

            string sql = "SELECT id, description, icon_id FROM achievements;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Achievement achievement = MapRowToAchievement(reader);

                                achievements.Add(achievement);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the achievements.", ex);
            }

            return achievements;
        }

        /*  
            **********************************************************************************************
                                          WORK EXPERIENCE ACHIEVEMENT CRUD
            **********************************************************************************************
        */

        public Achievement CreateAchievementByWorkExperienceId(int experienceId, Achievement achievement)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("Experience ID must be greater than zero.");
            }

            CheckAchievementDescriptionIsNotNullOrEmpty(achievement);

            string insertAchievementSql = "INSERT INTO achievements (description) VALUES (@description) RETURNING id;";
            string insertExperienceAchievementSql = "INSERT INTO work_experience_achievements (experience_id, achievement_id) VALUES (@experienceId, @achievementId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int achievementId;

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertAchievementSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@description", achievement.Description);
                                cmd.Transaction = transaction;
                                achievementId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertExperienceAchievementSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@experienceId", experienceId);
                                cmd.Parameters.AddWithValue("@achievementId", achievementId);
                                cmd.Transaction = transaction;
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            achievement.Id = achievementId;

                            return achievement;

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the achievement by work experience ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Achievement> GetAchievementsByWorkExperienceId(int experienceId)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("Experience ID must be greater than zero.");
            }

            List<Achievement> achievements = new List<Achievement>();

            string sql = "SELECT a.id, a.description, a.icon_id " +
                         "FROM achievements a " +
                         "JOIN work_experience_achievements ea ON a.id = ea.achievement_id " +
                         "WHERE ea.experience_id = @experienceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Achievement achievement = MapRowToAchievement(reader);

                                achievements.Add(achievement);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the achievements by work experience ID.", ex);
            }

            return achievements;
        }

        public Achievement GetAchievementByWorkExperienceId(int experienceId, int achievementId)
        {
            if (experienceId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("Experience ID and Achievement ID must be greater than zero.");
            }

            string sql = "SELECT a.id, a.description, a.icon_id " +
                         "FROM achievements a " +
                         "JOIN work_experience_achievements ea ON a.id = ea.achievement_id " +
                         "WHERE ea.experience_id = @experienceId AND a.id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToAchievement(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the achievement by work experience ID and achievement ID.", ex);
            }

            return null;
        }

        public Achievement UpdateAchievementByWorkExperienceId(int experienceId, int achievementId, Achievement achievement)
        {
            if (experienceId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("Experience ID and Achievement ID must be greater than zero.");
            }

            CheckAchievementDescriptionIsNotNullOrEmpty(achievement);

            string sql = "UPDATE achievements " +
                         "SET description = @description " +
                         "FROM work_experience_achievements " +
                         "WHERE achievements.id = work_experience_achievements.achievement_id " +
                         "AND work_experience_achievements.experience_id = @experienceId " +
                         "AND achievements.id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);
                        cmd.Parameters.AddWithValue("@description", achievement.Description);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return achievement;
                        }
                    }

                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the achievement by work experience ID.", ex);
            }

            return null;
        }

        public int DeleteAchievementByWorkExperienceId(int experienceId, int achievementId)
        {
            if (experienceId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("Experience ID and Achievement ID must be greater than zero.");
            }

            string deleteAchievementFromExperienceSql = "DELETE FROM work_experience_achievements WHERE experience_id = @experienceId AND achievement_id = @achievementId;";
            string deleteAchievementSql = "DELETE FROM achievements WHERE id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int rowsAffected;

                            int? iconId = GetIconIdByAchievementId(achievementId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteAchievementFromExperienceSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@experienceId", experienceId);
                                cmd.Parameters.AddWithValue("@achievementId", achievementId);
                                cmd.Transaction = transaction;
                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageByAchievementId(achievementId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteAchievementSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@achievementId", achievementId);
                                cmd.Transaction = transaction;

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the achievement by work experience ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            
            }

        }

        /*  
            **********************************************************************************************
                                          EDUCATION ACHIEVEMENT CRUD
            **********************************************************************************************
        */

        public Achievement CreateAchievementByEducationId(int educationId, Achievement achievement)
        {
            if (educationId <= 0)
            {
                throw new ArgumentException("Education ID must be greater than zero.");
            }

            CheckAchievementDescriptionIsNotNullOrEmpty(achievement);

            string insertAchievementSql = "INSERT INTO achievements (description) VALUES (@description) RETURNING id;";
            string insertEducationAchievementSql = "INSERT INTO education_achievements (education_id, achievement_id) VALUES (@educationId, @achievementId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int achievementId;

                            using (NpgsqlCommand cmdInsertAchievement = new NpgsqlCommand(insertAchievementSql, connection))
                            {
                                cmdInsertAchievement.Parameters.AddWithValue("@description", achievement.Description);
                                cmdInsertAchievement.Transaction = transaction;
                                achievementId = Convert.ToInt32(cmdInsertAchievement.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertEducationAchievement = new NpgsqlCommand(insertEducationAchievementSql, connection))
                            {
                                cmdInsertEducationAchievement.Parameters.AddWithValue("@educationId", educationId);
                                cmdInsertEducationAchievement.Parameters.AddWithValue("@achievementId", achievementId);
                                cmdInsertEducationAchievement.Transaction = transaction;
                                cmdInsertEducationAchievement.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            achievement.Id = achievementId;

                            return achievement;

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the achievement by education ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Achievement> GetAchievementsByEducationId(int educationId)
        {
            if (educationId <= 0)
            {
                throw new ArgumentException("Education ID must be greater than zero.");
            }

            List<Achievement> achievements = new List<Achievement>();

            string sql = "SELECT a.id, a.description, a.icon_id " +
                         "FROM achievements a " +
                         "JOIN education_achievements ea ON a.id = ea.achievement_id " +
                         "WHERE ea.education_id = @educationId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Achievement achievement = MapRowToAchievement(reader);

                                achievements.Add(achievement);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the achievements by education ID.", ex);
            }

            return achievements;
        }

        public Achievement GetAchievementByEducationId(int educationId, int achievementId)
        {
            if (educationId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("Education ID and Achievement ID must be greater than zero.");
            }

            string sql = "SELECT a.id, a.description, a.icon_id " +
                         "FROM achievements a " +
                         "JOIN education_achievements ea ON a.id = ea.achievement_id " +
                         "WHERE ea.education_id = @educationId AND a.id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToAchievement(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the achievement by education ID and achievement ID.", ex);
            }

            return null;
        }

        public Achievement UpdateAchievementByEducationId(int educationId, int achievementId, Achievement achievement)
        {
            if (educationId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("Education ID and Achievement ID must be greater than zero.");
            }

            CheckAchievementDescriptionIsNotNullOrEmpty(achievement);

            string sql = "UPDATE achievements " +
                         "SET description = @description " +
                         "FROM education_achievements " +
                         "WHERE achievements.id = education_achievements.achievement_id " +
                         "AND education_achievements.education_id = @educationId " +
                         "AND achievements.id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);
                        cmd.Parameters.AddWithValue("@description", achievement.Description);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return achievement;
                        }
                    }

                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the achievement by education ID.", ex);
            }

            return null;
        }

        public int DeleteAchievementByEducationId(int educationId, int achievementId)
        {
            if (educationId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("Education ID and Achievement ID must be greater than zero.");
            }

            string deleteAchievementFromEducationSql = "DELETE FROM education_achievements WHERE education_id = @educationId AND achievement_id = @achievementId;";
            string deleteAchievementSql = "DELETE FROM achievements WHERE id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int rowsAffected;

                            int? iconId = GetIconIdByAchievementId(achievementId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteAchievementFromEducationSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@educationId", educationId);
                                cmd.Parameters.AddWithValue("@achievementId", achievementId);
                                cmd.Transaction = transaction;
                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageByAchievementId(achievementId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteAchievementSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@achievementId", achievementId);
                                cmd.Transaction = transaction;

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the achievement by education ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                    OPEN SOURCE CONTRIBUTION ACHIEVEMENT CRUD
            **********************************************************************************************
        */

        public Achievement CreateAchievementByOpenSourceContributionId(int contributionId, Achievement achievement)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("Contribution ID must be greater than zero.");
            }

            CheckAchievementDescriptionIsNotNullOrEmpty(achievement);

            string insertAchievementSql = "INSERT INTO achievements (description) VALUES (@description) RETURNING id;";
            string insertContributionAchievementSql = "INSERT INTO open_source_contribution_achievements (contribution_id, achievement_id) VALUES (@contributionId, @achievementId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int achievementId;

                            using (NpgsqlCommand cmdInsertAchievement = new NpgsqlCommand(insertAchievementSql, connection))
                            {
                                cmdInsertAchievement.Parameters.AddWithValue("@description", achievement.Description);
                                cmdInsertAchievement.Transaction = transaction;
                                achievementId = Convert.ToInt32(cmdInsertAchievement.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertContributionAchievement = new NpgsqlCommand(insertContributionAchievementSql, connection))
                            {
                                cmdInsertContributionAchievement.Parameters.AddWithValue("@contributionId", contributionId);
                                cmdInsertContributionAchievement.Parameters.AddWithValue("@achievementId", achievementId);
                                cmdInsertContributionAchievement.Transaction = transaction;
                                cmdInsertContributionAchievement.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            achievement.Id = achievementId;

                            return achievement;

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the achievement by open source contribution ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Achievement> GetAchievementsByOpenSourceContributionId(int contributionId)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("Contribution ID must be greater than zero.");
            }

            List<Achievement> achievements = new List<Achievement>();

            string sql = "SELECT a.id, a.description, a.icon_id " +
                         "FROM achievements a " +
                         "JOIN open_source_contribution_achievements ca ON a.id = ca.achievement_id " +
                         "WHERE ca.contribution_id = @contributionId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Achievement achievement = MapRowToAchievement(reader);

                                achievements.Add(achievement);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the achievements by open source contribution ID.", ex);
            }

            return achievements;
        }

        public Achievement GetAchievementByOpenSourceContributionId(int contributionId, int achievementId)
        {
            if (contributionId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("Contribution ID and Achievement ID must be greater than zero.");
            }

            string sql = "SELECT a.id, a.description, a.icon_id " +
                         "FROM achievements a " +
                         "JOIN open_source_contribution_achievements ca ON a.id = ca.achievement_id " +
                         "WHERE ca.contribution_id = @contributionId AND a.id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToAchievement(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the achievement by open source contribution ID and achievement ID.", ex);
            }

            return null;
        }

        public Achievement UpdateAchievementByOpenSourceContributionId(int contributionId, int achievementId, Achievement achievement)
        {
            if (contributionId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("Contribution ID and Achievement ID must be greater than zero.");
            }

            CheckAchievementDescriptionIsNotNullOrEmpty(achievement);

            string sql = "UPDATE achievements " +
                         "SET description = @description " +
                         "FROM open_source_contribution_achievements " +
                         "WHERE achievements.id = open_source_contribution_achievements.achievement_id " +
                         "AND open_source_contribution_achievements.contribution_id = @contributionId " +
                         "AND achievements.id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);
                        cmd.Parameters.AddWithValue("@description", achievement.Description);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return achievement;
                        }
                    }

                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the achievement by open source contribution ID.", ex);
            }

            return null;
        }

        public int DeleteAchievementByOpenSourceContributionId(int contributionId, int achievementId)
        {
            if (contributionId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("Contribution ID and Achievement ID must be greater than zero.");
            }

            string deleteAchievementFromContributionSql = "DELETE FROM open_source_contribution_achievements WHERE contribution_id = @contributionId AND achievement_id = @achievementId;";
            string deleteAchievementSql = "DELETE FROM achievements WHERE id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int rowsAffected;

                            int? iconId = GetIconIdByAchievementId(achievementId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteAchievementFromContributionSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@contributionId", contributionId);
                                cmd.Parameters.AddWithValue("@achievementId", achievementId);
                                cmd.Transaction = transaction;
                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageByAchievementId(achievementId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteAchievementSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@achievementId", achievementId);
                                cmd.Transaction = transaction;

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the achievement by open source contribution ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                        VOLUNTEER WORK ACHIEVEMENT CRUD
            **********************************************************************************************
        */

        public Achievement CreateAchievementByVolunteerWorkId(int volunteerId, Achievement achievement)
        {
            if (volunteerId <= 0)
            {
                throw new ArgumentException("VolunteerWorkId must be greater than zero.");
            }

            CheckAchievementDescriptionIsNotNullOrEmpty(achievement);

            string insertAchievementSql = "INSERT INTO achievements (description) VALUES (@description) RETURNING id;";
            string insertVolunteerWorkAchievementSql = "INSERT INTO volunteer_work_achievements (volunteer_work_id, achievement_id) VALUES (@volunteerId, @achievementId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int achievementId;

                            using (NpgsqlCommand cmdInsertAchievement = new NpgsqlCommand(insertAchievementSql, connection))
                            {
                                cmdInsertAchievement.Parameters.AddWithValue("@description", achievement.Description);
                                cmdInsertAchievement.Transaction = transaction;
                                achievementId = Convert.ToInt32(cmdInsertAchievement.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertWorkAchievement = new NpgsqlCommand(insertVolunteerWorkAchievementSql, connection))
                            {
                                cmdInsertWorkAchievement.Parameters.AddWithValue("@volunteerId", volunteerId);
                                cmdInsertWorkAchievement.Parameters.AddWithValue("@achievementId", achievementId);
                                cmdInsertWorkAchievement.Transaction = transaction;
                                cmdInsertWorkAchievement.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            achievement.Id = achievementId;

                            return achievement;

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the achievement by volunteer work ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Achievement> GetAchievementsByVolunteerWorkId(int volunteerId)
        {
            if (volunteerId <= 0)
            {
                throw new ArgumentException("VolunteerWorkId must be greater than zero.");
            }

            List<Achievement> achievements = new List<Achievement>();

            string sql = "SELECT a.id, a.description, a.icon_id " +
                         "FROM achievements a " +
                         "JOIN volunteer_work_achievements va ON a.id = va.achievement_id " +
                         "WHERE va.volunteer_work_id = @volunteerId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerId", volunteerId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Achievement achievement = MapRowToAchievement(reader);

                                achievements.Add(achievement);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the achievements by volunteer work ID.", ex);
            }

            return achievements;
        }

        public Achievement GetAchievementByVolunteerWorkId(int volunteerId, int achievementId)
        {
            if (volunteerId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("VolunteerWorkId and AchievementId must be greater than zero.");
            }

            string sql = "SELECT a.id, a.description, a.icon_id " +
                         "FROM achievements a " +
                         "JOIN volunteer_work_achievements va ON a.id = va.achievement_id " +
                         "WHERE va.volunteer_work_id = @volunteerId AND a.id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerId", volunteerId);
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToAchievement(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the achievement by volunteer work ID and achievement ID.", ex);
            }

            return null;
        }

        public Achievement UpdateAchievementByVolunteerWorkId(int volunteerId, int achievementId, Achievement achievement)
        {
            if (volunteerId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("VolunteerWorkId and AchievementId must be greater than zero.");
            }

            CheckAchievementDescriptionIsNotNullOrEmpty(achievement);

            string sql = "UPDATE achievements " +
                         "SET description = @description " +
                         "FROM volunteer_work_achievements " +
                         "WHERE achievements.id = volunteer_work_achievements.achievement_id " +
                         "AND volunteer_work_achievements.volunteer_work_id = @volunteerId " +
                         "AND achievements.id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerId", volunteerId);
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);
                        cmd.Parameters.AddWithValue("@description", achievement.Description);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return achievement;
                        }
                    }

                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the achievement by volunteer work ID.", ex);
            }

            return null;
        }

        public int DeleteAchievementByVolunteerWorkId(int volunteerId, int achievementId)
        {
            if (volunteerId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("VolunteerWorkId and AchievementId must be greater than zero.");
            }

            string deleteAchievementFromVolunteerSql = "DELETE FROM volunteer_work_achievements WHERE volunteer_work_id = @volunteerId AND achievement_id = @achievementId;";
            string deleteAchievementSql = "DELETE FROM achievements WHERE id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int rowsAffected;

                            int? iconId = GetIconIdByAchievementId(achievementId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteAchievementFromVolunteerSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@volunteerId", volunteerId);
                                cmd.Parameters.AddWithValue("@achievementId", achievementId);
                                cmd.Transaction = transaction;
                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageByAchievementId(achievementId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteAchievementSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@achievementId", achievementId);
                                cmd.Transaction = transaction;

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the achievement by volunteer work ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                                HELPER METHODS
            **********************************************************************************************
        */

        private int? GetIconIdByAchievementId(int achievementId)
        {
            string sql = "SELECT icon_id FROM achievements WHERE id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving icon ID by achievement ID: " + ex.Message);
                return null;
            }
        }

        private void CheckAchievementDescriptionIsNotNullOrEmpty(Achievement achievement)
        {
            if (string.IsNullOrEmpty(achievement.Description))
            {
                throw new ArgumentException("Achievement description cannot be null or empty.");
            }
        }

        /*  
            **********************************************************************************************
                                               ACHIEVEMENT MAP ROW
            **********************************************************************************************
        */

        private Achievement MapRowToAchievement(NpgsqlDataReader reader)
        {
            Achievement achievement = new Achievement
            {
                Id = Convert.ToInt32(reader["id"]),
                Description = Convert.ToString(reader["description"])
            };

            int achievementId = achievement.Id;

            SetAchievementIconIdProperties(reader, achievement, achievementId);

            return achievement;
        }

        private void SetAchievementIconIdProperties(NpgsqlDataReader reader, Achievement achievement, int achievementId)
        {
            if (reader["icon_id"] != DBNull.Value)
            {
                achievement.IconId = Convert.ToInt32(reader["icon_id"]);

                achievement.Icon = _imageDao.GetImageByAchievementId(achievementId);
            }
            else
            {
                achievement.IconId = 0;
            }
        }


    }
}