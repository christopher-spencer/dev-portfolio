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

            if (string.IsNullOrEmpty(website.Type))
            {
                throw new ArgumentException("Website Type cannot be null or empty.");
            }

            string sql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", website.Name);
                        cmd.Parameters.AddWithValue("@url", website.Url);
                        cmd.Parameters.AddWithValue("@type", website.Type);

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

        public Website GetWebsite(int websiteId)
        {
            if (websiteId <= 0)
            {
                throw new ArgumentException("WebsiteId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT id, name, url, type, logo_id " +
                         "FROM websites " +
                         "WHERE id = @id;";

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
                                website = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website link.", ex);
            }

            return website;
        }

        public List<Website> GetWebsites()
        {
            List<Website> websites = new List<Website>();

            string sql = "SELECT id, name, url, type, logo_id " +
                         "FROM websites;";

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
                                Website website = MapRowToWebsite(reader);
                                websites.Add(website);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website links.", ex);
            }

            return websites;
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

            if (string.IsNullOrEmpty(website.Type))
            {
                throw new ArgumentException("Website Type cannot be null or empty.");
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

            string sql = "DELETE FROM websites WHERE id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected;
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

        public Website CreateWebsiteBySideProjectId(int sideProjectId, Website website, string websiteType)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(website.Name))
            {
                throw new ArgumentException("Website name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(website.Url))
            {
                throw new ArgumentException("Website URL cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(website.Type))
            {
                throw new ArgumentException("Website Type cannot be null or empty.");
            }

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertSideProjectWebsiteSql = "INSERT INTO sideproject_websites (sideproject_id, website_id) VALUES (@sideProjectId, @websiteId);";

            string updateSideProjectWebsiteSql;

            switch (websiteType)
            {
                case "website":
                    updateSideProjectWebsiteSql = "UPDATE sideprojects SET website_id = @websiteId WHERE id = @sideProjectId;";
                    break;
                case "github":
                    updateSideProjectWebsiteSql = "UPDATE sideprojects SET github_repo_link_id = @websiteId WHERE id = @sideProjectId;";
                    break;
                default:
                    throw new ArgumentException("Invalid website type.");
            }

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int websiteId;

                            using (NpgsqlCommand cmdInsertWebsite = new NpgsqlCommand(insertWebsiteSql, connection))
                            {
                                cmdInsertWebsite.Parameters.AddWithValue("@name", website.Name);
                                cmdInsertWebsite.Parameters.AddWithValue("@url", website.Url);
                                cmdInsertWebsite.Parameters.AddWithValue("@type", website.Type);
                                cmdInsertWebsite.Transaction = transaction;
                                websiteId = Convert.ToInt32(cmdInsertWebsite.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertSideProjectWebsite = new NpgsqlCommand(insertSideProjectWebsiteSql, connection))
                            {
                                cmdInsertSideProjectWebsite.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmdInsertSideProjectWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertSideProjectWebsite.Transaction = transaction;
                                cmdInsertSideProjectWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateSideProjectWebsite = new NpgsqlCommand(updateSideProjectWebsiteSql, connection))
                            {
                                cmdUpdateSideProjectWebsite.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmdUpdateSideProjectWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdUpdateSideProjectWebsite.Transaction = transaction;
                                cmdUpdateSideProjectWebsite.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            website.Id = websiteId;
                            website.Type = websiteType;

                            return website;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the website for the side project.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }
        public Website GetWebsiteBySideProjectId(int sideProjectId, int websiteId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN sideproject_websites sw ON w.id = sw.website_id " +
                         "WHERE sw.sideproject_id = @sideProjectId AND w.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
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
                throw new DaoException("An error occurred while retrieving the website by SideProjectId and websiteId.", ex);
            }

            return website;
        }

        // TODO create path for GetWebsite(s) BySideProjectId (?)
        // TODO consider do we need Website Type here? Do we want it to be able to change at all or once you set 
        // TODO the type for CREATE are we good? ONLY choice to delete it and create new by WebsiteType? Consider...
        public Website UpdateWebsiteBySideProjectId(int sideProjectId, int websiteId, Website website)
        {
            if (sideProjectId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("SideProjectId and websiteId must be greater than zero.");
            }

            string sql = "UPDATE websites " +
                         "SET name = @name, url = @url " +
                         "FROM sideproject_websites " +
                         "WHERE websites.id = sideproject_websites.website_id " +
                         "AND sideproject_websites.sideproject_id = @sideProjectId " +
                         "AND websites.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
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
                throw new DaoException("An error occurred while updating the website by side project id.", ex);
            }

            return null;
        }
        // NOTE fixed foreign key constraint on website_images_website_id foreign key here
        // NOTE NEED TO UPDATE OTHER DELETE METHODS W/ ISSUES LIKE THIS, TO DELETE ASSOCIATED OBJECTS
        // FIXME CHECK ADDITIONAL PLACES WHERE ASSOCIATED OBJECTS SHOULD BE DELETED WHEN OBJECT HOLDING THEM DELETED******
        public int DeleteWebsiteBySideProjectId(int sideProjectId, int websiteId, string websiteType)
        {
            if (sideProjectId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("SideProjectId and websiteId must be greater than zero.");
            }

            string updateSideProjectWebsiteIdSql;

            switch (websiteType)
            {
                case "website":
                    updateSideProjectWebsiteIdSql = "UPDATE sideprojects SET website_id = NULL WHERE website_id = @websiteId;";
                    break;
                case "github":
                    updateSideProjectWebsiteIdSql = "UPDATE sideprojects SET github_repo_link_id = NULL WHERE github_repo_link_id = @websiteId;";
                    break;
                default:
                    throw new ArgumentException("Invalid website type.");
            }

            string deleteWebsiteFromSideProjectSql = "DELETE FROM sideproject_websites WHERE sideproject_id = @sideProjectId AND website_id = @websiteId;";
            string deleteWebsiteImagesSql = "DELETE FROM website_images WHERE website_id = @websiteId;";
            string deleteWebsiteSql = "DELETE FROM websites WHERE id = @websiteId;";

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

                            // Update side project websiteId reference to null
                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateSideProjectWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            // Delete sideproject_websites table association
                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteFromSideProjectSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            // Delete website_images table association
                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteImagesSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            DeleteAssociatedWebsiteImages(connection, transaction, websiteId);

                            // Delete the website itself
                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the website by side project ID.", ex);
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
                                            CONTRIBUTOR WEBSITE CRUD
            **********************************************************************************************
        */

        public Website CreateWebsiteByContributorId(int contributorId, Website website, string websiteType)
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

            if (string.IsNullOrEmpty(website.Type))
            {
                throw new ArgumentException("Website Type cannot be null or empty.");
            }

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertContributorWebsiteSql = "INSERT INTO contributor_websites (contributor_id, website_id) VALUES (@contributorId, @websiteId);";

            string updateContributorWebsiteIdSql;

            switch (websiteType)
            {
                case "portfoliolink":
                    updateContributorWebsiteIdSql = "UPDATE contributors SET portfolio_id = @websiteId WHERE id = @contributorId;";
                    break;
                case "github":
                    updateContributorWebsiteIdSql = "UPDATE contributors SET github_id = @websiteId WHERE id = @contributorId;";
                    break;
                case "linkedin":
                    updateContributorWebsiteIdSql = "UPDATE contributors SET linkedin_id = @websiteId WHERE id = @contributorId;";
                    break;
                default:
                    throw new ArgumentException("Invalid website type.");
            }

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int websiteId;

                            using (NpgsqlCommand cmdInsertWebsite = new NpgsqlCommand(insertWebsiteSql, connection))
                            {
                                cmdInsertWebsite.Parameters.AddWithValue("@name", website.Name);
                                cmdInsertWebsite.Parameters.AddWithValue("@url", website.Url);
                                cmdInsertWebsite.Parameters.AddWithValue("@type", website.Type);
                                cmdInsertWebsite.Transaction = transaction;
                                websiteId = Convert.ToInt32(cmdInsertWebsite.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertContributorWebsite = new NpgsqlCommand(insertContributorWebsiteSql, connection))
                            {
                                cmdInsertContributorWebsite.Parameters.AddWithValue("@contributorId", contributorId);
                                cmdInsertContributorWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertContributorWebsite.Transaction = transaction;
                                cmdInsertContributorWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateContributor = new NpgsqlCommand(updateContributorWebsiteIdSql, connection))
                            {
                                cmdUpdateContributor.Parameters.AddWithValue("@contributorId", contributorId);
                                cmdUpdateContributor.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdUpdateContributor.Transaction = transaction;
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
        // TODO add GetWEBSITESByContributorId (?)
        public Website GetWebsiteByContributorId(int contributorId, int websiteId)
        {
            if (contributorId <= 0)
            {
                throw new ArgumentException("ContributorId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN contributor_websites cw ON w.id = cw.website_id " +
                         "WHERE cw.contributor_id = @contributorId AND w.id = @websiteId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);
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
                throw new DaoException("An error occurred while retrieving the website by contributor ID.", ex);
            }

            return website;
        }
        // TODO consider do we need Website Type here? Do we want it to be able to change at all or once you set the type for CREATE are we good? Consider...
        public Website UpdateWebsiteByContributorId(int contributorId, int websiteId, Website website)
        {
            if (contributorId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ContributorId and websiteId must be greater than zero.");
            }

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url " +
                                      "FROM contributor_websites " +
                                      "WHERE websites.id = contributor_websites.website_id " +
                                      "AND contributor_websites.contributor_id = @contributorId " +
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

        public int DeleteWebsiteByContributorId(int contributorId, int websiteId, string websiteType)
        {
            if (contributorId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ContributorId and websiteId must be greater than zero.");
            }

            string updateContributorWebsiteIdSql;

            switch (websiteType)
            {
                case "linkedin":
                    updateContributorWebsiteIdSql = "UPDATE contributors SET linkedin_id = NULL WHERE linkedin_id = @websiteId;";
                    break;
                case "github":
                    updateContributorWebsiteIdSql = "UPDATE contributors SET github_id = NULL WHERE github_id = @websiteId;";
                    break;
                case "portfoliolink":
                    updateContributorWebsiteIdSql = "UPDATE contributors SET portfolio_id = NULL WHERE portfolio_id = @websiteId;";
                    break;
                default:
                    throw new ArgumentException("Invalid website type.");
            }

            string deleteContributorWebsiteSql = "DELETE FROM contributor_websites WHERE contributor_id = @contributorId AND website_id = @websiteId;";
            string deleteWebsiteImagesSql = "DELETE FROM website_images WHERE website_id = @websiteId;";
            string deleteWebsiteSql = "DELETE FROM websites WHERE id = @websiteId;";

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

                            // Update contributor websiteId reference to null
                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateContributorWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            // Delete contributor_websites table association
                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteContributorWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@contributorId", contributorId);
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            // Delete website_images table association
                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteImagesSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            DeleteAssociatedWebsiteImages(connection, transaction, websiteId);

                            // Delete the website itself
                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the website by contributor ID.", ex);
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

            if (string.IsNullOrEmpty(website.Type))
            {
                throw new ArgumentException("Website Type cannot be null or empty.");
            }

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertApiServiceWebsiteSql = "INSERT INTO api_service_websites (apiservice_id, website_id) VALUES (@apiServiceId, @websiteId);";
            string updateApiServiceWebsiteIdSql = "UPDATE apis_and_services SET website_id = @websiteId WHERE id = @apiServiceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int websiteId;

                            using (NpgsqlCommand cmdInsertWebsite = new NpgsqlCommand(insertWebsiteSql, connection))
                            {
                                cmdInsertWebsite.Parameters.AddWithValue("@name", website.Name);
                                cmdInsertWebsite.Parameters.AddWithValue("@url", website.Url);
                                cmdInsertWebsite.Parameters.AddWithValue("@type", website.Type);
                                cmdInsertWebsite.Transaction = transaction;
                                websiteId = Convert.ToInt32(cmdInsertWebsite.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertApiServiceWebsite = new NpgsqlCommand(insertApiServiceWebsiteSql, connection))
                            {
                                cmdInsertApiServiceWebsite.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                                cmdInsertApiServiceWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertApiServiceWebsite.Transaction = transaction;
                                cmdInsertApiServiceWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateApiService = new NpgsqlCommand(updateApiServiceWebsiteIdSql, connection))
                            {
                                cmdUpdateApiService.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                                cmdUpdateApiService.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdUpdateApiService.Transaction = transaction;
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

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
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

            string updateApiServiceWebsiteIdSql = "UPDATE apis_and_services SET website_id = NULL WHERE website_id = @websiteId;";
            string deleteApiServiceWebsiteSql = "DELETE FROM api_service_websites WHERE apiservice_id = @apiServiceId AND website_id = @websiteId;";
            string deleteWebsiteSql = "DELETE FROM websites WHERE id = @websiteId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateApiServiceWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteApiServiceWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the website by API/Service ID.", ex);
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

            if (string.IsNullOrEmpty(website.Type))
            {
                throw new ArgumentException("Website Type cannot be null or empty.");
            }

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertDependencyLibraryWebsiteSql = "INSERT INTO dependency_library_websites (dependencylibrary_id, website_id) VALUES (@dependencyLibraryId, @websiteId);";
            string updateDependencyLibraryWebsiteIdSql = "UPDATE dependencies_and_libraries SET website_id = @websiteId WHERE id = @dependencyLibraryId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int websiteId;

                            using (NpgsqlCommand cmdInsertWebsite = new NpgsqlCommand(insertWebsiteSql, connection))
                            {
                                cmdInsertWebsite.Parameters.AddWithValue("@name", website.Name);
                                cmdInsertWebsite.Parameters.AddWithValue("@url", website.Url);
                                cmdInsertWebsite.Parameters.AddWithValue("@type", website.Type);
                                cmdInsertWebsite.Transaction = transaction;
                                websiteId = Convert.ToInt32(cmdInsertWebsite.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertDependencyLibraryWebsite = new NpgsqlCommand(insertDependencyLibraryWebsiteSql, connection))
                            {
                                cmdInsertDependencyLibraryWebsite.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                                cmdInsertDependencyLibraryWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertDependencyLibraryWebsite.Transaction = transaction;
                                cmdInsertDependencyLibraryWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateDependencyLibrary = new NpgsqlCommand(updateDependencyLibraryWebsiteIdSql, connection))
                            {
                                cmdUpdateDependencyLibrary.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                                cmdUpdateDependencyLibrary.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdUpdateDependencyLibrary.Transaction = transaction;
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

        public Website GetWebsiteByDependencyLibraryId(int dependencyLibraryId, int websiteId)
        {
            if (dependencyLibraryId <= 0)
            {
                throw new ArgumentException("DependencyLibraryId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN dependency_library_websites dw ON w.id = dw.website_id " +
                         "WHERE dw.dependencylibrary_id = @dependencyLibraryId AND dw.website_id = @websiteId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
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

            string updateDependencyLibraryWebsiteIdSql = "UPDATE dependencies_and_libraries SET website_id = NULL WHERE website_id = @websiteId;";
            string deleteDependencyLibraryWebsiteSql = "DELETE FROM dependency_library_websites WHERE dependencylibrary_id = @dependencyLibraryId AND website_id = @websiteId;";
            string deleteWebsiteSql = "DELETE FROM websites WHERE id = @websiteId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateDependencyLibraryWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteDependencyLibraryWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the website by dependency/library ID.", ex);
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

        private void DeleteAssociatedWebsiteImages(NpgsqlConnection connection, NpgsqlTransaction transaction, int websiteId)
        {
            // Get the image ID associated with the website
            int? imageId = null;

            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT logo_id FROM websites WHERE id = @websiteId;", connection))
            {
                cmd.Parameters.AddWithValue("@websiteId", websiteId);
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    imageId = Convert.ToInt32(result);
                }
            }

            // If imageId exists, remove the reference to the image from the websites table
            if (imageId != null)
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE websites SET logo_id = NULL WHERE logo_id = @imageId;", connection))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@imageId", imageId);

                    cmd.ExecuteNonQuery();
                }
            }

            // If imageId exists, delete the associated image from the images table if it exists
            if (imageId != null)
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM images WHERE id = @imageId;", connection))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@imageId", imageId);

                    cmd.ExecuteNonQuery();
                }
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
                Type = Convert.ToString(reader["type"]),
                Url = Convert.ToString(reader["url"])
            };

            int websiteId = website.Id;

            SetWebsiteLogoIdProperties(reader, website, websiteId);

            return website;
        }

        private void SetWebsiteLogoIdProperties(NpgsqlDataReader reader, Website website, int websiteId)
        {

            //FIXME still initializing all logo ids as null instead of setting to 0
            if (reader["logo_id"] != DBNull.Value)
            {
                website.LogoId = Convert.ToInt32(reader["logo_id"]);
                int imageId = website.LogoId;

                website.Logo = _imageDao.GetImageByWebsiteId(websiteId, imageId);
            }
            else
            {
                website.LogoId = 0;
            }
        }
    }
}
