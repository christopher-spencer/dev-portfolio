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

                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            apiService = MapRowToAPIsAndServices(reader);
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

        public List<ApiService> GetAllAPIsAndServices()
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
                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            apiServices.Add(MapRowToAPIsAndServices(reader));
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the API services.", ex);
            }

            return apiServices;
        }

        public ApiService UpdateAPIOrService(ApiService apiService)
        {
            string sql = "UPDATE apis_and_services " +
                         "SET name = @name, description = @description " +
                         "WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", apiService.Id);
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
            string sql = "DELETE FROM api_and_services WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", apiServiceId);

                    return cmd.ExecuteNonQuery();
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
        public ApiService CreateAPIOrServiceBySideProjectId(int projectId, ApiService apiService)
        {
            string insertApiServiceSql = "INSERT INTO apis_and_services (name, description) " +
                                         "VALUES (@name, @description) RETURNING id;";
            string insertSideProjectApiServiceSql = "INSERT INTO sideproject_apis_and_services (sideproject_id, apiservice_id) " +
                                                     "VALUES (@projectId, @apiServiceId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmdInsertApiService = new NpgsqlCommand(insertApiServiceSql, connection))
                    {
                        cmdInsertApiService.Parameters.AddWithValue("@name", apiService.Name);
                        cmdInsertApiService.Parameters.AddWithValue("@description", apiService.Description);

                        int apiServiceId = Convert.ToInt32(cmdInsertApiService.ExecuteScalar());

                        using (NpgsqlCommand cmdInsertSideProjectApiService = new NpgsqlCommand(insertSideProjectApiServiceSql, connection))
                        {
                            cmdInsertSideProjectApiService.Parameters.AddWithValue("@projectId", projectId);
                            cmdInsertSideProjectApiService.Parameters.AddWithValue("@apiServiceId", apiServiceId);

                            cmdInsertSideProjectApiService.ExecuteNonQuery();

                            apiService.Id = apiServiceId;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the API service by project ID.", ex);
            }

            return apiService;
        }

        public List<ApiService> GetAPIsAndServicesBySideProjectId(int projectId)
        {
            List<ApiService> apiServices = new List<ApiService>();

            string sql = "SELECT a.id, a.name, a.description, a.website_id, a.logo_id " +
                         "FROM apis_and_services a " +
                         "JOIN sideproject_apis_and_services pas ON a.id = pas.apiservice_id " +
                         "WHERE pas.sideproject_id = @projectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);

                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            apiServices.Add(MapRowToAPIsAndServices(reader));
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving API services by project ID.", ex);
            }

            return apiServices;
        }

        public ApiService GetAPIOrServiceBySideProjectId(int projectId, int apiServiceId)
        {
            ApiService apiService = null;

            string sql = "SELECT a.id, a.name, a.description, a.website_id, a.logo_id " +
                         "FROM apis_and_services a " +
                         "JOIN sideproject_apis_and_services pas ON a.id = pas.apiservice_id " +
                         "WHERE pas.sideproject_id = @projectId AND a.id = @apiServiceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);
                        cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);

                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            apiService = MapRowToAPIsAndServices(reader);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the API service by project ID and API service ID.", ex);
            }

            return apiService;
        }

        public ApiService UpdateAPIOrServiceBySideProjectId(int projectId, ApiService updatedApiService)
        {
            string sql = "UPDATE apis_and_services " +
                         "SET name = @name, description = @description " +
                         "FROM sideproject_apis_and_services " +
                         "WHERE apis_and_services.id = sideproject_apis_and_services.apiservice_id " +
                         "AND sideproject_apis_and_services.sideproject_id = @projectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);
                        cmd.Parameters.AddWithValue("@name", updatedApiService.Name);
                        cmd.Parameters.AddWithValue("@description", updatedApiService.Description);

                        int count = cmd.ExecuteNonQuery();
                        if (count == 1)
                        {
                            return updatedApiService;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the API service by project ID.", ex);
            }

            return null;
        }
        public int DeleteAPIOrServiceBySideProjectId(int projectId, int apiServiceId)
        {
            string sql = "DELETE FROM sideproject_apis_and_services " +
                        "WHERE sideproject_id = @projectId AND apiservice_id = @apiServiceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);
                        cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the API service by project ID.", ex);
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

            SetApiServiceWebsiteIdProperties(reader, apiService);
            SetApiServiceLogoIdProperties(reader, apiService);

            return apiService;
        }
// TODO ApiService Image CRUD in ImagePostgresDao   
// TODO ApiService Website CRUD in WebsitePostgresDao
        private void SetApiServiceWebsiteIdProperties(NpgsqlDataReader reader, ApiService apiService)
        {
            if (reader["website_id"] != DBNull.Value)
            {
                apiService.WebsiteId = Convert.ToInt32(reader["website_id"]);

                int websiteId = Convert.ToInt32(reader["website_id"]);
                apiService.Website = _websiteDao.GetWebsiteById(websiteId);
            }
            else
            {
                apiService.WebsiteId = 0;
            }
        }

        private void SetApiServiceLogoIdProperties(NpgsqlDataReader reader, ApiService apiService)
        {
            apiService.LogoId = Convert.ToInt32(reader["logo_id"]);

            if (reader["logo_id"] != DBNull.Value)
            {
                int logoId = Convert.ToInt32(reader["logo_id"]);
                apiService.Logo = _imageDao.GetImageById(logoId);
            }
            else
            {
                apiService.LogoId = 0;
            }
        }
    }
}
