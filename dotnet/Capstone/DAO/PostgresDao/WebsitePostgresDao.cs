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

        const string MainWebsite = "main website";
        const string SecondaryWebsite = "secondary website";
        const string GitHub = "github";
        const string PortfolioLink = "portfolio link";
        const string LinkedIn = "linkedin";
        const string PullRequestLink = "pull request link";

// NOTE: (TEST IF FIXED) issue with websites where if you update to new one, the old one hangs out in the database (NOT DELETED) unattached but attached by the same id, causing foreign key constraints in join tables


// NOTE: (TEST IF FIXED) Do UPDATE methods throughout need Website Type? OR take back off? ********

        /*  
            **********************************************************************************************
                                                    WEBSITE CRUD
            **********************************************************************************************
        */


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

        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                            PORTFOLIO WEBSITE CRUD
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */

        public Website CreateWebsiteByPortfolioId(int portfolioId, Website website)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            bool isWebsiteTypeRequired = true;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertPortfolioWebsiteSql = "INSERT INTO portfolio_websites (portfolio_id, website_id) VALUES (@portfolioId, @websiteId);";
            
            string updatePortfolioWebsiteIdSql;

            switch (website.Type)
            {
                case GitHub:
                    updatePortfolioWebsiteIdSql = "UPDATE portfolios SET github_repo_link_id = @websiteId WHERE id = @portfolioId;";
                    break;
                case LinkedIn:
                    updatePortfolioWebsiteIdSql = "UPDATE portfolios SET linkedin_id = @websiteId WHERE id = @portfolioId;";
                    break;
                default:
                    throw new ArgumentException("Invalid website type.");
            }

            if (website.Type == GitHub)
            {
                Website existingGitHub = GetGitHubByPortfolioId(portfolioId);

                if (existingGitHub != null)
                {
                    throw new ArgumentException("A GitHub website already exists for this portfolio.");
                }
            }
            else if (website.Type == LinkedIn)
            {
                Website existingLinkedIn = GetLinkedInByPortfolioId(portfolioId);

                if (existingLinkedIn != null)
                {
                    throw new ArgumentException("A LinkedIn website already exists for this portfolio.");
                }
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

                            using (NpgsqlCommand cmdInsertPortfolioWebsite = new NpgsqlCommand(insertPortfolioWebsiteSql, connection))
                            {
                                cmdInsertPortfolioWebsite.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmdInsertPortfolioWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertPortfolioWebsite.Transaction = transaction;
                                cmdInsertPortfolioWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdatePortfolio = new NpgsqlCommand(updatePortfolioWebsiteIdSql, connection))
                            {
                                cmdUpdatePortfolio.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmdUpdatePortfolio.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdUpdatePortfolio.Transaction = transaction;
                                cmdUpdatePortfolio.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            website.Id = websiteId;

                            return website;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the website by portfolio ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Website GetWebsiteByPortfolioId(int portfolioId, int websiteId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN portfolio_websites pw ON w.id = pw.website_id " +
                         "WHERE pw.portfolio_id = @portfolioId AND w.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
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
                throw new DaoException("An error occurred while retrieving the website by portfolio ID.", ex);
            }

            return website;
        }

        public Website GetGitHubByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN portfolio_websites pw ON w.id = pw.website_id " +
                         "WHERE pw.portfolio_id = @portfolioId AND w.type = @type;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@type", GitHub);

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
                throw new DaoException("An error occurred while retrieving the GitHub website by portfolio ID.", ex);
            }

            return website;
        }

        public Website GetLinkedInByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN portfolio_websites pw ON w.id = pw.website_id " +
                         "WHERE pw.portfolio_id = @portfolioId AND w.type = @type;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@type", LinkedIn);

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
                throw new DaoException("An error occurred while retrieving the LinkedIn website by portfolio ID.", ex);
            }

            return website;
        }

        public Website UpdateWebsiteByPortfolioId(int portfolioId, int websiteId, Website website)
        {
            if (portfolioId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("PortfolioId and websiteId must be greater than zero.");
            }

            bool isWebsiteTypeRequired = true;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            if (website.Type != GitHub && website.Type != LinkedIn)
            {
                throw new ArgumentException("Invalid website type. Website type must be 'github' or 'linkedin'.");
            }

            string sql = "UPDATE websites " +
                         "SET name = @name, url = @url, type = @type " +
                         "FROM portfolio_websites " +
                         "WHERE websites.id = portfolio_websites.website_id " +
                         "AND portfolio_websites.portfolio_id = @portfolioId " +
                         "AND websites.id = @websiteId;";

            if (website.Type == GitHub)
            {
                Website existingGitHub = GetGitHubByPortfolioId(portfolioId);

                if (existingGitHub != null && existingGitHub.Id != websiteId)
                {
                    throw new ArgumentException("A GitHub website already exists for this portfolio. Delete the GitHub to replace it, or you can set this website as a 'linkedin' site.");
                }
            }
            else if (website.Type == LinkedIn)
            {
                Website existingLinkedIn = GetLinkedInByPortfolioId(portfolioId);

                if (existingLinkedIn != null && existingLinkedIn.Id != websiteId)
                {
                    throw new ArgumentException("A LinkedIn website already exists for this portfolio. Delete the LinkedIn to replace it, or you can set this website as a 'github' site.");
                }
            }

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@name", website.Name);
                        cmd.Parameters.AddWithValue("@url", website.Url);
                        cmd.Parameters.AddWithValue("@type", website.Type);

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
                throw new DaoException("An error occurred while updating the website by portfolio ID.", ex);
            }

            return null;
        }

        public int DeleteWebsiteByPortfolioId(int portfolioId, int websiteId)
        {
            if (portfolioId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("PortfolioId and websiteId must be greater than zero.");
            }

            Website website = GetWebsiteByPortfolioId(portfolioId, websiteId);

            string updatePortfolioWebsiteIdSql;

            switch (website.Type)
            {
                case GitHub:
                    updatePortfolioWebsiteIdSql = "UPDATE portfolios SET github_repo_link_id = NULL WHERE github_repo_link_id = @websiteId;";
                    break;
                case LinkedIn:
                    updatePortfolioWebsiteIdSql = "UPDATE portfolios SET linkedin_id = NULL WHERE linkedin_id = @websiteId;";
                    break;
                default:
                    throw new ArgumentException("Invalid website type.");
            }

            string deleteWebsiteFromPortfolioSql = "DELETE FROM portfolio_websites WHERE portfolio_id = @portfolioId AND website_id = @websiteId;";
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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updatePortfolioWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteFromPortfolioSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
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

                            throw new DaoException("An error occurred while deleting the website by portfolio ID.", ex);
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
                                           WORK EXPERIENCE WEBSITE CRUD
            **********************************************************************************************
        */

        public Website CreateWebsiteByWorkExperienceId(int experienceId, Website website)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("Experience Id must be greater than zero.");
            }

            bool isWebsiteTypeRequired = false;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertExperienceWebsiteSql = "INSERT INTO work_experience_websites (experience_id, website_id) VALUES (@experienceId, @websiteId);";
            string updateExperienceWebsiteIdSql = "UPDATE work_experiences SET company_website_id = @websiteId WHERE id = @experienceId;";

            Website existingWebsite = GetWebsiteByWorkExperienceId(experienceId);

            if (existingWebsite != null)
            {
                throw new ArgumentException("A website already exists for this work experience.");
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
                                cmdInsertWebsite.Parameters.AddWithValue("@type", website.Type ?? (object)DBNull.Value);
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

                            throw new DaoException("An error occurred while creating the website by work experience ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Website GetWebsiteByWorkExperienceId(int experienceId)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("ExperienceId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN work_experience_websites wew ON w.id = wew.website_id " +
                         "WHERE wew.experience_id = @experienceId";

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
                throw new DaoException("An error occurred while retrieving the website by Work Experience ID.", ex);
            }

            return website;
        }

        public Website UpdateWebsiteByWorkExperienceId(int experienceId, int websiteId, Website website)
        {
            if (experienceId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ExperienceId and websiteId must be greater than zero.");
            }

            bool isWebsiteTypeRequired = false;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url, type = @type " +
                                      "FROM work_experience_websites " +
                                      "WHERE websites.id = work_experience_websites.website_id " +
                                      "AND work_experience_websites.experience_id = @experienceId " +
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
                        cmd.Parameters.AddWithValue("@type", website.Type ?? (object)DBNull.Value);

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
                throw new DaoException("An error occurred while updating the website by Work Experience ID.", ex);
            }

            return null;
        }
        
        public int DeleteWebsiteByWorkExperienceId(int experienceId, int websiteId)
        {
            if (experienceId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ExperienceId and websiteId must be greater than zero.");
            }

            string updateExperienceWebsiteIdSql = "UPDATE work_experiences SET company_website_id = NULL WHERE company_website_id = @websiteId;";
            string deleteExperienceWebsiteSql = "DELETE FROM work_experience_websites WHERE experience_id = @experienceId AND website_id = @websiteId;";
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

                            throw new DaoException("An error occurred while deleting the website by Work Experience ID.", ex);
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

        public Website CreateWebsiteByCredentialId(int credentialId, Website website)
        {
            if (credentialId <= 0)
            {
                throw new ArgumentException("CredentialId must be greater than zero.");
            }

            bool isWebsiteTypeRequired = true;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertCredentialWebsiteSql = "INSERT INTO credential_websites (credential_id, website_id) VALUES (@credentialId, @websiteId);";

            string updateOrganizationOrCredentialWebsiteSql;

            switch (website.Type)
            {
                case MainWebsite:
                    updateOrganizationOrCredentialWebsiteSql = "UPDATE credentials SET organization_website_id = @websiteId WHERE id = @credentialId;";
                    break;
                case SecondaryWebsite:
                    updateOrganizationOrCredentialWebsiteSql = "UPDATE credentials SET credential_website_id = @websiteId WHERE id = @credentialId;";
                    break;
                default:
                    throw new ArgumentException("Invalid website type.");
            }

            if (website.Type == MainWebsite)
            {
                Website existingOrganizationWebsite = GetOrganizationWebsiteByCredentialId(credentialId);

                if (existingOrganizationWebsite != null)
                {
                    throw new ArgumentException("An organization website already exists for this credential.");
                }
            }
            else if (website.Type == SecondaryWebsite)
            {
                Website existingCredentialWebsite = GetCredentialWebsiteByCredentialId(credentialId);

                if (existingCredentialWebsite != null)
                {
                    throw new ArgumentException("A credential website already exists for this credential.");
                }
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

                            using (NpgsqlCommand cmdInsertCredentialWebsite = new NpgsqlCommand(insertCredentialWebsiteSql, connection))
                            {
                                cmdInsertCredentialWebsite.Parameters.AddWithValue("@credentialId", credentialId);
                                cmdInsertCredentialWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertCredentialWebsite.Transaction = transaction;
                                cmdInsertCredentialWebsite.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateCredentialWebsite = new NpgsqlCommand(updateOrganizationOrCredentialWebsiteSql, connection))
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

                            throw new DaoException("An error occurred while creating the website by credential ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Website GetWebsiteByCredentialId(int credentialId, int websiteId)
        {
            if (credentialId <= 0)
            {
                throw new ArgumentException("CredentialId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN credential_websites cw ON w.id = cw.website_id " +
                         "WHERE cw.credential_id = @credentialId AND w.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);
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
                throw new DaoException("An error occurred while retrieving the website by Credential ID and website ID.", ex);
            }

            return website;
        }

        public Website GetOrganizationWebsiteByCredentialId(int credentialId)
        {
            if (credentialId <= 0)
            {
                throw new ArgumentException("CredentialId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN credential_websites cw ON w.id = cw.website_id " +
                         "WHERE cw.credential_id = @credentialId AND w.type = @type;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);
                        cmd.Parameters.AddWithValue("@type", MainWebsite);

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
                throw new DaoException("An error occurred while retrieving the organization website by Credential ID.", ex);
            }

            return website;
        }

        public Website GetCredentialWebsiteByCredentialId(int credentialId)
        {
            if (credentialId <= 0)
            {
                throw new ArgumentException("CredentialId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN credential_websites cw ON w.id = cw.website_id " +
                         "WHERE cw.credential_id = @credentialId AND w.type = @type;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);
                        cmd.Parameters.AddWithValue("@type", SecondaryWebsite);

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
                throw new DaoException("An error occurred while retrieving the credential website by Credential ID.", ex);
            }

            return website;
        }

        public Website UpdateWebsiteByCredentialId(int credentialId, int websiteId, Website website)
        {
            if (credentialId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("CredentialId and websiteId must be greater than zero.");
            }

            bool isWebsiteTypeRequired = true;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            if (website.Type != MainWebsite && website.Type != SecondaryWebsite)
            {
                throw new ArgumentException("Invalid website type. Website type must be 'main website' or 'secondary website'.");
            }

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url, type = @type " +
                                      "FROM credential_websites " +
                                      "WHERE websites.id = credential_websites.website_id " +
                                      "AND credential_websites.credential_id = @credentialId " +
                                      "AND websites.id = @websiteId;";
            
            if (website.Type == MainWebsite)
            {
                Website existingOrganizationWebsite = GetOrganizationWebsiteByCredentialId(credentialId);

                if (existingOrganizationWebsite != null && existingOrganizationWebsite.Id != websiteId)
                {
                    throw new ArgumentException("An organization website already exists for this credential. Delete the organization website to replace it, or you can set this website as a credential website using 'secondary website' type.");
                }
            }
            else if (website.Type == SecondaryWebsite)
            {
                Website existingCredentialWebsite = GetCredentialWebsiteByCredentialId(credentialId);

                if (existingCredentialWebsite != null && existingCredentialWebsite.Id != websiteId)
                {
                    throw new ArgumentException("A credential website already exists for this credential. Delete the credential website to replace it, or you can set this website as an organization website using 'main website' type.");
                }
            }

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@name", website.Name);
                        cmd.Parameters.AddWithValue("@url", website.Url);
                        cmd.Parameters.AddWithValue("@type", website.Type);

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
                throw new DaoException("An error occurred while updating the website by Credential ID.", ex);
            }

            return null;
        }

        public int DeleteWebsiteByCredentialId(int credentialId, int websiteId)
        {
            if (credentialId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("CredentialId and websiteId must be greater than zero.");
            }

            Website website = GetWebsiteByCredentialId(credentialId, websiteId);

            string updateCredentialWebsiteIdSql;

            switch (website.Type)
            {
                case MainWebsite:
                    updateCredentialWebsiteIdSql = "UPDATE credentials SET organization_website_id = NULL WHERE organization_website_id = @websiteId;";
                    break;
                case SecondaryWebsite:
                    updateCredentialWebsiteIdSql = "UPDATE credentials SET credential_website_id = NULL WHERE credential_website_id = @websiteId;";
                    break;
                default:
                    throw new ArgumentException("Invalid website type.");
            }

            string deleteCredentialWebsiteSql = "DELETE FROM credential_websites WHERE credential_id = @credentialId AND website_id = @websiteId;";
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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateCredentialWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteCredentialWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@credentialId", credentialId);
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

                            throw new DaoException("An error occurred while deleting the website by Credential ID.", ex);
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

        public Website CreateWebsiteByEducationId(int educationId, Website website)
        {
            if (educationId <= 0)
            {
                throw new ArgumentException("Education Id must be greater than zero.");
            }

            bool isWebsiteTypeRequired = false;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertEducationWebsiteSql = "INSERT INTO education_websites (education_id, website_id) VALUES (@educationId, @websiteId);";
            string updateEducationWebsiteIdSql = "UPDATE educations SET institution_website_id = @websiteId WHERE id = @educationId;";

            Website existingWebsite = GetWebsiteByEducationId(educationId);

            if (existingWebsite != null)
            {
                throw new ArgumentException("A website already exists for this education.");
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
                                cmdInsertWebsite.Parameters.AddWithValue("@type", website.Type ?? (object)DBNull.Value);
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

            bool isWebsiteTypeRequired = false;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url, type = @type " +
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
                        cmd.Parameters.AddWithValue("@type", website.Type ?? (object)DBNull.Value);

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

        public Website CreateWebsiteByOpenSourceContributionId(int contributionId, Website website)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("Contribution Id must be greater than zero.");
            }

            bool isWebsiteTypeRequired = true;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertContributionWebsiteSql = "INSERT INTO open_source_contribution_websites (contribution_id, website_id) VALUES (@contributionId, @websiteId);";

            string updateContributionWebsiteIdSql = null;

            switch (website.Type)
            {
                case MainWebsite:
                    updateContributionWebsiteIdSql = "UPDATE open_source_contributions SET organization_website_id = @websiteId WHERE id = @contributionId;";
                    break;
                case GitHub:
                    updateContributionWebsiteIdSql = "UPDATE open_source_contributions SET organization_github_id = @websiteId WHERE id = @contributionId;";
                    break;
                case PullRequestLink:
                    break;
                default:
                    throw new ArgumentException("Invalid website type. Website type must be 'main website', 'github', or 'pull request link'.");
            }

            if (website.Type == MainWebsite)
            {
                Website existingMainWebsite = GetMainWebsiteOrGitHubByOpenSourceContributionId(contributionId, MainWebsite);

                if (existingMainWebsite != null)
                {
                    throw new DaoException("Main website already exists for this open source contribution.");
                }
            }
            else if (website.Type == GitHub)
            {
                Website existingGitHub = GetMainWebsiteOrGitHubByOpenSourceContributionId(contributionId, GitHub);

                if (existingGitHub != null)
                {
                    throw new DaoException("GitHub already exists for this open source contribution.");
                }
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

                            using (NpgsqlCommand cmdInsertContributionWebsite = new NpgsqlCommand(insertContributionWebsiteSql, connection))
                            {
                                cmdInsertContributionWebsite.Parameters.AddWithValue("@contributionId", contributionId);
                                cmdInsertContributionWebsite.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertContributionWebsite.Transaction = transaction;
                                cmdInsertContributionWebsite.ExecuteNonQuery();
                            }

                            if ( (website.Type == MainWebsite) || (website.Type == GitHub) )
                            {
                                using (NpgsqlCommand cmdUpdateContribution = new NpgsqlCommand(updateContributionWebsiteIdSql, connection))
                                {
                                    cmdUpdateContribution.Parameters.AddWithValue("@contributionId", contributionId);
                                    cmdUpdateContribution.Parameters.AddWithValue("@websiteId", websiteId);
                                    cmdUpdateContribution.Transaction = transaction;
                                    cmdUpdateContribution.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();

                            website.Id = websiteId;

                            return website;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the organization website or github or pull request link by open source contribution ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Website GetWebsiteByOpenSourceContributionId(int contributionId, int websiteId)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("ContributionId must be greater than zero.");
            }

            Website website = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN open_source_contribution_websites ocw ON w.id = ocw.website_id " +
                         "WHERE ocw.contribution_id = @contributionId AND w.id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
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
                throw new DaoException("An error occurred while retrieving the website by Open Source Contribution ID and website ID.", ex);
            }

            return website;
        }

        public Website GetMainWebsiteOrGitHubByOpenSourceContributionId(int contributionId, string websiteType)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("ContributionId must be greater than zero.");
            }

            if (websiteType != MainWebsite && websiteType != GitHub)
            {
                throw new ArgumentException("Website Type must be either 'main website' or 'github'.");
            }

            Website mainWebsiteOrGitHub = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN open_source_contribution_websites ocw ON w.id = ocw.website_id " +
                         "WHERE ocw.contribution_id = @contributionId AND w.type = @websiteType;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
                        cmd.Parameters.AddWithValue("@websiteType", websiteType);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                mainWebsiteOrGitHub = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the main website or GitHub by Open Source Contribution ID.", ex);
            }

            return mainWebsiteOrGitHub;
        }

        public List<Website> GetAllWebsitesByOpenSourceContributionId(int contributionId)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("ContributionId must be greater than zero.");
            }

            List<Website> websites = new List<Website>();

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN open_source_contribution_websites ocw ON w.id = ocw.website_id " +
                         "WHERE ocw.contribution_id = @contributionId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                websites.Add(MapRowToWebsite(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the websites by Open Source Contribution ID.", ex);
            }

            return websites;
        }

        public List<Website> GetPullRequestLinksByOpenSourceContributionId(int contributionId)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("ContributionId must be greater than zero.");
            }

            List<Website> pullRequestLinks = new List<Website>();

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN open_source_contribution_websites ocw ON w.id = ocw.website_id " +
                         "WHERE ocw.contribution_id = @contributionId AND w.type = @type;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
                        cmd.Parameters.AddWithValue("@type", PullRequestLink);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pullRequestLinks.Add(MapRowToWebsite(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the pull request links by Open Source Contribution ID.", ex);
            }

            return pullRequestLinks;
        }

        public Website UpdateWebsiteByOpenSourceContributionId(int contributionId, int websiteId, Website website)
        {
            if (contributionId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ContributionId and websiteId must be greater than zero.");
            }

            bool isWebsiteTypeRequired = true;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            if (website.Type != MainWebsite && website.Type != GitHub && website.Type != PullRequestLink)
            {
                throw new ArgumentException("Invalid website type. Website type must be 'main website', 'github', or 'pull request link'.");
            }

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url, type = @type " +
                                      "FROM open_source_contribution_websites " +
                                      "WHERE websites.id = open_source_contribution_websites.website_id " +
                                      "AND open_source_contribution_websites.contribution_id = @contributionId " +
                                      "AND websites.id = @websiteId;";
            
            if (website.Type == MainWebsite)
            {
                Website existingMainWebsite = GetMainWebsiteOrGitHubByOpenSourceContributionId(contributionId, MainWebsite);

                if (existingMainWebsite != null && existingMainWebsite.Id != websiteId)
                {
                    throw new DaoException("Main website already exists for this open source contribution.");
                }
            }
            else if (website.Type == GitHub)
            {
                Website existingGitHub = GetMainWebsiteOrGitHubByOpenSourceContributionId(contributionId, GitHub);

                if (existingGitHub != null && existingGitHub.Id != websiteId)
                {
                    throw new DaoException("GitHub already exists for this open source contribution.");
                }
            }

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@name", website.Name);
                        cmd.Parameters.AddWithValue("@url", website.Url);
                        cmd.Parameters.AddWithValue("@type", website.Type);

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
                throw new DaoException("An error occurred while updating the website by Open Source Contribution ID.", ex);
            }

            return null;
        }

