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

        /*  
            **********************************************************************************************
                                        DEPENDENCIES AND LIBRARIES CRUD
            **********************************************************************************************
        */
        public DependencyLibrary CreateDependencyOrLibrary(DependencyLibrary dependencyLibrary)
        {
            if (string.IsNullOrEmpty(dependencyLibrary.Name))
            {
                throw new ArgumentException("Dependency/Library name cannot be null or empty.");
            }

            string sql = "INSERT INTO dependencies_and_libraries (name, description) " +
                "VALUES (@name, @description) " +
                "RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                        cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        dependencyLibrary.Id = id;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the dependency/library.", ex);
            }

            return dependencyLibrary;
        }

        public DependencyLibrary GetDependencyOrLibraryById(int dependencyLibraryId)
        {
            if (dependencyLibraryId <= 0)
            {
                throw new ArgumentException("DependencyLibraryId must be greater than zero.");
            }

            string sql = "SELECT id, name, description, website_id, logo_id FROM dependencies_and_libraries WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", dependencyLibraryId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToDependencyLibrary(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the dependency/library.", ex);
            }

            return null;
        }

        public List<DependencyLibrary> GetDependenciesAndLibraries()
        {
            List<DependencyLibrary> dependencyLibraries = new List<DependencyLibrary>();

            string sql = "SELECT id, name, description, website_id, logo_id FROM dependencies_and_libraries;";

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
                                dependencyLibraries.Add(MapRowToDependencyLibrary(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the dependencies and libraries.", ex);
            }

            return dependencyLibraries;
        }

        public DependencyLibrary UpdateDependencyOrLibrary(int dependencyLibraryId, DependencyLibrary dependencyLibrary)
        {
            if (string.IsNullOrEmpty(dependencyLibrary.Name))
            {
                throw new ArgumentException("Dependency/Library name cannot be null or empty.");
            }

            if (dependencyLibraryId <= 0)
            {
                throw new ArgumentException("DependencyLibraryId must be greater than zero.");
            }

            string sql = "UPDATE dependencies_and_libraries SET name = @name, description = @description " +
                         "WHERE id = @dependencyLibraryId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                        cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                        cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return dependencyLibrary;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the dependency/library.", ex);
            }

            return null;
        }

        public int DeleteDependencyOrLibrary(int dependencyLibraryId)
        {
            if (dependencyLibraryId <= 0)
            {
                throw new ArgumentException("DependencyLibraryId must be greater than zero.");
            }

            string sql = "DELETE FROM dependencies_and_libraries WHERE id = @dependencyLibraryId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the dependency/library.", ex);
            }
        }


        /*  
            **********************************************************************************************
                                    SIDE PROJECT DEPENDENCIES AND LIBRARIES CRUD
            **********************************************************************************************
        */
        public DependencyLibrary CreateDependencyOrLibraryBySideProjectId(int sideProjectId, DependencyLibrary dependencyLibrary)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(dependencyLibrary.Name))
            {
                throw new ArgumentException("Dependency/library name cannot be null or empty.");
            }

            string sql = "INSERT INTO dependencies_and_libraries (name, description) " +
                         "VALUES (@name, @description) RETURNING id;";

            string insertAssociationSql = "INSERT INTO sideproject_dependencies_and_libraries (sideproject_id, dependencylibrary_id) " +
                                                "VALUES (@sideProjectId, @dependencyLibraryId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int dependencyLibraryId;

                            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                            {
                                cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                                cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);

                                dependencyLibraryId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertAssociation = new NpgsqlCommand(insertAssociationSql, connection))
                            {
                                cmdInsertAssociation.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmdInsertAssociation.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                                cmdInsertAssociation.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            dependencyLibrary.Id = dependencyLibraryId;

                            return dependencyLibrary;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the dependency/library for the side project.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<DependencyLibrary> GetDependenciesAndLibrariesBySideProjectId(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            List<DependencyLibrary> dependencyLibraries = new List<DependencyLibrary>();

            string sql = "SELECT dl.id, dl.name, dl.description, dl.website_id, dl.logo_id " +
                         "FROM dependencies_and_libraries dl " +
                         "JOIN sideproject_dependencies_and_libraries pdl ON dl.id = pdl.dependencylibrary_id " +
                         "WHERE pdl.sideproject_id = @sideProjectId;";

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
                                dependencyLibraries.Add(MapRowToDependencyLibrary(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving dependencies and libraries by side project ID.", ex);
            }

            return dependencyLibraries;
        }

        public DependencyLibrary GetDependencyOrLibraryBySideProjectId(int sideProjectId, int dependencyLibraryId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            DependencyLibrary dependencyLibrary = null;

            string sql = "SELECT dl.id, dl.name, dl.description, dl.website_id, dl.logo_id " +
                         "FROM dependencies_and_libraries dl " +
                         "JOIN sideproject_dependencies_and_libraries pdl ON dl.id = pdl.dependencylibrary_id " +
                         "WHERE pdl.sideproject_id = @sideProjectId AND dl.id = @dependencyLibraryId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {                    
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dependencyLibrary = MapRowToDependencyLibrary(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the dependency/library by side project ID.", ex);
            }

            return dependencyLibrary;
        }

        public DependencyLibrary UpdateDependencyOrLibraryBySideProjectId(int sideProjectId, int dependencyLibraryId, DependencyLibrary dependencyLibrary)
        {
            if (sideProjectId <= 0 || dependencyLibraryId <= 0)
            {
                throw new ArgumentException("SideProjectId and dependencyLibraryId must be greater than zero.");
            }

            string sql = "UPDATE dependencies_and_libraries dl " +
                         "SET name = @name, description = @description " +
                         "FROM sideproject_dependencies_and_libraries pdl " +
                         "WHERE dl.id = pdl.dependencylibrary_id AND pdl.sideproject_id = @sideProjectId AND dl.id = @dependencyLibraryId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {                    
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                        cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                        cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return dependencyLibrary;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the dependency/library for the side project.", ex);
            }

            return null;
        }

        public int DeleteDependencyOrLibraryBySideProjectId(int sideProjectId, int dependencyLibraryId)
        {
            if (sideProjectId <= 0 || dependencyLibraryId <= 0)
            {
                throw new ArgumentException("SideProjectId and dependencyLibraryId must be greater than zero.");
            }

            string sql = "DELETE FROM sideproject_dependencies_and_libraries WHERE sideproject_id = @sideProjectId AND dependencylibrary_id = @dependencyLibraryId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {                    
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected;                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the dependency/library from the side project.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                    DEPENDENCIES AND LIBRARIES MAP ROW
            **********************************************************************************************
        */
        private DependencyLibrary MapRowToDependencyLibrary(NpgsqlDataReader reader)
        {
            DependencyLibrary dependencyLibrary = new DependencyLibrary
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Description = Convert.ToString(reader["description"])
            };

            SetDependencyLibraryWebsiteIdProperties(reader, dependencyLibrary);
            SetDependencyLibraryLogoIdProperties(reader, dependencyLibrary);

            return dependencyLibrary;
        }

        private void SetDependencyLibraryWebsiteIdProperties(NpgsqlDataReader reader, DependencyLibrary dependencyLibrary)
        {
            if (reader["website_id"] != DBNull.Value)
            {
                dependencyLibrary.WebsiteId = Convert.ToInt32(reader["website_id"]);

                int websiteId = Convert.ToInt32(reader["website_id"]);
                dependencyLibrary.Website = _websiteDao.GetWebsite(websiteId);
            }
            else
            {
                dependencyLibrary.WebsiteId = 0;
            }
        }

        private void SetDependencyLibraryLogoIdProperties(NpgsqlDataReader reader, DependencyLibrary dependencyLibrary)
        {
            if (reader["logo_id"] != DBNull.Value)
            {
                dependencyLibrary.LogoId = Convert.ToInt32(reader["logo_id"]);

                int logoId = Convert.ToInt32(reader["logo_id"]);
                dependencyLibrary.Logo = _imageDao.GetImageById(logoId);
            }
            else
            {
                dependencyLibrary.LogoId = 0;
            }
        }
    }
}

