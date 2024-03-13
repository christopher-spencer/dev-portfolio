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
            string sql = "INSERT INTO dependencies_and_libraries (name, description) " +
                "VALUES (@name, @description) " +
                "RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                    cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);

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

        public DependencyLibrary GetDependencyOrLibraryById(int dependencyLibraryId)
        {
            string sql = "SELECT id, name, description, website_id, logo_id FROM dependencies_and_libraries WHERE id = @id;";

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
            string sql = "SELECT id, name, description, website_id, logo_id FROM dependencies_and_libraries;";

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
        //FIXME add depLibraryId

        public DependencyLibrary UpdateDependencyOrLibrary(DependencyLibrary dependencyLibrary)
        {
            string sql = "UPDATE dependencies_and_libraries SET name = @name, description = @description " +
                         "WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", dependencyLibrary.Id);
                    cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                    cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);

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
            string sql = "DELETE FROM dependencies_and_libraries WHERE id = @id;";

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


        /*  
            **********************************************************************************************
                                    SIDE PROJECT DEPENDENCIES AND LIBRARIES CRUD
            **********************************************************************************************
        */ 
        public DependencyLibrary CreateDependencyOrLibraryBySideProjectId(int projectId, DependencyLibrary dependencyLibrary)
        {
            string sql = "INSERT INTO dependencies_and_libraries (name, description) " +
                         "VALUES (@name, @description) RETURNING id;";
            
            string insertAssociationSql = "INSERT INTO sideproject_dependencies_and_libraries (sideproject_id, dependencylibrary_id) " +
                                                "VALUES (@projectId, @dependencyLibraryId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                    cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    dependencyLibrary.Id = id;

                    NpgsqlCommand cmdInsertAssociation = new NpgsqlCommand(insertAssociationSql, connection);
                    cmdInsertAssociation.Parameters.AddWithValue("@projectId", projectId);
                    cmdInsertAssociation.Parameters.AddWithValue("@dependencyLibraryId", id);
                    cmdInsertAssociation.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the dependency library for the project.", ex);
            }

            return dependencyLibrary;
        }

        public List<DependencyLibrary> GetDependenciesAndLibrariesBySideProjectId(int projectId)
        {
            List<DependencyLibrary> dependencyLibraries = new List<DependencyLibrary>();

            string sql = "SELECT dl.id, dl.name, dl.description, dl.website_id, dl.logo_id " +
                         "FROM dependencies_and_libraries dl " +
                         "JOIN sideproject_dependencies_and_libraries pdl ON dl.id = pdl.dependencylibrary_id " +
                         "WHERE pdl.sideproject_id = @projectId;";

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

        public DependencyLibrary GetDependencyOrLibraryBySideProjectId(int projectId, int dependencyLibraryId)
        {
            DependencyLibrary dependencyLibrary = null;

            string sql = "SELECT dl.id, dl.name, dl.description, dl.website_id, dl.logo_id " +
                         "FROM dependencies_and_libraries dl " +
                         "JOIN sideproject_dependencies_and_libraries pdl ON dl.id = pdl.dependencylibrary_id " +
                         "WHERE pdl.sideproject_id = @projectId AND dl.id = @dependencyLibraryId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        dependencyLibrary = MapRowToDependencyLibrary(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the dependency library by project ID.", ex);
            }

            return dependencyLibrary;
        }
        //FIXME add depLibraryId

        public DependencyLibrary UpdateDependencyOrLibraryBySideProjectId(int projectId, DependencyLibrary dependencyLibrary)
        {
            string sql = "UPDATE dependencies_and_libraries dl " +
                         "SET name = @name, description = @description " +
                         "FROM sideproject_dependencies_and_libraries pdl " +
                         "WHERE dl.id = pdl.dependencylibrary_id AND pdl.sideproject_id = @projectId AND dl.id = @dependencyLibraryId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibrary.Id);
                    cmd.Parameters.AddWithValue("@name", dependencyLibrary.Name);
                    cmd.Parameters.AddWithValue("@description", dependencyLibrary.Description);

                    int count = cmd.ExecuteNonQuery();
                    if (count == 1)
                    {
                        return dependencyLibrary;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the dependency library for the project.", ex);
            }

            return null;
        }

        public int DeleteDependencyOrLibraryBySideProjectId(int projectId, int dependencyLibraryId)
        {
            string sql = "DELETE FROM sideproject_dependencies_and_libraries WHERE sideproject_id = @projectId AND dependencylibrary_id = @dependencyLibraryId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the dependency library from the project.", ex);
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
                dependencyLibrary.Website = _websiteDao.GetWebsiteById(websiteId);
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