//FIXME Possibly unnecessary after added exception checks to UpdateWebsiteByOpenSourceContributionId*******
        public Website UpdateMainWebsiteOrGitHubByOpenSourceContributionId(int contributionId, int websiteId, Website website)
        {
            if (contributionId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ContributionId and websiteId must be greater than zero.");
            }

            bool isWebsiteTypeRequired = true;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            if (website.Type != MainWebsite && website.Type != GitHub)
            {
                throw new ArgumentException("Website Type must be either 'main website' or 'github'.");
            }
            else
            {
                DeleteWebsiteByOpenSourceContributionId(contributionId, websiteId);
                CreateWebsiteByOpenSourceContributionId(contributionId, website);
            }

            return website;
        }

        public int DeleteWebsiteByOpenSourceContributionId(int contributionId, int websiteId)
        {
            if (contributionId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ContributionId and websiteId must be greater than zero.");
            }

            Website website = GetWebsiteByOpenSourceContributionId(contributionId, websiteId);

            string updateContributionWebsiteIdSql = null;

            switch (website.Type)
            {
                case MainWebsite:
                    updateContributionWebsiteIdSql = "UPDATE open_source_contributions SET organization_website_id = NULL WHERE organization_website_id = @websiteId;";
                    break;
                case GitHub:
                    updateContributionWebsiteIdSql = "UPDATE open_source_contributions SET organization_github_id = NULL WHERE organization_github_id = @websiteId;";
                    break;
                case PullRequestLink:
                    break;
                default:
                    throw new ArgumentException("Invalid website type.");
            }

            string deleteContributionWebsiteSql = "DELETE FROM open_source_contribution_websites WHERE contribution_id = @contributionId AND website_id = @websiteId;";
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

                            if (website.Type == MainWebsite || website.Type == GitHub)
                            {
                                using (NpgsqlCommand cmd = new NpgsqlCommand(updateContributionWebsiteIdSql, connection))
                                {
                                    cmd.Transaction = transaction;
                                    cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteContributionWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@contributionId", contributionId);
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
                            throw new DaoException("An error occurred while deleting the website by Open Source Contribution ID.");
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
                                            VOLUNTEER WORK WEBSITE CRUD
            **********************************************************************************************
        */

        public Website CreateWebsiteByVolunteerWorkId(int volunteerWorkId, Website website)
        {
            if (volunteerWorkId <= 0)
            {
                throw new ArgumentException("Volunteer Work Id must be greater than zero.");
            }

            bool isWebsiteTypeRequired = false;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertVolunteerWorkWebsiteSql = "INSERT INTO volunteer_work_websites (volunteer_work_id, website_id) VALUES (@volunteerWorkId, @websiteId);";
            string updateVolunteerWorkWebsiteIdSql = "UPDATE volunteer_works SET organization_website_id = @websiteId WHERE id = @volunteerWorkId;";

            Website existingWebsite = GetWebsiteByVolunteerWorkId(volunteerWorkId);

            if (existingWebsite != null)
            {
                throw new ArgumentException("A website already exists for this volunteer work.");
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
                                cmdInsertWebsite.Parameters.AddWithValue("@type", website.Type ?? (object)DBNull.Value);
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

            bool isWebsiteTypeRequired = false;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url, type = @type " +
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
                        cmd.Parameters.AddWithValue("@type", website.Type ?? (object)DBNull.Value);

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
            **********************************************************************************************
            **********************************************************************************************
                                            SIDE PROJECT WEBSITE CRUD
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */

        public Website CreateWebsiteBySideProjectId(int sideProjectId, Website website)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            bool isWebsiteTypeRequired = true;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

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

            if (website.Type == MainWebsite)
            {
                Website existingMainWebsite = GetMainWebsiteBySideProjectId(sideProjectId);

                if (existingMainWebsite != null)
                {
                    throw new DaoException("Main website already exists for this side project.");
                }
            }
            else if (website.Type == GitHub)
            {
                Website existingGitHub = GetGitHubBySideProjectId(sideProjectId);

                if (existingGitHub != null)
                {
                    throw new DaoException("GitHub already exists for this side project.");
                }
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

        public Website GetMainWebsiteBySideProjectId(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            Website mainWebsite = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN sideproject_websites sw ON w.id = sw.website_id " +
                         "WHERE sw.sideproject_id = @sideProjectId AND w.type = @type;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@type", MainWebsite);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                mainWebsite = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the main website by SideProjectId.", ex);
            }

            return mainWebsite;
        }

        public Website GetGitHubBySideProjectId(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            Website gitHub = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN sideproject_websites sw ON w.id = sw.website_id " +
                         "WHERE sw.sideproject_id = @sideProjectId AND w.type = @type;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@type", GitHub);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                gitHub = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the GitHub by SideProjectId.", ex);
            }

            return gitHub;
        }

        public Website UpdateWebsiteBySideProjectId(int sideProjectId, int websiteId, Website website)
        {
            if (sideProjectId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("SideProjectId and websiteId must be greater than zero.");
            }

            bool isWebsiteTypeRequired = true;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            if (website.Type != MainWebsite && website.Type != GitHub)
            {
                throw new ArgumentException("Invalid website type. Website type must be 'main website' or 'github'.");
            }

            string sql = "UPDATE websites " +
                         "SET name = @name, url = @url, type = @type " +
                         "FROM sideproject_websites " +
                         "WHERE websites.id = sideproject_websites.website_id " +
                         "AND sideproject_websites.sideproject_id = @sideProjectId " +
                         "AND websites.id = @websiteId;";

            if (website.Type == MainWebsite)
            {
                Website existingMainWebsite = GetMainWebsiteBySideProjectId(sideProjectId);

                if (existingMainWebsite != null && existingMainWebsite.Id != websiteId)
                {
                    throw new DaoException("Main website already exists for this side project.");
                }
            }
            else if (website.Type == GitHub)
            {
                Website existingGitHub = GetGitHubBySideProjectId(sideProjectId);

                if (existingGitHub != null && existingGitHub.Id != websiteId)
                {
                    throw new DaoException("GitHub already exists for this side project.");
                }
            }

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
                        cmd.Parameters.AddWithValue("@type", website.Type);

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

                            int? imageId = _imageDao.GetImageIdByWebsiteId(websiteId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateSideProjectWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteFromSideProjectSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
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

            bool isWebsiteTypeRequired = true;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

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

            if (website.Type == PortfolioLink)
            {
                Website existingPortfolioLink = GetPortfolioLinkByContributorId(contributorId);

                if (existingPortfolioLink != null)
                {
                    throw new DaoException("Portfolio link already exists for this contributor.");
                }
            }
            else if (website.Type == GitHub)
            {
                Website existingGitHub = GetGitHubByContributorId(contributorId);

                if (existingGitHub != null)
                {
                    throw new DaoException("GitHub already exists for this contributor.");
                }
            }
            else if (website.Type == LinkedIn)
            {
                Website existingLinkedIn = GetLinkedInByContributorId(contributorId);

                if (existingLinkedIn != null)
                {
                    throw new DaoException("LinkedIn already exists for this contributor.");
                }
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

        public Website GetPortfolioLinkByContributorId(int contributorId)
        {
            if (contributorId <= 0)
            {
                throw new ArgumentException("ContributorId must be greater than zero.");
            }

            Website portfolioLink = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN contributor_websites cw ON w.id = cw.website_id " +
                         "WHERE cw.contributor_id = @contributorId AND w.type = @type;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);
                        cmd.Parameters.AddWithValue("@type", PortfolioLink);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                portfolioLink = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the portfolio link by contributor ID.", ex);
            }

            return portfolioLink;
        }

        public Website GetGitHubByContributorId(int contributorId)
        {
            if (contributorId <= 0)
            {
                throw new ArgumentException("ContributorId must be greater than zero.");
            }

            Website gitHub = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN contributor_websites cw ON w.id = cw.website_id " +
                         "WHERE cw.contributor_id = @contributorId AND w.type = @type;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);
                        cmd.Parameters.AddWithValue("@type", GitHub);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                gitHub = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the GitHub by contributor ID.", ex);
            }

            return gitHub;
        }

        public Website GetLinkedInByContributorId(int contributorId)
        {
            if (contributorId <= 0)
            {
                throw new ArgumentException("ContributorId must be greater than zero.");
            }

            Website linkedIn = null;

            string sql = "SELECT w.id, w.name, w.url, w.type, w.logo_id " +
                         "FROM websites w " +
                         "JOIN contributor_websites cw ON w.id = cw.website_id " +
                         "WHERE cw.contributor_id = @contributorId AND w.type = @type;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);
                        cmd.Parameters.AddWithValue("@type", LinkedIn);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                linkedIn = MapRowToWebsite(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the LinkedIn by contributor ID.", ex);
            }

            return linkedIn;
        }

        public Website UpdateWebsiteByContributorId(int contributorId, int websiteId, Website website)
        {
            if (contributorId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ContributorId and websiteId must be greater than zero.");
            }

            bool isWebsiteTypeRequired = true;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            if (website.Type != PortfolioLink && website.Type != GitHub && website.Type != LinkedIn)
            {
                throw new ArgumentException("Invalid website type. Website type must be 'portfolio link', 'github', or 'linkedin'.");
            }

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url, type = @type " +
                                      "FROM contributor_websites " +
                                      "WHERE websites.id = contributor_websites.website_id " +
                                      "AND contributor_websites.contributor_id = @contributorId " +
                                      "AND websites.id = @websiteId;";

            if (website.Type == PortfolioLink)
            {
                Website existingPortfolioLink = GetPortfolioLinkByContributorId(contributorId);

                if (existingPortfolioLink != null && existingPortfolioLink.Id != websiteId)
                {
                    throw new DaoException("Portfolio link already exists for this contributor.");
                }
            }
            else if (website.Type == GitHub)
            {
                Website existingGitHub = GetGitHubByContributorId(contributorId);

                if (existingGitHub != null && existingGitHub.Id != websiteId)
                {
                    throw new DaoException("GitHub already exists for this contributor.");
                }
            }
            else if (website.Type == LinkedIn)
            {
                Website existingLinkedIn = GetLinkedInByContributorId(contributorId);

                if (existingLinkedIn != null && existingLinkedIn.Id != websiteId)
                {
                    throw new DaoException("LinkedIn already exists for this contributor.");
                }
            }

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
                        cmd.Parameters.AddWithValue("@type", website.Type);

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

                            int? imageId = _imageDao.GetImageIdByWebsiteId(websiteId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateContributorWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteContributorWebsiteSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@contributorId", contributorId);
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

            bool isWebsiteTypeRequired = false;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertApiServiceWebsiteSql = "INSERT INTO api_service_websites (apiservice_id, website_id) VALUES (@apiServiceId, @websiteId);";
            string updateApiServiceWebsiteIdSql = "UPDATE apis_and_services SET website_id = @websiteId WHERE id = @apiServiceId;";

            Website existingWebsite = GetWebsiteByApiServiceId(apiServiceId);

            if (existingWebsite != null)
            {
                throw new DaoException("Website already exists for this API/Service.");
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
                                cmdInsertWebsite.Parameters.AddWithValue("@type", website.Type ?? (object)DBNull.Value);
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

            bool isWebsiteTypeRequired = false;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url, type = @type " +
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
                        cmd.Parameters.AddWithValue("@type", website.Type ?? (object)DBNull.Value);

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

                            int? imageId = _imageDao.GetImageIdByWebsiteId(websiteId);

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

            bool isWebsiteTypeRequired = false;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string insertWebsiteSql = "INSERT INTO websites (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertDependencyLibraryWebsiteSql = "INSERT INTO dependency_library_websites (dependencylibrary_id, website_id) VALUES (@dependencyLibraryId, @websiteId);";
            string updateDependencyLibraryWebsiteIdSql = "UPDATE dependencies_and_libraries SET website_id = @websiteId WHERE id = @dependencyLibraryId;";

            Website existingWebsite = GetWebsiteByDependencyLibraryId(dependencyLibraryId, website.Id);

            if (existingWebsite != null)
            {
                throw new DaoException("Website already exists for this dependency/library.");
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
                                cmdInsertWebsite.Parameters.AddWithValue("@type", website.Type ?? (object)DBNull.Value);
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

            bool isWebsiteTypeRequired = false;

            CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(website, isWebsiteTypeRequired);

            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url, type = @type " +
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
                        cmd.Parameters.AddWithValue("@type", website.Type ?? (object)DBNull.Value);

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

                            int? imageId = _imageDao.GetImageIdByWebsiteId(websiteId);

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
                                            WEBSITE HELPER METHODS
            **********************************************************************************************
        */

        private void CheckNecessaryWebsitePropertiesAreNotNullOrEmpty(Website website, bool isWebsiteTypeRequired)
        {
            if (string.IsNullOrEmpty(website.Name))
            {
                throw new ArgumentException("Website name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(website.Url))
            {
                throw new ArgumentException("Website URL cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(website.Type) && isWebsiteTypeRequired)
            {
                throw new ArgumentException("Website Type cannot be null or empty.");
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
                Url = Convert.ToString(reader["url"]),
                Type = Convert.ToString(reader["type"])
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
