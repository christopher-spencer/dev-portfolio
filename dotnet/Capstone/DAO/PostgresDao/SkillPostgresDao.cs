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
        // FIXME use WEBSITE POSTGRES DAO CRUD AS TEMPLATE FOR IMPROVING CRUD METHODS

        /*  
            **********************************************************************************************
                                                    SKILL CRUD
            **********************************************************************************************
        */
        public Skill CreateSkill(Skill skill)
        {
            string sql = "INSERT INTO skills (name) VALUES (@name) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", skill.Name);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    skill.Id = id;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the skill.", ex);
            }

            return skill;
        }

        public Skill GetSkillById(int skillId)
        {
            string sql = "SELECT name, icon_id FROM skills WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", skillId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return MapRowToSkill(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the skill.", ex);
            }

            return null;
        }

        public List<Skill> GetAllSkills()
        {
            List<Skill> skills = new List<Skill>();
            string sql = "SELECT id, name, icon_id FROM skills;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        skills.Add(MapRowToSkill(reader));
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
            string sql = "UPDATE skills SET name = @name WHERE id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@skillId", skillId);
                    cmd.Parameters.AddWithValue("@name", skill.Name);

                    int count = cmd.ExecuteNonQuery();
                    if (count == 1)
                    {
                        return skill;
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
            string sql = "DELETE FROM skills WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", skillId);

                    return cmd.ExecuteNonQuery();
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
            string insertSkillSql = "INSERT INTO skills (name) VALUES (@name) RETURNING id;";
            string insertSideProjectSkillSql = "INSERT INTO sideproject_skills (sideproject_id, skill_id) VALUES (@projectId, @skillId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmdInsertSkill = new NpgsqlCommand(insertSkillSql, connection);
                    cmdInsertSkill.Parameters.AddWithValue("@name", skill.Name);

                    int skillId = (int)cmdInsertSkill.ExecuteScalar();

                    NpgsqlCommand cmdInsertSideProjectSkill = new NpgsqlCommand(insertSideProjectSkillSql, connection);
                    cmdInsertSideProjectSkill.Parameters.AddWithValue("@projectId", projectId);
                    cmdInsertSideProjectSkill.Parameters.AddWithValue("@skillId", skillId);

                    cmdInsertSideProjectSkill.ExecuteNonQuery();

                    skill.Id = skillId;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the skill by project ID.", ex);
            }

            return skill;
        }

        public List<Skill> GetSkillsBySideProjectId(int projectId)
        {
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Skill skill = MapRowToSkill(reader);
                        skills.Add(skill);
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@skillId", skillId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        skill = MapRowToSkill(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the skill by project ID and skill ID.", ex);
            }

            return skill;
        }

        public Skill UpdateSkillBySideProjectId(int projectId, int skillId, Skill skill)
        {
            string sql = "UPDATE skills " +
                         "SET name = @name " +
                         "FROM sideproject_skills " +
                         "WHERE skills.id = sideproject_skills.skill_id " +
                         "AND sideproject_skills.project_id = @projectId " +
                         "AND skills.id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@skillId", skillId); 
                    cmd.Parameters.AddWithValue("@name", skill.Name);

                    int count = cmd.ExecuteNonQuery();
                    if (count == 1)
                    {
                        return skill;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the skill by project ID.", ex);
            }

            return null;
        }

        public int DeleteSkillBySideProjectId(int projectId, int skillId)
        {
            string sql = "DELETE FROM sideproject_skills WHERE sideproject_id = @projectId AND skill_id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@skillId", skillId);

                    return cmd.ExecuteNonQuery();
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

            SetSkillIconIdProperties(reader, skill);

            return skill;
        }

        private void SetSkillIconIdProperties(NpgsqlDataReader reader, Skill skill)
        {
            if (reader["logo_id"] != DBNull.Value)
            {
                skill.IconId = Convert.ToInt32(reader["icon_id"]);

                int iconId = Convert.ToInt32(reader["icon_id"]);
                skill.Icon = _imageDao.GetImageById(iconId);
            }
            else
            {
                skill.IconId = 0;
            }

        }
    }
}
