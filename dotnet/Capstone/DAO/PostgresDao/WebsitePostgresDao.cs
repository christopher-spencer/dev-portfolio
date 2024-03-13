using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class WebsitePostgresDao : IWebsiteDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;

        public WebsitePostgresDao(string dbConnectionString, IImageDao imageDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
        }
        // TODO add Blog Post Website CRUD eventually after links set up in BP Model
        /*  
            **********************************************************************************************
                                                    WEBSITE CRUD
            **********************************************************************************************
        */

        public Website CreateWebsite(Website website)
        {
            if (string.IsNullOrEmpty(website.Name))
            {
                throw new ArgumentException("Website name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(website.Url))
            {
                throw new ArgumentException("Website URL cannot be null or empty.");
            }

            string sql = "INSERT INTO websites (name, url) VALUES (@name, @url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", website.Name);
                        cmd.Parameters.AddWithValue("@url", website.Url);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        website.Id = id;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the website link.", ex);
            }

            return website;
        }

        public Website GetWebsiteById(int websiteId)
        {
            if (websiteId <= 0)
            {
                throw new ArgumentException("WebsiteId must be greater than zero.");
            }

            string sql = "SELECT w.id, w.name, w.url, i.name AS logo_name, i.url AS logo_url " +
                         "FROM websites w " +
                         "JOIN images i ON w.logo_id = i.id " +
                         "WHERE w.id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", websiteId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website link.", ex);
            }

            return null;
        }

        public List<Website> GetAllWebsites()
        {
            List<Website> websiteLinks = new List<Website>();

            string sql = "SELECT w.id, w.name, w.url, i.name AS logo_name, i.url AS logo_url " +
                         "FROM websites w " +
                         "JOIN images i ON w.logo_id = i.id;";

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
                                websiteLinks.Add(MapRowToWebsite(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website links.", ex);
            }

            return websiteLinks;
        }

        public Website UpdateWebsite(Website website, int websiteId)
        {
            if (string.IsNullOrEmpty(website.Name))
            {
                throw new ArgumentException("Website name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(website.Url))
            {
                throw new ArgumentException("Website URL cannot be null or empty.");
            }

            if (websiteId <= 0)
            {
                throw new ArgumentException("WebsiteId must be greater than zero.");
            }

            string sql = "UPDATE websites " +
                         "SET name = @name, url = @url " +
                         "WHERE id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@name", website.Name);
                        cmd.Parameters.AddWithValue("@url", website.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return website;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the website.", ex);
            }

            return null;
        }

        public int DeleteWebsite(int websiteId)
        {
            if (websiteId <= 0)
            {
                throw new ArgumentException("WebsiteId must be greater than zero.");
            }

            string sql = "DELETE FROM websites WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", websiteId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the website link.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                            SIDE PROJECT WEBSITE CRUD
            **********************************************************************************************
        */
        public Website CreateWebsiteByProjectId(int projectId, Website website)
        {
            if (projectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(website.Name))
            {
                throw new ArgumentException("Website name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(website.Url))
            {
                throw new ArgumentException("Website URL cannot be null or empty.");
            }

            string insertWebsiteSql = "INSERT INTO websites (name, url) VALUES (@name, @url) RETURNING id;";
            string insertSideProjectWebsiteSql = "INSERT INTO sideproject_websites (sideproject_id, website_id) VALUES (@projectId, @websiteId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int websiteId;

                            using (NpgsqlCommand cmdInsertWebsite = new NpgsqlCommand(insertWebsiteSql, connection))
                            {
                                cmdInsertWebsite.Parameters.AddWithValue("@name", website.Name);
                                cmdInsertWebsite.Parameters.AddWithValue("@url", website.Url);

                                websiteId = Convert.ToInt32(cmdInsertWebsite.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertSideProjectWebsite = new NpgsqlCommand(insertSideProjectWebsiteSql, connection))
                            {
                                cmdInsertSideProjectWebsite.Parameters.AddWithValue("@projectId", projectId);
                                cmdInsertSideProjectWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertSideProjectWebsite.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            website.Id = websiteId;

                            return website;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the website.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Website GetWebsiteByProjectId(int projectId)
        {
            if (projectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, i.name AS logo_name, i.url AS logo_url " +
                         "FROM websites w " +
                         "JOIN sideproject_websites sw ON w.id = sw.website_id " +
                         "JOIN images i ON w.logo_id = i.id " +
                         "WHERE sw.sideproject_id = @projectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                website = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website by project ID.", ex);
            }

            return website;
        }

        public Website GetWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId)
        {
            if (projectId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ProjectId and websiteId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, i.name AS logo_name, i.url AS logo_url " +
                         "FROM websites w " +
                         "JOIN images i ON w.logo_id = i.id " +
                         "JOIN sideproject_websites sw ON w.id = sw.website_id " +
                         "WHERE sw.sideproject_id = @projectId AND w.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                website = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website by project ID and website ID.", ex);
            }

            return website;
        }

        public Website UpdateWebsiteByProjectId(int projectId, int websiteId, Website website)
        {
            if (projectId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ProjectId and websiteId must be greater than zero.");
            }

            string sql = "UPDATE websites " +
                         "SET name = @name, url = @url " +
                         "FROM sideproject_websites " +
                         "WHERE websites.id = sideproject_websites.website_id " +
                         "AND sideproject_websites.sideproject_id = @projectId " +
                         "AND websites.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@name", website.Name);
                        cmd.Parameters.AddWithValue("@url", website.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return website;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the website.", ex);
            }

            return null;
        }

        public int DeleteWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId)
        {
            if (projectId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ProjectId and websiteId must be greater than zero.");
            }

            string sql = "DELETE FROM sideproject_websites WHERE sideproject_id = @projectId AND website_id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the website from the side project.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                            CONTRIBUTOR WEBSITE CRUD
            **********************************************************************************************
        */

        public Website CreateWebsiteByContributorId(int contributorId, Website website)
        {
            if (contributorId <= 0)
            {
                throw new ArgumentException("Contributor Id must be greater than zero.");
            }

            if (string.IsNullOrEmpty(website.Name))
            {
                throw new ArgumentException("Website name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(website.Url))
            {
                throw new ArgumentException("Website URL cannot be null or empty.");
            }

            string insertWebsiteSql = "INSERT INTO websites (name, url) VALUES (@name, @url) RETURNING id;";
            string insertContributorWebsiteSql = "INSERT INTO contributor_websites (contributor_id, website_id) VALUES (@contributorId, @websiteId);";
            string updateContributorWebsiteIdSql = "UPDATE contributors SET website_id = @websiteId WHERE id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int websiteId;

                            using (NpgsqlCommand cmdInsertWebsite = new NpgsqlCommand(insertWebsiteSql, connection))
                            {
                                cmdInsertWebsite.Parameters.AddWithValue("@name", website.Name);
                                cmdInsertWebsite.Parameters.AddWithValue("@url", website.Url);

                                websiteId = Convert.ToInt32(cmdInsertWebsite.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertContributorWebsite = new NpgsqlCommand(insertContributorWebsiteSql, connection))
                            {
                                cmdInsertContributorWebsite.Parameters.AddWithValue("@contributorId", contributorId);
                                cmdInsertContributorWebsite.Parameters.AddWithValue("@websiteId", websiteId);

                                cmdInsertContributorWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateContributor = new NpgsqlCommand(updateContributorWebsiteIdSql, connection))
                            {
                                cmdUpdateContributor.Parameters.AddWithValue("@contributorId", contributorId);
                                cmdUpdateContributor.Parameters.AddWithValue("@websiteId", websiteId);

                                cmdUpdateContributor.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            website.Id = websiteId;

                            return website;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the website for the contributor.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Website GetWebsiteByContributorId(int contributorId)
        {
            if (contributorId <= 0)
            {
                throw new ArgumentException("ContributorId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url " +
                         "FROM websites w " +
                         "JOIN contributor_websites cw ON w.id = cw.website_id " +
                         "WHERE cw.contributor_id = @contributorId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                website = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website by contributor ID.", ex);
            }

            return website;
        }

        public Website UpdateWebsiteByContributorId(int contributorId, int websiteId, Website website)
        {
            if (contributorId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ContributorId and websiteId must be greater than zero.");
            }

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url " +
                                      "FROM contributor_websites " +
                                      "WHERE websites.id = contributor_websites.website_id AND contributor_websites.contributor_id = @contributorId " +
                                      "AND websites.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@name", website.Name);
                        cmd.Parameters.AddWithValue("@url", website.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return website;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the website by contributor ID.", ex);
            }

            return null;
        }

        public int DeleteWebsiteByContributorId(int contributorId, int websiteId)
        {
            if (contributorId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ContributorId and websiteId must be greater than zero.");
            }

            string deleteContributorWebsiteSql = "DELETE FROM contributor_websites WHERE contributor_id = @contributorId AND website_id = @websiteId;";
            string deleteWebsiteSql = "DELETE FROM websites WHERE id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteContributorWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);

                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the website by contributor ID.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                         API AND SERVICE WEBSITE CRUD
            **********************************************************************************************
        */

        public Website CreateWebsiteByApiServiceId(int apiServiceId, Website website)
        {
            if (apiServiceId <= 0)
            {
                throw new ArgumentException("API Service Id must be greater than zero.");
            }

            if (string.IsNullOrEmpty(website.Name))
            {
                throw new ArgumentException("Website name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(website.Url))
            {
                throw new ArgumentException("Website URL cannot be null or empty.");
            }

            string insertWebsiteSql = "INSERT INTO websites (name, url) VALUES (@name, @url) RETURNING id;";
            string insertApiServiceWebsiteSql = "INSERT INTO api_service_websites (apiservice_id, website_id) VALUES (@apiServiceId, @websiteId);";
            string updateApiServiceWebsiteIdSql = "UPDATE apis_and_services SET website_id = @websiteId WHERE id = @apiServiceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int websiteId;

                            using (NpgsqlCommand cmdInsertWebsite = new NpgsqlCommand(insertWebsiteSql, connection))
                            {
                                cmdInsertWebsite.Parameters.AddWithValue("@name", website.Name);
                                cmdInsertWebsite.Parameters.AddWithValue("@url", website.Url);

                                websiteId = Convert.ToInt32(cmdInsertWebsite.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertApiServiceWebsite = new NpgsqlCommand(insertApiServiceWebsiteSql, connection))
                            {
                                cmdInsertApiServiceWebsite.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                                cmdInsertApiServiceWebsite.Parameters.AddWithValue("@websiteId", websiteId);

                                cmdInsertApiServiceWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateApiService = new NpgsqlCommand(updateApiServiceWebsiteIdSql, connection))
                            {
                                cmdUpdateApiService.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                                cmdUpdateApiService.Parameters.AddWithValue("@websiteId", websiteId);

                                cmdUpdateApiService.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            website.Id = websiteId;

                            return website;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the website for the API/Service.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Website GetWebsiteByApiServiceId(int apiServiceId)
        {
            if (apiServiceId <= 0)
            {
                throw new ArgumentException("ApiServiceId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url " +
                         "FROM websites w " +
                         "JOIN api_service_websites aw ON w.id = aw.website_id " +
                         "WHERE aw.apiservice_id = @apiServiceId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                website = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website by API/Service ID.", ex);
            }

            return website;
        }

        public Website UpdateWebsiteByApiServiceId(int apiServiceId, int websiteId, Website website)
        {
            if (apiServiceId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ApiServiceId and websiteId must be greater than zero.");
            }

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url " +
                                      "FROM api_service_websites " +
                                      "WHERE websites.id = api_service_websites.website_id AND api_service_websites.apiservice_id = @apiServiceId " +
                                      "AND websites.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@name", website.Name);
                        cmd.Parameters.AddWithValue("@url", website.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return website;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the website by API/Service ID.", ex);
            }

            return null;
        }

        public int DeleteWebsiteByApiServiceId(int apiServiceId, int websiteId)
        {
            if (apiServiceId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ApiServiceId and websiteId must be greater than zero.");
            }

            string deleteApiServiceWebsiteSql = "DELETE FROM api_service_websites WHERE apiservice_id = @apiServiceId AND website_id = @websiteId;";
            string deleteWebsiteSql = "DELETE FROM websites WHERE id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteApiServiceWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);

                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the website by API/Service ID.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                        DEPENDENCY AND LIBRARY WEBSITE CRUD
            **********************************************************************************************
        */

        public Website CreateWebsiteByDependencyLibraryId(int dependencyLibraryId, Website website)
        {
            if (dependencyLibraryId <= 0)
            {
                throw new ArgumentException("Dependency Library Id must be greater than zero.");
            }

            if (string.IsNullOrEmpty(website.Name))
            {
                throw new ArgumentException("Website name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(website.Url))
            {
                throw new ArgumentException("Website URL cannot be null or empty.");
            }

            string insertWebsiteSql = "INSERT INTO websites (name, url) VALUES (@name, @url) RETURNING id;";
            string insertDependencyLibraryWebsiteSql = "INSERT INTO dependency_library_websites (dependencylibrary_id, website_id) VALUES (@dependencyLibraryId, @websiteId);";
            string updateDependencyLibraryWebsiteIdSql = "UPDATE dependencies_and_libraries SET website_id = @websiteId WHERE id = @dependencyLibraryId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int websiteId;

                            using (NpgsqlCommand cmdInsertWebsite = new NpgsqlCommand(insertWebsiteSql, connection))
                            {
                                cmdInsertWebsite.Parameters.AddWithValue("@name", website.Name);
                                cmdInsertWebsite.Parameters.AddWithValue("@url", website.Url);

                                websiteId = Convert.ToInt32(cmdInsertWebsite.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertDependencyLibraryWebsite = new NpgsqlCommand(insertDependencyLibraryWebsiteSql, connection))
                            {
                                cmdInsertDependencyLibraryWebsite.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                                cmdInsertDependencyLibraryWebsite.Parameters.AddWithValue("@websiteId", website.Id);

                                cmdInsertDependencyLibraryWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateDependencyLibrary = new NpgsqlCommand(updateDependencyLibraryWebsiteIdSql, connection))
                            {
                                cmdUpdateDependencyLibrary.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                                cmdUpdateDependencyLibrary.Parameters.AddWithValue("@websiteId", website.Id);

                                cmdUpdateDependencyLibrary.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            website.Id = websiteId;

                            return website;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the website for the dependency/library.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Website GetWebsiteByDependencyLibraryId(int dependencyLibraryId)
        {
            if (dependencyLibraryId <= 0)
            {
                throw new ArgumentException("DependencyLibraryId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url " +
                         "FROM websites w " +
                         "JOIN dependency_library_websites dw ON w.id = dw.website_id " +
                         "WHERE dw.dependencylibrary_id = @dependencyLibraryId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                website = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website by dependency/library ID.", ex);
            }

            return website;
        }

        public Website UpdateWebsiteByDependencyLibraryId(int dependencyLibraryId, int websiteId, Website website)
        {
            if (dependencyLibraryId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("DependencyLibraryId and websiteId must be greater than zero.");
            }

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url " +
                                      "FROM dependency_library_websites " +
                                      "WHERE websites.id = dependency_library_websites.website_id " +
                                      "AND dependency_library_websites.dependencylibrary_id = @dependencyLibraryId " +
                                      "AND websites.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@name", website.Name);
                        cmd.Parameters.AddWithValue("@url", website.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return website;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the website by dependency/library ID.", ex);
            }

            return null;
        }

        public int DeleteWebsiteByDependencyLibraryId(int dependencyLibraryId, int websiteId)
        {
            if (dependencyLibraryId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("DependencyLibraryId and websiteId must be greater than zero.");
            }

            string deleteDependencyLibraryWebsiteSql = "DELETE FROM dependency_library_websites WHERE dependencylibrary_id = @dependencyLibraryId AND website_id = @websiteId;";
            string deleteWebsiteSql = "DELETE FROM websites WHERE id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteDependencyLibraryWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);

                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the website by dependency/library ID.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                                WEBSITE MAP ROW
            **********************************************************************************************
        */

        private Website MapRowToWebsite(NpgsqlDataReader reader)
        {
            Website website = new Website
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Url = Convert.ToString(reader["url"])
            };

            SetWebsiteLogoIdProperties(reader, website);

            return website;
        }

        private void SetWebsiteLogoIdProperties(NpgsqlDataReader reader, Website website)
        {
            if (reader["logo_id"] != DBNull.Value)
            {
                website.LogoId = Convert.ToInt32(reader["logo_id"]);

                int logoId = Convert.ToInt32(reader["logo_id"]);
                website.Logo = _imageDao.GetImageById(logoId);
            }
            else
            {
                website.LogoId = 0;
            }
        }
    }
}
