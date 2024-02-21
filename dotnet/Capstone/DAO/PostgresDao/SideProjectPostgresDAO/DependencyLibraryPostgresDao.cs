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

        public DependencyLibraryPostgresDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public DependencyLibrary CreateDependencyLibrary(DependencyLibrary dependencyLibrary)
        {
            string sql = "INSERT INTO dependency_libraries (name, description, url, image_logo_name, image_logo_url) VALUES (@name, @description, @url, @image_logo_name, @image_logo_url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                    cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);
                    cmd.Parameters.AddWithValue("@url", dependencyLibrary.Url.Url);
                    cmd.Parameters.AddWithValue("@image_logo_name", dependencyLibrary.ImageLogoUrl.Name);
                    cmd.Parameters.AddWithValue("@image_logo_url", dependencyLibrary.ImageLogoUrl.Url);

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

            string sql = "SELECT dl.id, dl.name, dl.description, dl.url, dl.image_logo_name, dl.image_logo_url " +
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


        public DependencyLibrary GetDependencyLibraryById(int dependencyLibraryId)
        {
            string sql = "SELECT name, description, url, image_logo_name, image_logo_url FROM dependency_libraries WHERE id = @id;";

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

        public List<DependencyLibrary> GetAllDependencyLibraries()
        {
            List<DependencyLibrary> dependencyLibraries = new List<DependencyLibrary>();
            string sql = "SELECT id, name, description, url, image_logo_name, image_logo_url FROM dependency_libraries;";

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

        public DependencyLibrary UpdateDependencyLibrary(DependencyLibrary dependencyLibrary)
        {
            string sql = "UPDATE dependency_libraries SET name = @name, description = @description, url = @url, image_logo_name = @image_logo_name, image_logo_url = @image_logo_url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", dependencyLibrary.Id);
                    cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                    cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);
                    cmd.Parameters.AddWithValue("@url", dependencyLibrary.Url.Url);
                    cmd.Parameters.AddWithValue("@image_logo_name", dependencyLibrary.ImageLogoUrl.Name);
                    cmd.Parameters.AddWithValue("@image_logo_url", dependencyLibrary.ImageLogoUrl.Url);

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

        public int DeleteDependencyLibraryById(int dependencyLibraryId)
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
            return new DependencyLibrary
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Description = Convert.ToString(reader["description"]),
                Url = new WebsiteLink { Url = Convert.ToString(reader["url"]) },
                ImageLogoUrl = new Image
                {
                    Name = Convert.ToString(reader["image_logo_name"]),
                    Url = Convert.ToString(reader["image_logo_url"])
                }
            };
        }
    }
}
