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

// TODO for DELETES like this that include an OBJECT, when you delete, 
// TODO it doesnt DELETE the associated OBJECT from database, so on front end you need to
// TODO DELETE associations when DELETING OBJECTS that include other OBJECTS ******
        public int DeleteSkillBySideProjectId(int projectId, int skillId)
        {
            if (projectId <= 0 || skillId <= 0)
            {
                throw new ArgumentException("ProjectId and skillId must be greater than zero.");
            }

            string sql = "DELETE FROM sideproject_skills WHERE sideproject_id = @projectId AND skill_id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);
                        cmd.Parameters.AddWithValue("@skillId", skillId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the skill by project ID.", ex);
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
