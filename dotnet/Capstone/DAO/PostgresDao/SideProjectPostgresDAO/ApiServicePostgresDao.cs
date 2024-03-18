using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class ApiServicePostgresDao : IApiServiceDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;
        private readonly IWebsiteDao _websiteDao;

        public ApiServicePostgresDao(string dbConnectionString, IImageDao imageDao, IWebsiteDao websiteDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
            this._websiteDao = websiteDao;
        }

        /*  
            **********************************************************************************************
                                            APIS AND SERVICES CRUD
            **********************************************************************************************
        */
        public ApiService CreateAPIOrService(ApiService apiService)
        {
            if (string.IsNullOrEmpty(apiService.Name))
            {
                throw new ArgumentException("Api or Service name cannot be null or empty.");
            }

            string sql = "INSERT INTO apis_and_services (name, description) " +
                         "VALUES (@name, @description) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", apiService.Name);
                        cmd.Parameters.AddWithValue("@description", apiService.Description);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        apiService.Id = id;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the API service.", ex);
            }

            return apiService;
        }

        public ApiService GetAPIOrServiceById(int apiServiceId)
        {
            if (apiServiceId <= 0)
            {
                throw new ArgumentException("ApiServiceId must be greater than zero.");
            }

            ApiService apiService = null;

            string sql = "SELECT id, name, description, website_id, logo_id " +
                         "FROM apis_and_services " +
                         "WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", apiServiceId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                apiService = MapRowToAPIsAndServices(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the API service.", ex);
            }

            return apiService;
        }

        public List<ApiService> GetAPIsAndServices()
        {
            List<ApiService> apiServices = new List<ApiService>();

            string sql = "SELECT id, name, description, website_id, logo_id FROM apis_and_services;";

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
                                apiServices.Add(MapRowToAPIsAndServices(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the API or Services.", ex);
            }

            return apiServices;
        }

        public ApiService UpdateAPIOrService(int apiServiceId, ApiService apiService)
        {
            if (string.IsNullOrEmpty(apiService.Name))
            {
                throw new ArgumentException("ApiService name cannot be null or empty.");
            }

            if (apiServiceId <= 0)
            {
                throw new ArgumentException("ApiServiceId must be greater than zero.");
            }

            string sql = "UPDATE apis_and_services " +
                         "SET name = @name, description = @description " +
                         "WHERE id = @apiServiceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                        cmd.Parameters.AddWithValue("@name", apiService.Name);
                        cmd.Parameters.AddWithValue("@description", apiService.Description);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return apiService;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the API service.", ex);
            }

            return null;
        }

        public int DeleteAPIOrService(int apiServiceId)
        {
            if (apiServiceId <= 0)
            {
                throw new ArgumentException("ApiServiceId must be greater than zero.");
            }

            string sql = "DELETE FROM api_and_services WHERE id = @apiServiceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the API service.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                     SIDE PROJECT APIS AND SERVICES CRUD
            **********************************************************************************************
        */
        public ApiService CreateAPIOrServiceBySideProjectId(int sideProjectId, ApiService apiService)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(apiService.Name))
            {
                throw new ArgumentException("API service name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(apiService.Description))
            {
                throw new ArgumentException("API service description cannot be null or empty.");
            }

            string insertApiServiceSql = "INSERT INTO apis_and_services (name, description) " +
                                         "VALUES (@name, @description) RETURNING id;";
            string insertSideProjectApiServiceSql = "INSERT INTO sideproject_apis_and_services (sideproject_id, apiservice_id) " +
                                                     "VALUES (@sideProjectId, @apiServiceId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int apiServiceId;

                            using (NpgsqlCommand cmdInsertApiService = new NpgsqlCommand(insertApiServiceSql, connection))
                            {
                                cmdInsertApiService.Parameters.AddWithValue("@name", apiService.Name);
                                cmdInsertApiService.Parameters.AddWithValue("@description", apiService.Description);
                                cmdInsertApiService.Transaction = transaction;
                                apiServiceId = Convert.ToInt32(cmdInsertApiService.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertSideProjectApiService = new NpgsqlCommand(insertSideProjectApiServiceSql, connection))
                            {
                                cmdInsertSideProjectApiService.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmdInsertSideProjectApiService.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                                cmdInsertSideProjectApiService.Transaction = transaction;
                                cmdInsertSideProjectApiService.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            apiService.Id = apiServiceId;

                            return apiService;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new DaoException("An error occurred while creating the API or Service by SideProjectId.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<ApiService> GetAPIsAndServicesBySideProjectId(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            List<ApiService> apiServices = new List<ApiService>();

            string sql = "SELECT a.id, a.name, a.description, a.website_id, a.logo_id " +
                         "FROM apis_and_services a " +
                         "JOIN sideproject_apis_and_services pas ON a.id = pas.apiservice_id " +
                         "WHERE pas.sideproject_id = @sideProjectId;";

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
                                apiServices.Add(MapRowToAPIsAndServices(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving APIs or Services by SideProjectId.", ex);
            }

            return apiServices;
        }

        public ApiService GetAPIOrServiceBySideProjectId(int sideProjectId, int apiServiceId)
        {
            if (sideProjectId <= 0 || apiServiceId <= 0)
            {
                throw new ArgumentException("SideProjectId and apiServiceId must be greater than zero.");
            }

            ApiService apiService = null;

            string sql = "SELECT a.id, a.name, a.description, a.website_id, a.logo_id " +
                         "FROM apis_and_services a " +
                         "JOIN sideproject_apis_and_services pas ON a.id = pas.apiservice_id " +
                         "WHERE pas.sideproject_id = @sideProjectId AND a.id = @apiServiceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                apiService = MapRowToAPIsAndServices(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the API or Service by SideProjectId and APIServiceId.", ex);
            }

            return apiService;
        }

        public ApiService UpdateAPIOrServiceBySideProjectId(int sideProjectId, int apiServiceId, ApiService apiService)
        {
            if (sideProjectId <= 0 || apiServiceId <= 0)
            {
                throw new ArgumentException("SideProjectId and apiServiceId must be greater than zero.");
            }

            string sql = "UPDATE apis_and_services " +
                         "SET name = @name, description = @description " +
                         "FROM sideproject_apis_and_services " +
                         "WHERE apis_and_services.id = sideproject_apis_and_services.apiservice_id " +
                         "AND sideproject_apis_and_services.sideproject_id = @sideProjectId " +
                         "AND apis_and_services.id = @apiServiceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                        cmd.Parameters.AddWithValue("@name", apiService.Name);
                        cmd.Parameters.AddWithValue("@description", apiService.Description);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return apiService;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the API or Service by side project ID.", ex);
            }

            return null;
        }

        public int DeleteAPIOrServiceBySideProjectId(int sideProjectId, int apiServiceId)
        {
            if (sideProjectId <= 0 || apiServiceId <= 0)
            {
                throw new ArgumentException("SideProjectId and apiServiceId must be greater than zero.");
            }

            string deleteAPIServiceFromSideProjectSql = "DELETE FROM sideproject_apis_and_services WHERE sideproject_id = @sideProjectId AND apiservice_id = @apiServiceId;";
            string deleteAPIServiceSql = "DELETE FROM apis_and_services WHERE id = @apiServiceId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteAPIServiceFromSideProjectSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteAPIServiceSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the API or Service by SideProjectId.", ex);
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
                                      APIS AND SERVICES MAP ROW
            **********************************************************************************************
        */
        private ApiService MapRowToAPIsAndServices(NpgsqlDataReader reader)
        {
            ApiService apiService = new ApiService
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Description = Convert.ToString(reader["description"])
            };

            int apiServiceId = apiService.Id;

            SetApiServiceWebsiteIdProperties(reader, apiService, apiServiceId);
            SetApiServiceLogoIdProperties(reader, apiService, apiServiceId);

            return apiService;
        }

        private void SetApiServiceWebsiteIdProperties(NpgsqlDataReader reader, ApiService apiService, int apiServiceId)
        {
            if (reader["website_id"] != DBNull.Value)
            {
                apiService.WebsiteId = Convert.ToInt32(reader["website_id"]);

                apiService.Website = _websiteDao.GetWebsiteByApiServiceId(apiServiceId);
            }
            else
            {
                apiService.WebsiteId = 0;
            }
        }

        private void SetApiServiceLogoIdProperties(NpgsqlDataReader reader, ApiService apiService, int apiServiceId)
        {
            if (reader["logo_id"] != DBNull.Value)
            {
                apiService.LogoId = Convert.ToInt32(reader["logo_id"]);

                apiService.Logo = _imageDao.GetImageByApiServiceId(apiServiceId);
            }
            else
            {
                apiService.LogoId = 0;
            }
        }
    }
}
