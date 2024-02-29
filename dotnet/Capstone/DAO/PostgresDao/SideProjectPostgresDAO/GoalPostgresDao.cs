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

        /*  
            **********************************************************************************************
                                                    GOAL CRUD
            **********************************************************************************************
        */    
        public Goal CreateGoal(Goal goal)
        {
            string sql = "INSERT INTO goals (description) VALUES (@description) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@description", goal.Description);

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
            string sql = "UPDATE goals SET description = @description WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", goal.Id);
                    cmd.Parameters.AddWithValue("@description", goal.Description);

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

        /*  
            **********************************************************************************************
                                            SIDE PROJECT GOAL CRUD
            **********************************************************************************************
        */              
        public Goal CreateGoalBySideProjectId(int projectId, Goal goal)
        {
            string insertGoalSql = "INSERT INTO goals (description) VALUES (@description) RETURNING id;";
            string insertSideProjectGoalSql = "INSERT INTO sideproject_goals (sideproject_id, goal_id) VALUES (@projectId, @goalId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmdInsertGoal = new NpgsqlCommand(insertGoalSql, connection);
                    cmdInsertGoal.Parameters.AddWithValue("@description", goal.Description);

                    int goalId = (int)cmdInsertGoal.ExecuteScalar();

                    NpgsqlCommand cmdInsertSideProjectGoal = new NpgsqlCommand(insertSideProjectGoalSql, connection);
                    cmdInsertSideProjectGoal.Parameters.AddWithValue("@projectId", projectId);
                    cmdInsertSideProjectGoal.Parameters.AddWithValue("@goalId", goalId);

                    cmdInsertSideProjectGoal.ExecuteNonQuery();

                    goal.Id = goalId;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the goal for the project.", ex);
            }

            return goal;
        }

        public List<Goal> GetGoalsBySideProjectId(int projectId)
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

        public Goal GetGoalBySideProjectId(int projectId, int goalId)
        {
            Goal goal = null;

            string sql = "SELECT g.id, g.description, g.icon_id " +
                         "FROM goals g " +
                         "JOIN sideproject_goals spg ON g.id = spg.goal_id " +
                         "WHERE spg.sideproject_id = @projectId AND g.id = @goalId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@goalId", goalId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        goal = MapRowToGoal(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the goal for the project.", ex);
            }

            return goal;
        }

        public Goal UpdateGoalBySideProjectId(int projectId, Goal updatedGoal)
        {
            string sql = "UPDATE goals " +
                         "SET description = @description " +
                         "FROM sideproject_goals " +
                         "WHERE goals.id = sideproject_goals.goal_id " +
                         "AND sideproject_goals.sideproject_id = @projectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@description", updatedGoal.Description);

                    int count = cmd.ExecuteNonQuery();

                    if (count > 0)
                    {
                        return updatedGoal;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the goal for the project.", ex);
            }

            return null;
        }

        public int DeleteGoalBySideProjectId(int projectId, int goalId)
        {
            string sql = "DELETE FROM sideproject_goals WHERE sideproject_id = @projectId AND goal_id = @goalId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@goalId", goalId);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the goal from the project.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                                GOAL MAP ROW
            **********************************************************************************************
        */
        private Goal MapRowToGoal(NpgsqlDataReader reader)
        {
            Goal goal = new Goal
            {
                Id = Convert.ToInt32(reader["id"]),
                Description = Convert.ToString(reader["description"])
            };

            SetGoalIconIdProperties(reader, goal);

            return goal;
        }
// TODO Goal Image CRUD in ImagePostgresDao 
        private void SetGoalIconIdProperties(NpgsqlDataReader reader, Goal goal)
        {
            if (reader["icon_id"] != DBNull.Value)
            {
                goal.IconId = Convert.ToInt32(reader["icon_id"]);

                int iconId = Convert.ToInt32(reader["icon_id"]);
                goal.Icon = _imageDao.GetImageById(iconId);
            }
            else
            {
                goal.IconId = 0;
            }
        }

    }
}
