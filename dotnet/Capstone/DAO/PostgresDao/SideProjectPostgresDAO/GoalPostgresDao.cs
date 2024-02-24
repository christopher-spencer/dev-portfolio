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
        private readonly IImageDao _imageDao;

        public GoalPostgresDao(string dbConnectionString, IImageDao imageDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
        }

        public Goal CreateGoal(Goal goal)
        {
            string sql = "INSERT INTO goals (description, icon_id) VALUES (@description, @iconId) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@description", goal.Description);
                    cmd.Parameters.AddWithValue("@iconId", goal.Icon.Id);

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

            string sql = "SELECT g.id, g.description, g.icon_id " +
                         "FROM goals g " +
                         "JOIN sideproject_goals spg ON g.id = spg.goal_id " +
                         "WHERE spg.sideproject_id = @projectId;";

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
            string sql = "SELECT description, icon_id FROM goals WHERE id = @id;";

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
            string sql = "SELECT id, description, icon_id FROM goals;";

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
            string sql = "UPDATE goals SET description = @description, icon_id = @iconId WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", goal.Id);
                    cmd.Parameters.AddWithValue("@description", goal.Description);
                    cmd.Parameters.AddWithValue("@iconId", goal.Icon.Id);

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
            Goal goal = new Goal
            {
                Id = Convert.ToInt32(reader["id"]),
                Description = Convert.ToString(reader["description"]),
                IconId = Convert.ToInt32(reader["icon_id"])
            };

            if (reader["icon_id"] != DBNull.Value)
            {
                int iconId = Convert.ToInt32(reader["icon_id"]);
                goal.Icon = _imageDao.GetImageById(iconId);
            }

            return goal;
        }
    }
}
