using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class GoalPostgresDao : IGoalDao
    {
        private readonly string connectionString;

        public GoalPostgresDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Goal CreateGoal(Goal goal)
        {
            string sql = "INSERT INTO goals (description, icon_image_name, icon_image_url) VALUES (@description, @icon_image_name, @icon_image_url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@description", goal.Description);
                    cmd.Parameters.AddWithValue("@icon_image_name", goal.IconImageUrl.Name);
                    cmd.Parameters.AddWithValue("@icon_image_url", goal.IconImageUrl.Url);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    goal.Id = id;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the goal.", ex);
            }

            return goal;
        }

        public List<Goal> GetGoalsAndObjectivesByProjectId(int projectId)
        {
            List<Goal> goals = new List<Goal>();

            string sql = "SELECT g.id, g.description, g.icon_image_name, g.icon_image_url " +
                         "FROM goals g " +
                         "JOIN project_goals pg ON g.id = pg.goal_id " +
                         "WHERE pg.project_id = @projectId;";

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
                        goals.Add(MapRowToGoal(reader));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving goals and objectives for the project.", ex);
            }

            return goals;
        }

        public Goal GetGoalById(int goalId)
        {
            string sql = "SELECT description, icon_image_name, icon_image_url FROM goals WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", goalId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return MapRowToGoal(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the goal.", ex);
            }

            return null;
        }

        public List<Goal> GetAllGoals()
        {
            List<Goal> goals = new List<Goal>();
            string sql = "SELECT id, description, icon_image_name, icon_image_url FROM goals;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        goals.Add(MapRowToGoal(reader));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the goals.", ex);
            }

            return goals;
        }

        public Goal UpdateGoal(Goal goal)
        {
            string sql = "UPDATE goals SET description = @description, icon_image_name = @icon_image_name, icon_image_url = @icon_image_url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", goal.Id);
                    cmd.Parameters.AddWithValue("@description", goal.Description);
                    cmd.Parameters.AddWithValue("@icon_image_name", goal.IconImageUrl.Name);
                    cmd.Parameters.AddWithValue("@icon_image_url", goal.IconImageUrl.Url);

                    int count = cmd.ExecuteNonQuery();
                    if (count == 1)
                    {
                        return goal;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the goal.", ex);
            }

            return null;
        }

        public int DeleteGoalById(int goalId)
        {
            string sql = "DELETE FROM goals WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", goalId);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the goal.", ex);
            }
        }

        private Goal MapRowToGoal(NpgsqlDataReader reader)
        {
            return new Goal
            {
                Id = Convert.ToInt32(reader["id"]),
                Description = Convert.ToString(reader["description"]),
                IconImageUrl = new Image
                {
                    Name = Convert.ToString(reader["icon_image_name"]),
                    Url = Convert.ToString(reader["icon_image_url"])
                }
            };
        }
    }
}
