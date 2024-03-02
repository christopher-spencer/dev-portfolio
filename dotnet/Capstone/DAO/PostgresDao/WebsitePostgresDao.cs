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

        public Website CreateWebsiteLink(Website websiteLink)
        {
            string sql = "INSERT INTO websites (name, url) VALUES (@name, @url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", websiteLink.Name);
                    cmd.Parameters.AddWithValue("@url", websiteLink.Url);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    websiteLink.Id = id;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the website link.", ex);
            }

            return websiteLink;
        }

        public Website GetWebsiteById(int websiteId)
        {
            string sql = "SELECT w.id, w.name, w.url, i.name AS logo_name, i.url AS logo_url " +
                         "FROM websites w " +
                         "JOIN images i ON w.logo_id = i.id " +
                         "WHERE w.id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", websiteId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return MapRowToWebsite(reader);
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        websiteLinks.Add(MapRowToWebsite(reader));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website links.", ex);
            }

            return websiteLinks;
        }

        public Website UpdateWebsiteByWebsiteId(int websiteId, Website updatedWebsite)
        {
            string sql = "UPDATE websites " +
                         "SET name = @name, url = @url " +
                         "WHERE id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@websiteId", websiteId);
                    cmd.Parameters.AddWithValue("@name", updatedWebsite.Name);
                    cmd.Parameters.AddWithValue("@url", updatedWebsite.Url);

                    int count = cmd.ExecuteNonQuery();

                    if (count > 0)
                    {
                        return updatedWebsite;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the website.", ex);
            }

            return null;
        }

        public Website UpdateWebsite(Website updatedWebsite)
        {
            string sql = "UPDATE websites SET name = @name, url = @url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", updatedWebsite.Id);
                    cmd.Parameters.AddWithValue("@name", updatedWebsite.Name);
                    cmd.Parameters.AddWithValue("@url", updatedWebsite.Url);

                    int count = cmd.ExecuteNonQuery();
                    if (count == 1)
                    {
                        return updatedWebsite;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the website link.", ex);
            }

            return null;
        }

        public int DeleteWebsiteById(int websiteId)
        {
            string sql = "DELETE FROM websites WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", websiteId);

                    return cmd.ExecuteNonQuery();
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
            string insertWebsiteSql = "INSERT INTO websites (name, url) VALUES (@name, @url) RETURNING id;";
            string insertSideProjectWebsiteSql = "INSERT INTO sideproject_websites (sideproject_id, website_id) VALUES (@projectId, @websiteId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmdInsertWebsite = new NpgsqlCommand(insertWebsiteSql, connection);
                    cmdInsertWebsite.Parameters.AddWithValue("@name", website.Name);
                    cmdInsertWebsite.Parameters.AddWithValue("@url", website.Url);

                    int websiteId = (int)cmdInsertWebsite.ExecuteScalar();

                    NpgsqlCommand cmdInsertSideProjectWebsite = new NpgsqlCommand(insertSideProjectWebsiteSql, connection);
                    cmdInsertSideProjectWebsite.Parameters.AddWithValue("@projectId", projectId);
                    cmdInsertSideProjectWebsite.Parameters.AddWithValue("@websiteId", websiteId);

                    cmdInsertSideProjectWebsite.ExecuteNonQuery();

                    website.Id = websiteId;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the website.", ex);
            }

            return website;
        }

        public Website GetWebsiteByProjectId(int projectId)
        {
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        website = MapRowToWebsite(reader);
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@websiteId", websiteId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        website = MapRowToWebsite(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website by project ID and website ID.", ex);
            }

            return website;
        }

        public Website UpdateWebsiteByProjectId(int projectId, Website updatedWebsite)
        {
            string sql = "UPDATE websites " +
                         "SET name = @name, url = @url " +
                         "FROM sideproject_websites " +
                         "WHERE websites.id = sideproject_websites.website_id " +
                         "AND sideproject_websites.sideproject_id = @projectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@name", updatedWebsite.Name);
                    cmd.Parameters.AddWithValue("@url", updatedWebsite.Url);

                    int count = cmd.ExecuteNonQuery();

                    if (count > 0)
                    {
                        return updatedWebsite;
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
            string sql = "DELETE FROM sideproject_websites WHERE sideproject_id = @projectId AND website_id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@websiteId", websiteId);

                    return cmd.ExecuteNonQuery();
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

        public Website CreateWebsiteByControllerId(int controllerId, Website website)
        {
            string insertWebsiteSql = "INSERT INTO websites (name, url) VALUES (@name, @url) RETURNING id;";
            string insertControllerWebsiteSql = "INSERT INTO controller_websites (controller_id, website_id) VALUES (@controllerId, @websiteId);";
            string updateControllerWebsiteIdSql = "UPDATE controllers SET website_id = @websiteId WHERE id = @controllerId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmdInsertWebsite = new NpgsqlCommand(insertWebsiteSql, connection))
                    {
                        cmdInsertWebsite.Parameters.AddWithValue("@name", website.Name);
                        cmdInsertWebsite.Parameters.AddWithValue("@url", website.Url);

                        int websiteId = Convert.ToInt32(cmdInsertWebsite.ExecuteScalar());
                        website.Id = websiteId;
                    }

                    using (NpgsqlCommand cmdInsertControllerWebsite = new NpgsqlCommand(insertControllerWebsiteSql, connection))
                    {
                        cmdInsertControllerWebsite.Parameters.AddWithValue("@controllerId", controllerId);
                        cmdInsertControllerWebsite.Parameters.AddWithValue("@websiteId", website.Id);

                        cmdInsertControllerWebsite.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmdUpdateController = new NpgsqlCommand(updateControllerWebsiteIdSql, connection))
                    {
                        cmdUpdateController.Parameters.AddWithValue("@controllerId", controllerId);
                        cmdUpdateController.Parameters.AddWithValue("@websiteId", website.Id);

                        cmdUpdateController.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the website for the controller.", ex);
            }

            return website;
        }

        public Website GetWebsiteByContributorId(int contributorId)
        {
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

        public Website UpdateWebsiteByContributorId(int contributorId, Website updatedWebsite)
        {
            string updateWebsiteSql = "UPDATE websites " +
                                      "SET name = @name, url = @url " +
                                      "FROM contributor_websites " +
                                      "WHERE websites.id = contributor_websites.website_id AND contributor_websites.contributor_id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateWebsiteSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);
                        cmd.Parameters.AddWithValue("@name", updatedWebsite.Name);
                        cmd.Parameters.AddWithValue("@url", updatedWebsite.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return updatedWebsite;
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
