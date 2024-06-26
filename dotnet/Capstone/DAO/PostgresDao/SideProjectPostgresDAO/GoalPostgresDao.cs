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

        public Goal GetGoal(int goalId)
        {
            if (goalId <= 0)
            {
                throw new ArgumentException("GoalId must be greater than zero.");
            }

            string sql = "SELECT id, description, icon_id FROM goals WHERE id = @goalId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@goalId", goalId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToGoal(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the goal.", ex);
            }

            return null;
        }

        public List<Goal> GetGoals()
        {
            List<Goal> goals = new List<Goal>();

            string sql = "SELECT id, description, icon_id FROM goals;";

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
                                goals.Add(MapRowToGoal(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the goals.", ex);
            }

            return goals;
        }

        /*  
            **********************************************************************************************
                                            SIDE PROJECT GOAL CRUD
            **********************************************************************************************
        */
        public Goal CreateGoalBySideProjectId(int sideProjectId, Goal goal)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            CheckGoalDescriptionIsNotNullOrEmpty(goal);

            string insertGoalSql = "INSERT INTO goals (description) VALUES (@description) RETURNING id;";
            string insertSideProjectGoalSql = "INSERT INTO sideproject_goals (sideproject_id, goal_id) VALUES (@sideProjectId, @goalId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int goalId;

                            using (NpgsqlCommand cmdInsertGoal = new NpgsqlCommand(insertGoalSql, connection))
                            {
                                cmdInsertGoal.Parameters.AddWithValue("@description", goal.Description);
                                cmdInsertGoal.Transaction = transaction;
                                goalId = Convert.ToInt32(cmdInsertGoal.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertSideProjectGoal = new NpgsqlCommand(insertSideProjectGoalSql, connection))
                            {
                                cmdInsertSideProjectGoal.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmdInsertSideProjectGoal.Parameters.AddWithValue("@goalId", goalId);
                                cmdInsertSideProjectGoal.Transaction = transaction;
                                cmdInsertSideProjectGoal.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            goal.Id = goalId;

                            return goal;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new DaoException("An error occurred while creating the goal for the side project.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Goal> GetGoalsBySideProjectId(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            List<Goal> goals = new List<Goal>();

            string sql = "SELECT g.id, g.description, g.icon_id " +
                         "FROM goals g " +
                         "JOIN sideproject_goals spg ON g.id = spg.goal_id " +
                         "WHERE spg.sideproject_id = @sideProjectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                goals.Add(MapRowToGoal(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving goals and objectives for the side project.", ex);
            }

            return goals;
        }

        public Goal GetGoalBySideProjectId(int sideProjectId, int goalId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            Goal goal = null;

            string sql = "SELECT g.id, g.description, g.icon_id " +
                         "FROM goals g " +
                         "JOIN sideproject_goals spg ON g.id = spg.goal_id " +
                         "WHERE spg.sideproject_id = @sideProjectId AND g.id = @goalId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@goalId", goalId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                goal = MapRowToGoal(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the goal for the side project.", ex);
            }

            return goal;
        }

        public Goal UpdateGoalBySideProjectId(int sideProjectId, int goalId, Goal goal)
        {
            if (sideProjectId <= 0 || goalId <= 0)
            {
                throw new ArgumentException("SideProjectId and goalId must be greater than zero.");
            }

            CheckGoalDescriptionIsNotNullOrEmpty(goal);

            string sql = "UPDATE goals " +
                         "SET description = @description " +
                         "FROM sideproject_goals " +
                         "WHERE goals.id = sideproject_goals.goal_id " +
                         "AND sideproject_goals.sideproject_id = @sideProjectId " +
                         "AND goals.id = @goalId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@goalId", goalId);
                        cmd.Parameters.AddWithValue("@description", goal.Description);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return goal;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the goal for the side project.", ex);
            }

            return null;
        }

        public int DeleteGoalBySideProjectId(int sideProjectId, int goalId)
        {
            if (sideProjectId <= 0 || goalId <= 0)
            {
                throw new ArgumentException("SideProjectId and goalId must be greater than zero.");
            }

            string deleteGoalFromSideProjectSql = "DELETE FROM sideproject_goals WHERE sideproject_id = @sideProjectId AND goal_id = @goalId;";
            string deleteGoalSql = "DELETE FROM goals WHERE id = @goalId;";

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

                            int? iconId = GetIconIdByGoalId(goalId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteGoalFromSideProjectSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmd.Parameters.AddWithValue("@goalId", goalId);

                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageByGoalId(goalId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteGoalSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@goalId", goalId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the goal by side project ID.", ex);
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

        private int? GetIconIdByGoalId(int goalId)
        {
            string sql = "SELECT icon_id FROM goals WHERE id = @goalId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@goalId", goalId);

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
                Console.WriteLine("Error retrieving icon ID by goal ID: " + ex.Message);
                return null;
            }
        }

        private void CheckGoalDescriptionIsNotNullOrEmpty(Goal goal)
        {
            if (string.IsNullOrEmpty(goal.Description))
            {
                throw new ArgumentException("Goal description cannot be null or empty.");
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

            int goalId = goal.Id;

            SetGoalIconIdProperties(reader, goal, goalId);

            return goal;
        }

        private void SetGoalIconIdProperties(NpgsqlDataReader reader, Goal goal, int goalId)
        {
            if (reader["icon_id"] != DBNull.Value)
            {
                goal.IconId = Convert.ToInt32(reader["icon_id"]);

                goal.Icon = _imageDao.GetImageByGoalId(goalId);
            }
            else
            {
                goal.IconId = 0;
            }
        }

    }
}
