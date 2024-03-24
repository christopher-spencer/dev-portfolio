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

        const string MainWebsite = "main-website";
        const string SecondaryWebsite = "secondary-website";
        const string GitHub = "github";
        const string PortfolioLink = "portfolio-link";
        const string LinkedIn = "linkedin";

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
                                            PORTFOLIO WEBSITE CRUD
            **********************************************************************************************
        */
        // TODO Portfolio Website PGDAO****

        /*  
            **********************************************************************************************
                                            SIDE PROJECT WEBSITE CRUD
            **********************************************************************************************
        */

        public Website CreateWebsiteBySideProjectId(int sideProjectId, Website website)
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

            switch (website.Type)
            {
                case MainWebsite:
                    updateSideProjectWebsiteSql = "UPDATE sideprojects SET website_id = @websiteId WHERE id = @sideProjectId;";
                    break;
                case GitHub:
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

        // TODO UPDATE need Website Type? Change only once when setting type in CREATE? Must DELETE and CREATE new by WebsiteType? Consider...
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

        public int DeleteWebsiteBySideProjectId(int sideProjectId, int websiteId)
        {
            if (sideProjectId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("SideProjectId and websiteId must be greater than zero.");
            }

            Website website = GetWebsiteBySideProjectId(sideProjectId, websiteId);

            string updateSideProjectWebsiteIdSql;

            switch (website.Type)
            {
                case MainWebsite:
                    updateSideProjectWebsiteIdSql = "UPDATE sideprojects SET website_id = NULL WHERE website_id = @websiteId;";
                    break;
                case GitHub:
                    updateSideProjectWebsiteIdSql = "UPDATE sideprojects SET github_repo_link_id = NULL WHERE github_repo_link_id = @websiteId;";
                    break;
                default:
                    throw new ArgumentException("Invalid website type.");
            }

            string deleteWebsiteFromSideProjectSql = "DELETE FROM sideproject_websites WHERE sideproject_id = @sideProjectId AND website_id = @websiteId;";
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

                            // Get the image ID associated with the website
                            int? imageId = _imageDao.GetImageIdByWebsiteId(websiteId);

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

                            //Delete Website Image, if exists
                            if (imageId.HasValue)
                            {
                                _imageDao.DeleteImageByWebsiteId(websiteId, imageId.Value);
                            }

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

            if (string.IsNullOrEmpty(website.Type))
            {
                throw new ArgumentException("Website Type cannot be null or empty.");
            }

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertContributorWebsiteSql = "INSERT INTO contributor_websites (contributor_id, website_id) VALUES (@contributorId, @websiteId);";

            string updateContributorWebsiteIdSql;

            switch (website.Type)
            {
                case PortfolioLink:
                    updateContributorWebsiteIdSql = "UPDATE contributors SET portfolio_id = @websiteId WHERE id = @contributorId;";
                    break;
                case GitHub:
                    updateContributorWebsiteIdSql = "UPDATE contributors SET github_id = @websiteId WHERE id = @contributorId;";
                    break;
                case LinkedIn:
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
        // TODO UPDATE need Website Type? Change only once when setting type in CREATE? Must DELETE and CREATE new by WebsiteType? Consider...
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

        public int DeleteWebsiteByContributorId(int contributorId, int websiteId)
        {
            if (contributorId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ContributorId and websiteId must be greater than zero.");
            }

            Website website = GetWebsiteByContributorId(contributorId, websiteId);

            string updateContributorWebsiteIdSql;

            switch (website.Type)
            {
                case LinkedIn:
                    updateContributorWebsiteIdSql = "UPDATE contributors SET linkedin_id = NULL WHERE linkedin_id = @websiteId;";
                    break;
                case GitHub:
                    updateContributorWebsiteIdSql = "UPDATE contributors SET github_id = NULL WHERE github_id = @websiteId;";
                    break;
                case PortfolioLink:
                    updateContributorWebsiteIdSql = "UPDATE contributors SET portfolio_id = NULL WHERE portfolio_id = @websiteId;";
                    break;
                default:
                    throw new ArgumentException("Invalid website type.");
            }

            string deleteContributorWebsiteSql = "DELETE FROM contributor_websites WHERE contributor_id = @contributorId AND website_id = @websiteId;";
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

                            // Get the image ID associated with the website
                            int? imageId = _imageDao.GetImageIdByWebsiteId(websiteId);

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

                            //Delete Website Image, if exists
                            if (imageId.HasValue)
                            {
                                _imageDao.DeleteImageByWebsiteId(websiteId, imageId.Value);
                            }

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

                            // Get the image ID associated with the website
                            int? imageId = _imageDao.GetImageIdByWebsiteId(websiteId);

                            // Update api_service websiteId reference to null
                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateApiServiceWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            // Delete api_service_websites association
                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteApiServiceWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            //Delete Website Image, if exists
                            if (imageId.HasValue)
                            {
                                _imageDao.DeleteImageByWebsiteId(websiteId, imageId.Value);
                            }

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

                            // Get the image ID associated with the website
                            int? imageId = _imageDao.GetImageIdByWebsiteId(websiteId);

                            // Update dependency/library websiteId reference to null
                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateDependencyLibraryWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            // Delete dependency_library_websites table association
                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteDependencyLibraryWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            //Delete Website Image, if exists
                            if (imageId.HasValue)
                            {
                                _imageDao.DeleteImageByWebsiteId(websiteId, imageId.Value);
                            }

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
                                            EXPERIENCE WEBSITE CRUD
            **********************************************************************************************
        */
        public Website CreateWebsiteByExperienceId(int experienceId, Website website)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("Experience Id must be greater than zero.");
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
            string insertExperienceWebsiteSql = "INSERT INTO experience_websites (experience_id, website_id) VALUES (@experienceId, @websiteId);";
            string updateExperienceWebsiteIdSql = "UPDATE experiences SET company_website_id = @websiteId WHERE id = @experienceId;";

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

                            using (NpgsqlCommand cmdInsertExperienceWebsite = new NpgsqlCommand(insertExperienceWebsiteSql, connection))
                            {
                                cmdInsertExperienceWebsite.Parameters.AddWithValue("@experienceId", experienceId);
                                cmdInsertExperienceWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertExperienceWebsite.Transaction = transaction;
                                cmdInsertExperienceWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateExperience = new NpgsqlCommand(updateExperienceWebsiteIdSql, connection))
                            {
                                cmdUpdateExperience.Parameters.AddWithValue("@experienceId", experienceId);
                                cmdUpdateExperience.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdUpdateExperience.Transaction = transaction;
                                cmdUpdateExperience.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            website.Id = websiteId;

                            return website;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the website by experience ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Website GetWebsiteByExperienceId(int experienceId)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("ExperienceId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN experience_websites ew ON w.id = ew.website_id " +
                         "WHERE ew.experience_id = @experienceId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);

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
                throw new DaoException("An error occurred while retrieving the website by Experience ID.", ex);
            }

            return website;
        }

        public Website UpdateWebsiteByExperienceId(int experienceId, int websiteId, Website website)
        {
            if (experienceId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ExperienceId and websiteId must be greater than zero.");
            }

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url " +
                                      "FROM experience_websites " +
                                      "WHERE websites.id = experience_websites.website_id AND experience_websites.experience_id = @experienceId " +
                                      "AND websites.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);
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
                throw new DaoException("An error occurred while updating the website by Experience ID.", ex);
            }

            return null;
        }

        public int DeleteWebsiteByExperienceId(int experienceId, int websiteId)
        {
            if (experienceId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ExperienceId and websiteId must be greater than zero.");
            }

            string updateExperienceWebsiteIdSql = "UPDATE experiences SET company_website_id = NULL WHERE company_website_id = @websiteId;";
            string deleteExperienceWebsiteSql = "DELETE FROM experience_websites WHERE experience_id = @experienceId AND website_id = @websiteId;";
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

                            int? imageId = _imageDao.GetImageIdByWebsiteId(websiteId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateExperienceWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteExperienceWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@experienceId", experienceId);
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            if (imageId.HasValue)
                            {
                                _imageDao.DeleteImageByWebsiteId(websiteId, imageId.Value);
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

                            throw new DaoException("An error occurred while deleting the website by Experience ID.", ex);
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
                                            CREDENTIAL WEBSITE CRUD
            **********************************************************************************************
        */
        // TODO WEBSITE Credential PGDAO****

        public Website CreateWebsiteByCredentialId(int credentialId, Website website)
        {
            if (credentialId <= 0)
            {
                throw new ArgumentException("CredentialId must be greater than zero.");
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
            string insertCredentialWebsiteSql = "INSERT INTO credential_websites (credential_id, website_id) VALUES (@credentialId, @websiteId);";
            string updateCredentialWebsiteSql = "UPDATE credentials SET credential_website_id = @websiteId WHERE id = @credentialId;";



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

                            using (NpgsqlCommand cmdInsertCredentialWebsite = new NpgsqlCommand(insertCredentialWebsiteSql, connection))
                            {
                                cmdInsertCredentialWebsite.Parameters.AddWithValue("@credentialId", credentialId);
                                cmdInsertCredentialWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertCredentialWebsite.Transaction = transaction;
                                cmdInsertCredentialWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateCredentialWebsite = new NpgsqlCommand(updateCredentialWebsiteSql, connection))
                            {
                                cmdUpdateCredentialWebsite.Parameters.AddWithValue("@credentialId", credentialId);
                                cmdUpdateCredentialWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdUpdateCredentialWebsite.Transaction = transaction;
                                cmdUpdateCredentialWebsite.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            website.Id = websiteId;

                            return website;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the website for the credential.", ex);
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
                                            EDUCATION WEBSITE CRUD
            **********************************************************************************************
        */
        // TODO WEBSITE Education PGDAO****
        public Website CreateWebsiteByEducationId(int educationId, Website website)
        {
            if (educationId <= 0)
            {
                throw new ArgumentException("Education Id must be greater than zero.");
            }

            if (string.IsNullOrEmpty(website.Name))
            {
                throw new ArgumentException("Website name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(website.Url))
            {
                throw new ArgumentException("Website URL cannot be null or empty.");
            }
            // TODO take out Null TYPE checks on all that don't need TYPE*****
            if (string.IsNullOrEmpty(website.Type))
            {
                throw new ArgumentException("Website Type cannot be null or empty.");
            }

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertEducationWebsiteSql = "INSERT INTO education_websites (education_id, website_id) VALUES (@educationId, @websiteId);";
            string updateEducationWebsiteIdSql = "UPDATE educations SET institution_website_id = @websiteId WHERE id = @educationId;";

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

                            using (NpgsqlCommand cmdInsertEducationWebsite = new NpgsqlCommand(insertEducationWebsiteSql, connection))
                            {
                                cmdInsertEducationWebsite.Parameters.AddWithValue("@educationId", educationId);
                                cmdInsertEducationWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertEducationWebsite.Transaction = transaction;
                                cmdInsertEducationWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateEducation = new NpgsqlCommand(updateEducationWebsiteIdSql, connection))
                            {
                                cmdUpdateEducation.Parameters.AddWithValue("@educationId", educationId);
                                cmdUpdateEducation.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdUpdateEducation.Transaction = transaction;
                                cmdUpdateEducation.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            website.Id = websiteId;

                            return website;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the website by education ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Website GetWebsiteByEducationId(int educationId)
        {
            if (educationId <= 0)
            {
                throw new ArgumentException("EducationId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN education_websites ew ON w.id = ew.website_id " +
                         "WHERE ew.education_id = @educationId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);

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
                throw new DaoException("An error occurred while retrieving the website by Education ID.", ex);
            }

            return website;
        }

        public Website UpdateWebsiteByEducationId(int educationId, int websiteId, Website website)
        {
            if (educationId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("EducationId and websiteId must be greater than zero.");
            }

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url " +
                                      "FROM education_websites " +
                                      "WHERE websites.id = education_websites.website_id AND education_websites.education_id = @educationId " +
                                      "AND websites.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);
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
                throw new DaoException("An error occurred while updating the website by Education ID.", ex);
            }

            return null;
        }

        public int DeleteWebsiteByEducationId(int educationId, int websiteId)
        {
            if (educationId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("EducationId and websiteId must be greater than zero.");
            }

            string updateEducationWebsiteIdSql = "UPDATE educations SET institution_website_id = NULL WHERE institution_website_id = @websiteId;";
            string deleteEducationWebsiteSql = "DELETE FROM education_websites WHERE education_id = @educationId AND website_id = @websiteId;";
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

                            int? imageId = _imageDao.GetImageIdByWebsiteId(websiteId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateEducationWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteEducationWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@educationId", educationId);
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            if (imageId.HasValue)
                            {
                                _imageDao.DeleteImageByWebsiteId(websiteId, imageId.Value);
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

                            throw new DaoException("An error occurred while deleting the website by Education ID.", ex);
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
                                        OPEN SOURCE CONTRIBUTION WEBSITE CRUD
            **********************************************************************************************
        */
        // TODO WEBSITE OpenSourceContribution PGDAO****

        /*  
            **********************************************************************************************
                                            VOLUNTEER WORK WEBSITE CRUD
            **********************************************************************************************
        */
        public Website CreateWebsiteByVolunteerWorkId(int volunteerWorkId, Website website)
        {
            if (volunteerWorkId <= 0)
            {
                throw new ArgumentException("Volunteer Work Id must be greater than zero.");
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
            string insertVolunteerWorkWebsiteSql = "INSERT INTO volunteer_work_websites (volunteer_work_id, website_id) VALUES (@volunteerWorkId, @websiteId);";
            string updateVolunteerWorkWebsiteIdSql = "UPDATE volunteer_works SET organization_website_id = @websiteId WHERE id = @volunteerWorkId;";

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

                            using (NpgsqlCommand cmdInsertVolunteerWorkWebsite = new NpgsqlCommand(insertVolunteerWorkWebsiteSql, connection))
                            {
                                cmdInsertVolunteerWorkWebsite.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
                                cmdInsertVolunteerWorkWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertVolunteerWorkWebsite.Transaction = transaction;
                                cmdInsertVolunteerWorkWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateVolunteerWork = new NpgsqlCommand(updateVolunteerWorkWebsiteIdSql, connection))
                            {
                                cmdUpdateVolunteerWork.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
                                cmdUpdateVolunteerWork.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdUpdateVolunteerWork.Transaction = transaction;
                                cmdUpdateVolunteerWork.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            website.Id = websiteId;

                            return website;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the website for the volunteer work.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Website GetWebsiteByVolunteerWorkId(int volunteerWorkId)
        {
            if (volunteerWorkId <= 0)
            {
                throw new ArgumentException("Volunteer Work Id must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN volunteer_work_websites vw ON w.id = vw.website_id " +
                         "WHERE vw.volunteer_work_id = @volunteerWorkId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);

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
                throw new DaoException("An error occurred while retrieving the website by Volunteer Work ID.", ex);
            }

            return website;
        }

        public Website UpdateWebsiteByVolunteerWorkId(int volunteerWorkId, int websiteId, Website website)
        {
            if (volunteerWorkId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("Volunteer Work Id and websiteId must be greater than zero.");
            }

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url " +
                                      "FROM volunteer_work_websites " +
                                      "WHERE websites.id = volunteer_work_websites.website_id AND volunteer_work_websites.volunteer_work_id = @volunteerWorkId " +
                                      "AND websites.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
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
                throw new DaoException("An error occurred while updating the website by Volunteer Work ID.", ex);
            }

            return null;
        }

        public int DeleteWebsiteByVolunteerWorkId(int volunteerWorkId, int websiteId)
        {
            if (volunteerWorkId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("Volunteer Work Id and websiteId must be greater than zero.");
            }

            string updateVolunteerWorkWebsiteIdSql = "UPDATE volunteer_works SET organization_website_id = NULL WHERE organization_website_id = @websiteId;";
            string deleteVolunteerWorkWebsiteSql = "DELETE FROM volunteer_work_websites WHERE volunteer_work_id = @volunteerWorkId AND website_id = @websiteId;";
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

                            int? imageId = _imageDao.GetImageIdByWebsiteId(websiteId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateVolunteerWorkWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteVolunteerWorkWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            if (imageId.HasValue)
                            {
                                _imageDao.DeleteImageByWebsiteId(websiteId, imageId.Value);
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

                            throw new DaoException("An error occurred while deleting the website by Volunteer Work ID.", ex);
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
