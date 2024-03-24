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
                                            EXPERIENCE ACHIEVEMENT CRUD
            **********************************************************************************************
        */

        public Achievement CreateAchievementByExperienceId(int experienceId, Achievement achievement)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("Experience ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(achievement.Description))
            {
                throw new ArgumentException("Achievement description cannot be null or empty.");
            }

            string insertAchievementSql = "INSERT INTO achievements (description) VALUES (@description) RETURNING id;";
            string insertExperienceAchievementSql = "INSERT INTO experience_achievements (experience_id, achievement_id) VALUES (@experienceId, @achievementId);";

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

                            using (NpgsqlCommand cmdInsertExperienceAchievement = new NpgsqlCommand(insertExperienceAchievementSql, connection))
                            {
                                cmdInsertExperienceAchievement.Parameters.AddWithValue("@experienceId", experienceId);
                                cmdInsertExperienceAchievement.Parameters.AddWithValue("@achievementId", achievementId);
                                cmdInsertExperienceAchievement.Transaction = transaction;
                                cmdInsertExperienceAchievement.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            achievement.Id = achievementId;

                            return achievement;

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the achievement by experience ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Achievement> GetAchievementsByExperienceId(int experienceId)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("Experience ID must be greater than zero.");
            }

            List<Achievement> achievements = new List<Achievement>();

            string sql = "SELECT a.id, a.description, a.icon_id " +
                         "FROM achievements a " +
                         "JOIN experience_achievements ea ON a.id = ea.achievement_id " +
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
                throw new DaoException("An error occurred while retrieving the achievements by experience ID.", ex);
            }

            return achievements;
        }

        public Achievement GetAchievementByExperienceId(int experienceId, int achievementId)
        {
            if (experienceId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("Experience ID and Achievement ID must be greater than zero.");
            }

            string sql = "SELECT a.id, a.description, a.icon_id " +
                         "FROM achievements a " +
                         "JOIN experience_achievements ea ON a.id = ea.achievement_id " +
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
                throw new DaoException("An error occurred while retrieving the achievement by experience ID and achievement ID.", ex);
            }

            return null;
        }

        public Achievement UpdateAchievementByExperienceId(int experienceId, int achievementId, Achievement achievement)
        {
            if (experienceId <= 0 || achievementId <= 0)
            {
                throw new ArgumentException("Experience ID and Achievement ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(achievement.Description))
            {
                throw new ArgumentException("Achievement description cannot be null or empty.");
            }

            string sql = "UPDATE achievements " +
                         "SET description = @description " +
                         "FROM experience_achievements " +
                         "WHERE achievements.id = experience_achievements.achievement_id " +
                         "AND experience_achievements.experience_id = @experienceId " +
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
                throw new DaoException("An error occurred while updating the achievement by experience ID.", ex);
            }

            return null;
        }

        /*  
            **********************************************************************************************
                                    OPEN SOURCE CONTRIBUTION ACHIEVEMENT CRUD
            **********************************************************************************************
        */

        /*  
            **********************************************************************************************
                                        VOLUNTEER WORK ACHIEVEMENT CRUD
            **********************************************************************************************
        */

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
                        cmd.Parameters.AddWithValue("achievementId", achievementId);

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