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

        public ApiServicePostgresDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public ApiService CreateApiService(ApiService apiService)
        {
            string sql = "INSERT INTO api_services (name, description, url, image_logo_name, image_logo_url) VALUES (@name, @description, @url, @image_logo_name, @image_logo_url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", apiService.Name);
                    cmd.Parameters.AddWithValue("@description", apiService.Description);
                    cmd.Parameters.AddWithValue("@url", apiService.Url.Url);
                    cmd.Parameters.AddWithValue("@image_logo_name", apiService.ImageLogoUrl.Name);
                    cmd.Parameters.AddWithValue("@image_logo_url", apiService.ImageLogoUrl.Url);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    apiService.Id = id;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the API service.", ex);
            }

            return apiService;
        }

        public List<ApiService> GetAPIsAndServicesByProjectId(int projectId)
        {
            List<ApiService> apiServices = new List<ApiService>();

            string sql = "SELECT a.id, a.name, a.description, a.url, a.image_logo_name, a.image_logo_url " +
                         "FROM api_services a " +
                         "JOIN side_project_api_services pas ON a.id = pas.api_service_id " +
                         "WHERE pas.project_id = @projectId;";

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
                        apiServices.Add(MapRowToApiService(reader));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving API services by project ID.", ex);
            }

            return apiServices;
        }


        public ApiService GetApiServiceById(int apiServiceId)
        {
            string sql = "SELECT name, description, url, image_logo_name, image_logo_url FROM api_services WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", apiServiceId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return MapRowToApiService(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the API service.", ex);
            }

            return null;
        }

        public List<ApiService> GetAllApiServices()
        {
            List<ApiService> apiServices = new List<ApiService>();
            string sql = "SELECT id, name, description, url, image_logo_name, image_logo_url FROM api_services;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        apiServices.Add(MapRowToApiService(reader));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the API services.", ex);
            }

            return apiServices;
        }

        public ApiService UpdateApiService(ApiService apiService)
        {
            string sql = "UPDATE api_services SET name = @name, description = @description, url = @url, image_logo_name = @image_logo_name, image_logo_url = @image_logo_url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", apiService.Id);
                    cmd.Parameters.AddWithValue("@name", apiService.Name);
                    cmd.Parameters.AddWithValue("@description", apiService.Description);
                    cmd.Parameters.AddWithValue("@url", apiService.Url.Url);
                    cmd.Parameters.AddWithValue("@image_logo_name", apiService.ImageLogoUrl.Name);
                    cmd.Parameters.AddWithValue("@image_logo_url", apiService.ImageLogoUrl.Url);

                    int count = cmd.ExecuteNonQuery();
                    if (count == 1)
                    {
                        return apiService;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the API service.", ex);
            }

            return null;
        }

        public int DeleteApiServiceById(int apiServiceId)
        {
            string sql = "DELETE FROM api_services WHERE id = @id;";

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

        private ApiService MapRowToApiService(NpgsqlDataReader reader)
        {
            return new ApiService
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Description = Convert.ToString(reader["description"]),
                Url = new Website { Url = Convert.ToString(reader["url"]) },
                ImageLogoUrl = new Image
                {
                    Name = Convert.ToString(reader["image_logo_name"]),
                    Url = Convert.ToString(reader["image_logo_url"])
                }
            };
        }
    }
}
