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

        public SkillPostgresDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Skill CreateSkill(Skill skill)
        {
            string sql = "INSERT INTO skills (name, icon_name, icon_url) VALUES (@name, @icon_name, @icon_url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", skill.Name);
                    cmd.Parameters.AddWithValue("@icon_name", skill.Icon.Name);
                    cmd.Parameters.AddWithValue("@icon_url", skill.Icon.Url);

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

        public List<Skill> GetSkillsByProjectId(int projectId)
        {
            List<Skill> skills = new List<Skill>();

            string sql = "SELECT s.id, s.name, s.icon_name, s.icon_url " +
                         "FROM skills s " +
                         "JOIN side_project_skills ps ON s.id = ps.skill_id " +
                         "WHERE ps.project_id = @projectId;";

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

        public Skill GetSkillById(int skillId)
        {
            string sql = "SELECT name, icon_name, icon_url FROM skills WHERE id = @id;";

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
            string sql = "SELECT id, name, icon_name, icon_url FROM skills;";

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

        public Skill UpdateSkill(Skill skill)
        {
            string sql = "UPDATE skills SET name = @name, icon_name = @icon_name, icon_url = @icon_url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", skill.Id);
                    cmd.Parameters.AddWithValue("@name", skill.Name);
                    cmd.Parameters.AddWithValue("@icon_name", skill.Icon.Name);
                    cmd.Parameters.AddWithValue("@icon_url", skill.Icon.Url);

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

        public int DeleteSkillById(int skillId)
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

        private Skill MapRowToSkill(NpgsqlDataReader reader)
        {
            return new Skill
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Icon = new Image
                {
                    Name = Convert.ToString(reader["icon_name"]),
                    Url = Convert.ToString(reader["icon_url"])
                }
            };
        }
    }
}
