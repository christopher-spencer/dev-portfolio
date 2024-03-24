using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class HobbyPostgresDao : IHobbyDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;

        public HobbyPostgresDao(string dbConnectionString, IImageDao imageDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
        }

        /*  
            **********************************************************************************************
                                                HOBBY CRUD
            **********************************************************************************************
        */

        public Hobby GetHobby(int hobbyId)
        {
            if (hobbyId <= 0)
            {
                throw new ArgumentException("Hobby ID must be greater than zero.");
            }

            string sql = "SELECT id, description, icon_id FROM hobbies WHERE id = @hobbyId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@hobbyId", hobbyId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToHobby(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the hobby.", ex);
            }

            return null;
        }

        public List<Hobby> GetHobbies()
        {
            List<Hobby> hobbies = new List<Hobby>();

            string sql = "SELECT id, description, icon_id FROM hobbies;";

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
                                Hobby hobby = MapRowToHobby(reader);

                                hobbies.Add(hobby);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving hobbies.", ex);
            }

            return hobbies;
        }

        /*  
            **********************************************************************************************
                                            PORTFOLIO HOBBY CRUD
            **********************************************************************************************
        */

        public Hobby CreateHobbyByPortfolioId(int portfolioId, Hobby hobby)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(hobby.Description))
            {
                throw new ArgumentException("Hobby cannot be null or empty.");
            }

            string insertHobbySql = "INSERT INTO hobbies (description) VALUES (@description) RETURNING id;";
            string insertPortfolioHobbySql = "INSERT INTO portfolio_hobbies (portfolio_id, hobby_id) VALUES (@portfolioId, @hobbyId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int hobbyId;

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertHobbySql, connection))
                            {
                                cmd.Parameters.AddWithValue("@description", hobby.Description);
                                cmd.Transaction = transaction;
                                hobbyId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertPortfolioHobbySql, connection))
                            {
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@hobbyId", hobbyId);
                                cmd.Transaction = transaction;
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            hobby.Id = hobbyId;

                            return hobby;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the hobby by portfolio ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Hobby> GetHobbiesByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            List<Hobby> hobbies = new List<Hobby>();

            string sql = "SELECT h.id, h.description, h.icon_id " +
                         "FROM hobbies h " +
                         "JOIN portfolio_hobbies ph ON h.id = ph.hobby_id " +
                         "WHERE ph.portfolio_id = @portfolioId;";

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
                                Hobby hobby = MapRowToHobby(reader);

                                hobbies.Add(hobby);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving hobbies by portfolio ID.", ex);
            }

            return hobbies;
        }

        public Hobby GetHobbyByPortfolioId(int portfolioId, int hobbyId)
        {
            if (portfolioId <= 0 || hobbyId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Hobby ID must be greater than zero.");
            }

            string sql = "SELECT h.id, h.description, h.icon_id " +
                         "FROM hobbies h " +
                         "JOIN portfolio_hobbies ph ON h.id = ph.hobby_id " +
                         "WHERE ph.portfolio_id = @portfolioId AND ph.hobby_id = @hobbyId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@hobbyId", hobbyId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToHobby(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the hobby by portfolio ID.", ex);
            }

            return null;
        }

        public Hobby UpdateHobbyByPortfolioId(int portfolioId, int hobbyId, Hobby hobby)
        {
            if (portfolioId <= 0 || hobbyId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Hobby ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(hobby.Description))
            {
                throw new ArgumentException("Hobby cannot be null or empty.");
            }

            string sql = "UPDATE hobbies " +
                         "SET description = @description " +
                         "FROM portfolio_hobbies " +
                         "WHERE hobbies.id = portfolio_hobbies.hobby_id " +
                         "AND portfolio_hobbies.portfolio_id = @portfolioId " +
                         "AND hobbies.id = @hobbyId;";
            
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@hobbyId", hobbyId);
                        cmd.Parameters.AddWithValue("@description", hobby.Description);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return hobby;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the hobby by portfolio ID.", ex);
            }

            return null;
        }

        public int DeleteHobbyByPortfolioId(int portfolioId, int hobbyId)
        {
            if (portfolioId <= 0 || hobbyId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Hobby ID must be greater than zero.");
            }

            string deletePortfolioHobbySql = "DELETE FROM portfolio_hobbies WHERE portfolio_id = @portfolioId AND hobby_id = @hobbyId;";
            string deleteHobbySql = "DELETE FROM hobbies WHERE id = @hobbyId;";

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
                            
                            int? iconId = GetIconByHobbyId(hobbyId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deletePortfolioHobbySql, connection))
                            {
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@hobbyId", hobbyId);
                                cmd.Transaction = transaction;
                                cmd.ExecuteNonQuery();
                            }

                            if (iconId.HasValue)
                            {
                                _imageDao.DeleteImageByHobbyId(hobbyId, iconId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteHobbySql, connection))
                            {
                                cmd.Parameters.AddWithValue("@hobbyId", hobbyId);
                                cmd.Transaction = transaction;

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the hobby by portfolio ID.", ex);
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

        private int? GetIconByHobbyId(int hobbyId)
        {
            string sql = "SELECT icon_id FROM hobbies WHERE id = @hobbyId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("hobbyId", hobbyId);

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
                Console.WriteLine("Error retrieving icon ID by hobby ID." + ex.Message);
                return null;
            }
        }

        /*  
            **********************************************************************************************
                                               HOBBY MAP ROW
            **********************************************************************************************
        */

        private Hobby MapRowToHobby(NpgsqlDataReader reader)
        {
            Hobby hobby = new Hobby
            {
                Id = Convert.ToInt32(reader["id"]),
                Description = Convert.ToString(reader["description"])
            };
            
            int hobbyId = hobby.Id;

            SetHobbyIconIdProperties(reader, hobby, hobbyId);

            return hobby;
        }

        private void SetHobbyIconIdProperties(NpgsqlDataReader reader, Hobby hobby, int hobbyId)
        {
            if (reader["icon_id"] != DBNull.Value)
            {
                hobby.IconId = Convert.ToInt32(reader["icon_id"]);

                hobby.Icon = _imageDao.GetImageByHobbyId(hobbyId);
            }
        }


    }
}
