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
            string sql = "INSERT INTO skills (name, icon_image_name, icon_image_url) VALUES (@name, @icon_image_name, @icon_image_url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", skill.Name);
                    cmd.Parameters.AddWithValue("@icon_image_name", skill.IconImageUrl.Name);
                    cmd.Parameters.AddWithValue("@icon_image_url", skill.IconImageUrl.Url);

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
            string sql = "SELECT name, icon_image_name, icon_image_url FROM skills WHERE id = @id;";

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
            string sql = "SELECT id, name, icon_image_name, icon_image_url FROM skills;";

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
            string sql = "UPDATE skills SET name = @name, icon_image_name = @icon_image_name, icon_image_url = @icon_image_url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", skill.Id);
                    cmd.Parameters.AddWithValue("@name", skill.Name);
                    cmd.Parameters.AddWithValue("@icon_image_name", skill.IconImageUrl.Name);
                    cmd.Parameters.AddWithValue("@icon_image_url", skill.IconImageUrl.Url);

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
                IconImageUrl = new Image
                {
                    Name = Convert.ToString(reader["icon_image_name"]),
                    Url = Convert.ToString(reader["icon_image_url"])
                }
            };
        }
    }
}
