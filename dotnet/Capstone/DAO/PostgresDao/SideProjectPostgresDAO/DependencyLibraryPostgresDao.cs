using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class DependencyLibraryPostgresDao : IDependencyLibraryDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;
        private readonly IWebsiteDao _websiteDao;

        public DependencyLibraryPostgresDao(string dbConnectionString, IImageDao imageDao, IWebsiteDao websiteDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
            this._websiteDao = websiteDao;
        }

        public DependencyLibrary CreateDependencyOrLibrary(DependencyLibrary dependencyLibrary)
        {
            string sql = "INSERT INTO dependency_libraries (name, description, website_url, logo_name, logo_url) " +
                "VALUES (@name, @description, @website_url, @logo_name, @logo_url) " +
                "RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                    cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);
                    cmd.Parameters.AddWithValue("@website_url", dependencyLibrary.Website.Url);
                    cmd.Parameters.AddWithValue("@logo_name", dependencyLibrary.Logo.Name);
                    cmd.Parameters.AddWithValue("@logo_url", dependencyLibrary.Logo.Url);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    dependencyLibrary.Id = id;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the dependency library.", ex);
            }

            return dependencyLibrary;
        }

        public List<DependencyLibrary> GetDependenciesAndLibrariesByProjectId(int projectId)
        {
            List<DependencyLibrary> dependencyLibraries = new List<DependencyLibrary>();

            string sql = "SELECT dl.id, dl.name, dl.description, dl.website_url, dl.logo_name, dl.logo_url " +
                         "FROM dependency_libraries dl " +
                         "JOIN side_project_dependency_libraries pdl ON dl.id = pdl.dependency_library_id " +
                         "WHERE pdl.project_id = @projectId;";

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
                        dependencyLibraries.Add(MapRowToDependencyLibrary(reader));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving dependency libraries by project ID.", ex);
            }

            return dependencyLibraries;
        }


        public DependencyLibrary GetDependencyOrLibraryById(int dependencyLibraryId)
        {
            string sql = "SELECT name, description, website_url, logo_name, logo_url FROM dependency_libraries WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", dependencyLibraryId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return MapRowToDependencyLibrary(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the dependency library.", ex);
            }

            return null;
        }

        public List<DependencyLibrary> GetAllDependenciesAndLibraries()
        {
            List<DependencyLibrary> dependencyLibraries = new List<DependencyLibrary>();
            string sql = "SELECT id, name, description, website_url, logo_name, logo_url FROM dependency_libraries;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        dependencyLibraries.Add(MapRowToDependencyLibrary(reader));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the dependency libraries.", ex);
            }

            return dependencyLibraries;
        }

        public DependencyLibrary UpdateDependencyOrLibrary(DependencyLibrary dependencyLibrary)
        {
            string sql = "UPDATE dependency_libraries SET name = @name, description = @description, website_url = @website_url, " +
                "logo_name = @logo_name, logo_url = @logo_url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", dependencyLibrary.Id);
                    cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                    cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);
                    cmd.Parameters.AddWithValue("@website_url", dependencyLibrary.Website.Url);
                    cmd.Parameters.AddWithValue("@logo_name", dependencyLibrary.Logo.Name);
                    cmd.Parameters.AddWithValue("@logo_url", dependencyLibrary.Logo.Url);

                    int count = cmd.ExecuteNonQuery();
                    if (count == 1)
                    {
                        return dependencyLibrary;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the dependency library.", ex);
            }

            return null;
        }

        public int DeleteDependencyOrLibraryById(int dependencyLibraryId)
        {
            string sql = "DELETE FROM dependency_libraries WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", dependencyLibraryId);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the dependency library.", ex);
            }
        }

        private DependencyLibrary MapRowToDependencyLibrary(NpgsqlDataReader reader)
        {
            DependencyLibrary dependencyLibrary = new DependencyLibrary
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Description = Convert.ToString(reader["description"]),
                WebsiteId = Convert.ToInt32(reader["website_id"]),
                LogoId = Convert.ToInt32(reader["logo_id"])
            };

            if (reader["website_id"] != DBNull.Value)
            {
                int websiteId = Convert.ToInt32(reader["website_id"]);
                dependencyLibrary.Website = _websiteDao.GetWebsiteById(websiteId);
            }

            if (reader["logo_id"] != DBNull.Value)
            {
                int logoId = Convert.ToInt32(reader["logo_id"]);
                dependencyLibrary.Logo = _imageDao.GetImageById(logoId);
            }

            return dependencyLibrary;
        }
    }
}
