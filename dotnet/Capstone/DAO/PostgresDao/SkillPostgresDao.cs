using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class SkillPostgresDao : ISkillDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;

        public SkillPostgresDao(string dbConnectionString, IImageDao imageDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
        }

        /*  
            **********************************************************************************************
                                                    SKILL CRUD
            **********************************************************************************************
        */
        public Skill CreateSkill(Skill skill)
        {
            if (string.IsNullOrEmpty(skill.Name))
            {
                throw new ArgumentException("Skill name cannot be null or empty.");
            }

            string sql = "INSERT INTO skills (name) VALUES (@name) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", skill.Name);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        skill.Id = id;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the skill.", ex);
            }

            return skill;
        }

        public Skill GetSkill(int skillId)
        {
            if (skillId <= 0)
            {
                throw new ArgumentException("SkillId must be greater than zero.");
            }

            string sql = "SELECT id, name, icon_id FROM skills WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", skillId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToSkill(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the skill.", ex);
            }

            return null;
        }

        public List<Skill> GetSkills()
        {
            List<Skill> skills = new List<Skill>();

            string sql = "SELECT id, name, icon_id FROM skills;";

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
                                skills.Add(MapRowToSkill(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the skills.", ex);
            }

            return skills;
        }

        public Skill UpdateSkill(int skillId, Skill skill)
        {
            if (string.IsNullOrEmpty(skill.Name))
            {
                throw new ArgumentException("Skill name cannot be null or empty.");
            }

            if (skillId <= 0)
            {
                throw new ArgumentException("SkillId must be greater than zero.");
            }

            string sql = "UPDATE skills SET name = @name WHERE id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@skillId", skillId);
                        cmd.Parameters.AddWithValue("@name", skill.Name);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return skill;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the skill.", ex);
            }

            return null;
        }

        public int DeleteSkill(int skillId)
        {
            if (skillId <= 0)
            {
                throw new ArgumentException("SkillId must be greater than zero.");
            }

            string sql = "DELETE FROM skills WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", skillId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the skill.", ex);
            }
        }

        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                            PORTFOLIO SKILL CRUD
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */
        public Skill CreateSkillByPortfolioId(int portfolioId, Skill skill)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(skill.Name))
            {
                throw new ArgumentException("Skill name cannot be null or empty.");
            }

            string insertSkillSql = "INSERT INTO skills (name) VALUES (@name) RETURNING id;";
            string insertPortfolioSkillSql = "INSERT INTO portfolio_skills (portfolio_id, skill_id) VALUES (@portfolioId, @skillId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int skillId;

                            using (NpgsqlCommand cmdInsertSkill = new NpgsqlCommand(insertSkillSql, connection))
                            {
                                cmdInsertSkill.Parameters.AddWithValue("@name", skill.Name);
                                cmdInsertSkill.Transaction = transaction;
                                skillId = Convert.ToInt32(cmdInsertSkill.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertPortfolioSkill = new NpgsqlCommand(insertPortfolioSkillSql, connection))
                            {
                                cmdInsertPortfolioSkill.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmdInsertPortfolioSkill.Parameters.AddWithValue("@skillId", skillId);
                                cmdInsertPortfolioSkill.Transaction = transaction;
                                cmdInsertPortfolioSkill.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            skill.Id = skillId;

                            return skill;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the skill by portfolio ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Skill> GetSkillsByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            List<Skill> skills = new List<Skill>();

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN portfolio_skills ps ON s.id = ps.skill_id " +
                         "WHERE ps.portfolio_id = @portfolioId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Skill skill = MapRowToSkill(reader);
                                skills.Add(skill);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving skills by portfolio ID.", ex);
            }

            return skills;
        }

        public Skill GetSkillByPortfolioId(int portfolioId, int skillId)
        {
            if (portfolioId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("PortfolioId and SkillId must be greater than zero.");
            }

            Skill skill = null;

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN portfolio_skills ps ON s.id = ps.skill_id " +
                         "WHERE ps.portfolio_id = @portfolioId AND s.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                skill = MapRowToSkill(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the skill by portfolio ID and skill ID.", ex);
            }

            return skill;
        }

        public Skill UpdateSkillByPortfolioId(int portfolioId, int skillId, Skill skill)
        {
            if (portfolioId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("PortfolioId and skillId must be greater than zero.");
            }

            string sql = "UPDATE skills " +
                         "SET name = @name " +
                         "FROM portfolio_skills " +
                         "WHERE skills.id = portfolio_skills.skill_id " +
                         "AND portfolio_skills.portfolio_id = @portfolioId " +
                         "AND skills.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);
                        cmd.Parameters.AddWithValue("@name", skill.Name);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return skill;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the skill by portfolio ID.", ex);
            }

            return null;
        }

        public int DeleteSkillByPortfolioId(int portfolioId, int skillId)
        {
            if (portfolioId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("PortfolioId and skillId must be greater than zero.");
            }

            string deleteSkillFromPortfolioSql = "DELETE FROM portfolio_skills WHERE portfolio_id = @portfolioId AND skill_id = @skillId;";
            string deleteSkillSql = "DELETE FROM skills WHERE id = @skillId;";

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

                            int? iconId = GetIconIdBySkillId(skillId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillFromPortfolioSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageBySkillId(skillId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the skill by portfolio ID.", ex);
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
                                            SIDE PROJECT SKILL CRUD
            **********************************************************************************************
        */

        public Skill CreateSkillBySideProjectId(int projectId, Skill skill)
        {
            if (projectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(skill.Name))
            {
                throw new ArgumentException("Skill name cannot be null or empty.");
            }

            string insertSkillSql = "INSERT INTO skills (name) VALUES (@name) RETURNING id;";
            string insertSideProjectSkillSql = "INSERT INTO sideproject_skills (sideproject_id, skill_id) VALUES (@projectId, @skillId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int skillId;

                            using (NpgsqlCommand cmdInsertSkill = new NpgsqlCommand(insertSkillSql, connection))
                            {
                                cmdInsertSkill.Parameters.AddWithValue("@name", skill.Name);
                                cmdInsertSkill.Transaction = transaction;
                                skillId = Convert.ToInt32(cmdInsertSkill.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertSideProjectSkill = new NpgsqlCommand(insertSideProjectSkillSql, connection))
                            {
                                cmdInsertSideProjectSkill.Parameters.AddWithValue("@projectId", projectId);
                                cmdInsertSideProjectSkill.Parameters.AddWithValue("@skillId", skillId);
                                cmdInsertSideProjectSkill.Transaction = transaction;
                                cmdInsertSideProjectSkill.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            skill.Id = skillId;

                            return skill;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the skill by project ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Skill> GetSkillsBySideProjectId(int projectId)
        {
            if (projectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than zero.");
            }

            List<Skill> skills = new List<Skill>();

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN sideproject_skills ps ON s.id = ps.skill_id " +
                         "WHERE ps.sideproject_id = @projectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Skill skill = MapRowToSkill(reader);
                                skills.Add(skill);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving skills by project ID.", ex);
            }

            return skills;
        }

        public Skill GetSkillBySideProjectId(int projectId, int skillId)
        {
            if (projectId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("ProjectId and SkillId must be greater than zero.");
            }

            Skill skill = null;

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN sideproject_skills ps ON s.id = ps.skill_id " +
                         "WHERE ps.sideproject_id = @projectId AND s.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                skill = MapRowToSkill(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the skill by project ID and skill ID.", ex);
            }

            return skill;
        }
        public Skill UpdateSkillBySideProjectId(int sideProjectId, int skillId, Skill skill)
        {
            if (sideProjectId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("ProjectId and skillId must be greater than zero.");
            }

            string sql = "UPDATE skills " +
                         "SET name = @name " +
                         "FROM sideproject_skills " +
                         "WHERE skills.id = sideproject_skills.skill_id " +
                         "AND sideproject_skills.sideproject_id = @sideProjectId " +
                         "AND skills.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);
                        cmd.Parameters.AddWithValue("@name", skill.Name);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return skill;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the skill by project ID.", ex);
            }

            return null;
        }

        public int DeleteSkillBySideProjectId(int sideProjectId, int skillId)
        {
            if (sideProjectId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("ProjectId and skillId must be greater than zero.");
            }

            string deleteSkillFromSideProjectSql = "DELETE FROM sideproject_skills WHERE sideproject_id = @sideProjectId AND skill_id = @skillId;";
            string deleteSkillSql = "DELETE FROM skills WHERE id = @skillId;";

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

                            int? iconId = GetIconIdBySkillId(skillId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillFromSideProjectSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageBySkillId(skillId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the skill by project ID.", ex);
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
                                            EXPERIENCE SKILL CRUD
            **********************************************************************************************
        */
        public Skill CreateSkillByExperienceId(int experienceId, Skill skill)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("ExperienceId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(skill.Name))
            {
                throw new ArgumentException("Skill name cannot be null or empty.");
            }

            string insertSkillSql = "INSERT INTO skills (name) VALUES (@name) RETURNING id;";
            string insertExperienceSkillSql = "INSERT INTO experience_skills (experience_id, skill_id) VALUES (@experienceId, @skillId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int skillId;

                            using (NpgsqlCommand cmdInsertSkill = new NpgsqlCommand(insertSkillSql, connection))
                            {
                                cmdInsertSkill.Parameters.AddWithValue("@name", skill.Name);
                                cmdInsertSkill.Transaction = transaction;
                                skillId = Convert.ToInt32(cmdInsertSkill.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertExperienceSkill = new NpgsqlCommand(insertExperienceSkillSql, connection))
                            {
                                cmdInsertExperienceSkill.Parameters.AddWithValue("@experienceId", experienceId);
                                cmdInsertExperienceSkill.Parameters.AddWithValue("@skillId", skillId);
                                cmdInsertExperienceSkill.Transaction = transaction;
                                cmdInsertExperienceSkill.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            skill.Id = skillId;

                            return skill;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the skill by experience ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Skill> GetSkillsByExperienceId(int experienceId)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("ExperienceId must be greater than zero.");
            }

            List<Skill> skills = new List<Skill>();

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN experience_skills es ON s.id = es.skill_id " +
                         "WHERE es.experience_id = @experienceId;";

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
                                Skill skill = MapRowToSkill(reader);
                                skills.Add(skill);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving skills by experience ID.", ex);
            }

            return skills;
        }

        public Skill GetSkillByExperienceId(int experienceId, int skillId)
        {
            if (experienceId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("ExperienceId and SkillId must be greater than zero.");
            }

            Skill skill = null;

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN experience_skills es ON s.id = es.skill_id " +
                         "WHERE es.experience_id = @experienceId AND s.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                skill = MapRowToSkill(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the skill by experience ID and skill ID.", ex);
            }

            return skill;
        }

        public Skill UpdateSkillByExperienceId(int experienceId, int skillId, Skill skill)
        {
            if (experienceId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("ExperienceId and skillId must be greater than zero.");
            }

            string sql = "UPDATE skills " +
                         "SET name = @name " +
                         "FROM experience_skills " +
                         "WHERE skills.id = experience_skills.skill_id " +
                         "AND experience_skills.experience_id = @experienceId " +
                         "AND skills.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);
                        cmd.Parameters.AddWithValue("@name", skill.Name);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return skill;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the skill by experience ID.", ex);
            }

            return null;
        }

        public int DeleteSkillByExperienceId(int experienceId, int skillId)
        {
            if (experienceId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("ExperienceId and skillId must be greater than zero.");
            }

            string deleteSkillFromExperienceSql = "DELETE FROM experience_skills WHERE experience_id = @experienceId AND skill_id = @skillId;";
            string deleteSkillSql = "DELETE FROM skills WHERE id = @skillId;";

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

                            int? iconId = GetIconIdBySkillId(skillId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillFromExperienceSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@experienceId", experienceId);
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageBySkillId(skillId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the skill by experience ID.", ex);
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
                                            CREDENTIAL SKILL CRUD
            **********************************************************************************************
        */
        public Skill CreateSkillByCredentialId(int credentialId, Skill skill)
        {
            if (credentialId <= 0)
            {
                throw new ArgumentException("CredentialId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(skill.Name))
            {
                throw new ArgumentException("Skill name cannot be null or empty.");
            }

            string insertSkillSql = "INSERT INTO skills (name) VALUES (@name) RETURNING id;";
            string insertCredentialSkillSql = "INSERT INTO credential_skills (credential_id, skill_id) VALUES (@credentialId, @skillId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int skillId;

                            using (NpgsqlCommand cmdInsertSkill = new NpgsqlCommand(insertSkillSql, connection))
                            {
                                cmdInsertSkill.Parameters.AddWithValue("@name", skill.Name);
                                cmdInsertSkill.Transaction = transaction;
                                skillId = Convert.ToInt32(cmdInsertSkill.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertCredentialSkill = new NpgsqlCommand(insertCredentialSkillSql, connection))
                            {
                                cmdInsertCredentialSkill.Parameters.AddWithValue("@credentialId", credentialId);
                                cmdInsertCredentialSkill.Parameters.AddWithValue("@skillId", skillId);
                                cmdInsertCredentialSkill.Transaction = transaction;
                                cmdInsertCredentialSkill.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            skill.Id = skillId;

                            return skill;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the skill by credential ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Skill> GetSkillsByCredentialId(int credentialId)
        {
            if (credentialId <= 0)
            {
                throw new ArgumentException("CredentialId must be greater than zero.");
            }

            List<Skill> skills = new List<Skill>();

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN credential_skills cs ON s.id = cs.skill_id " +
                         "WHERE cs.credential_id = @credentialId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Skill skill = MapRowToSkill(reader);
                                skills.Add(skill);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving skills by credential ID.", ex);
            }

            return skills;
        }

        public Skill GetSkillByCredentialId(int credentialId, int skillId)
        {
            if (credentialId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("CredentialId and SkillId must be greater than zero.");
            }

            Skill skill = null;

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN credential_skills cs ON s.id = cs.skill_id " +
                         "WHERE cs.credential_id = @credentialId AND s.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                skill = MapRowToSkill(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the skill by credential ID and skill ID.", ex);
            }

            return skill;
        }

        public Skill UpdateSkillByCredentialId(int credentialId, int skillId, Skill skill)
        {
            if (credentialId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("CredentialId and skillId must be greater than zero.");
            }

            string sql = "UPDATE skills " +
                         "SET name = @name " +
                         "FROM credential_skills " +
                         "WHERE skills.id = credential_skills.skill_id " +
                         "AND credential_skills.credential_id = @credentialId " +
                         "AND skills.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);
                        cmd.Parameters.AddWithValue("@name", skill.Name);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return skill;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the skill by credential ID.", ex);
            }

            return null;
        }

        public int DeleteSkillByCredentialId(int credentialId, int skillId)
        {
            if (credentialId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("CredentialId and skillId must be greater than zero.");
            }

            string deleteSkillFromCredentialSql = "DELETE FROM credential_skills WHERE credential_id = @credentialId AND skill_id = @skillId;";
            string deleteSkillSql = "DELETE FROM skills WHERE id = @skillId;";

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

                            int? iconId = GetIconIdBySkillId(skillId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillFromCredentialSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@credentialId", credentialId);
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageBySkillId(skillId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the skill by credential ID.", ex);
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
                                     OPEN SOURCE CONTRIBUTION SKILL CRUD
            **********************************************************************************************
        */
        public Skill CreateSkillByOpenSourceContributionId(int contributionId, Skill skill)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("ContributionId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(skill.Name))
            {
                throw new ArgumentException("Skill name cannot be null or empty.");
            }

            string insertSkillSql = "INSERT INTO skills (name) VALUES (@name) RETURNING id;";
            string insertContributionSkillSql = "INSERT INTO open_source_contribution_skills (contribution_id, skill_id) VALUES (@contributionId, @skillId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int skillId;

                            using (NpgsqlCommand cmdInsertSkill = new NpgsqlCommand(insertSkillSql, connection))
                            {
                                cmdInsertSkill.Parameters.AddWithValue("@name", skill.Name);
                                cmdInsertSkill.Transaction = transaction;
                                skillId = Convert.ToInt32(cmdInsertSkill.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertContributionSkill = new NpgsqlCommand(insertContributionSkillSql, connection))
                            {
                                cmdInsertContributionSkill.Parameters.AddWithValue("@contributionId", contributionId);
                                cmdInsertContributionSkill.Parameters.AddWithValue("@skillId", skillId);
                                cmdInsertContributionSkill.Transaction = transaction;
                                cmdInsertContributionSkill.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            skill.Id = skillId;

                            return skill;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the skill by contribution ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Skill> GetSkillsByOpenSourceContributionId(int contributionId)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("ContributionId must be greater than zero.");
            }

            List<Skill> skills = new List<Skill>();

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN open_source_contribution_skills ocs ON s.id = ocs.skill_id " +
                         "WHERE ocs.contribution_id = @contributionId;";

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
                                Skill skill = MapRowToSkill(reader);
                                skills.Add(skill);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving skills by contribution ID.", ex);
            }

            return skills;
        }

        public Skill GetSkillByOpenSourceContributionId(int contributionId, int skillId)
        {
            if (contributionId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("ContributionId and SkillId must be greater than zero.");
            }

            Skill skill = null;

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN open_source_contribution_skills ocs ON s.id = ocs.skill_id " +
                         "WHERE ocs.contribution_id = @contributionId AND s.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                skill = MapRowToSkill(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the skill by contribution ID and skill ID.", ex);
            }

            return skill;
        }

        public Skill UpdateSkillByOpenSourceContributionId(int contributionId, int skillId, Skill skill)
        {
            if (contributionId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("ContributionId and skillId must be greater than zero.");
            }

            string sql = "UPDATE skills " +
                         "SET name = @name " +
                         "FROM open_source_contribution_skills " +
                         "WHERE skills.id = open_source_contribution_skills.skill_id " +
                         "AND open_source_contribution_skills.contribution_id = @contributionId " +
                         "AND skills.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);
                        cmd.Parameters.AddWithValue("@name", skill.Name);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return skill;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the skill by contribution ID.", ex);
            }

            return null;
        }

        public int DeleteSkillByOpenSourceContributionId(int contributionId, int skillId)
        {
            if (contributionId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("ContributionId and skillId must be greater than zero.");
            }

            string deleteSkillFromContributionSql = "DELETE FROM open_source_contribution_skills WHERE contribution_id = @contributionId AND skill_id = @skillId;";
            string deleteSkillSql = "DELETE FROM skills WHERE id = @skillId;";

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

                            int? iconId = GetIconIdBySkillId(skillId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillFromContributionSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@contributionId", contributionId);
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageBySkillId(skillId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the skill by contribution ID.", ex);
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
                                        VOLUNTEER WORK SKILL CRUD
            **********************************************************************************************
        */
        public Skill CreateSkillByVolunteerWorkId(int volunteerWorkId, Skill skill)
        {
            if (volunteerWorkId <= 0)
            {
                throw new ArgumentException("VolunteerWorkId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(skill.Name))
            {
                throw new ArgumentException("Skill name cannot be null or empty.");
            }

            string insertSkillSql = "INSERT INTO skills (name) VALUES (@name) RETURNING id;";
            string insertVolunteerWorkSkillSql = "INSERT INTO volunteer_work_skills (volunteer_id, skill_id) VALUES (@volunteerWorkId, @skillId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int skillId;

                            using (NpgsqlCommand cmdInsertSkill = new NpgsqlCommand(insertSkillSql, connection))
                            {
                                cmdInsertSkill.Parameters.AddWithValue("@name", skill.Name);
                                cmdInsertSkill.Transaction = transaction;
                                skillId = Convert.ToInt32(cmdInsertSkill.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertVolunteerWorkSkill = new NpgsqlCommand(insertVolunteerWorkSkillSql, connection))
                            {
                                cmdInsertVolunteerWorkSkill.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
                                cmdInsertVolunteerWorkSkill.Parameters.AddWithValue("@skillId", skillId);
                                cmdInsertVolunteerWorkSkill.Transaction = transaction;
                                cmdInsertVolunteerWorkSkill.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            skill.Id = skillId;

                            return skill;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the skill by volunteer work ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Skill> GetSkillsByVolunteerWorkId(int volunteerWorkId)
        {
            if (volunteerWorkId <= 0)
            {
                throw new ArgumentException("VolunteerWorkId must be greater than zero.");
            }

            List<Skill> skills = new List<Skill>();

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN volunteer_work_skills vws ON s.id = vws.skill_id " +
                         "WHERE vws.volunteer_id = @volunteerWorkId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Skill skill = MapRowToSkill(reader);
                                skills.Add(skill);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving skills by volunteer work ID.", ex);
            }

            return skills;
        }

        public Skill GetSkillByVolunteerWorkId(int volunteerWorkId, int skillId)
        {
            if (volunteerWorkId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("VolunteerWorkId and SkillId must be greater than zero.");
            }

            Skill skill = null;

            string sql = "SELECT s.id, s.name, s.icon_id " +
                         "FROM skills s " +
                         "JOIN volunteer_work_skills vws ON s.id = vws.skill_id " +
                         "WHERE vws.volunteer_id = @volunteerWorkId AND s.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                skill = MapRowToSkill(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the skill by volunteer work ID and skill ID.", ex);
            }

            return skill;
        }

        public Skill UpdateSkillByVolunteerWorkId(int volunteerWorkId, int skillId, Skill skill)
        {
            if (volunteerWorkId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("VolunteerWorkId and skillId must be greater than zero.");
            }

            string sql = "UPDATE skills " +
                         "SET name = @name " +
                         "FROM volunteer_work_skills " +
                         "WHERE skills.id = volunteer_work_skills.skill_id " +
                         "AND volunteer_work_skills.volunteer_id = @volunteerWorkId " +
                         "AND skills.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);
                        cmd.Parameters.AddWithValue("@name", skill.Name);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return skill;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the skill by volunteer work ID.", ex);
            }

            return null;
        }

        public int DeleteSkillByVolunteerWorkId(int volunteerWorkId, int skillId)
        {
            if (volunteerWorkId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("VolunteerWorkId and skillId must be greater than zero.");
            }

            string deleteSkillFromVolunteerWorkSql = "DELETE FROM volunteer_work_skills WHERE volunteer_id = @volunteerWorkId AND skill_id = @skillId;";
            string deleteSkillSql = "DELETE FROM skills WHERE id = @skillId;";

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

                            int? iconId = GetIconIdBySkillId(skillId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillFromVolunteerWorkSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageBySkillId(skillId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@skillId", skillId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the skill by volunteer work ID.", ex);
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

        private int? GetIconIdBySkillId(int skillId)
        {
            string sql = "SELECT icon_id FROM skills WHERE id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@skillId", skillId);

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
                Console.WriteLine("Error retrieving icon ID by skill ID: " + ex.Message);
                return null;
            }
        }

        /*  
            **********************************************************************************************
                                                SKILL MAP ROW
            **********************************************************************************************
        */

        private Skill MapRowToSkill(NpgsqlDataReader reader)
        {
            Skill skill = new Skill
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"])
            };

            int skillId = skill.Id;

            SetSkillIconIdProperties(reader, skill, skillId);

            return skill;
        }

        private void SetSkillIconIdProperties(NpgsqlDataReader reader, Skill skill, int skillId)
        {
            if (reader["icon_id"] != DBNull.Value)
            {
                skill.IconId = Convert.ToInt32(reader["icon_id"]);

                skill.Icon = _imageDao.GetImageBySkillId(skillId);
            }
            else
            {
                skill.IconId = 0;
            }

        }
    }
}
