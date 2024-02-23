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

        public Website CreateWebsiteByProjectId(int projectId, Website website)
        {
            string insertWebsiteSql = "INSERT INTO websites (name, url, logo_id) VALUES (@name, @url, @logoId) RETURNING id;";
            string insertSideProjectWebsiteSql = "INSERT INTO sideproject_websites (sideproject_id, website_id) VALUES (@projectId, @websiteId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmdInsertWebsite = new NpgsqlCommand(insertWebsiteSql, connection);
                    cmdInsertWebsite.Parameters.AddWithValue("@name", website.Name);
                    cmdInsertWebsite.Parameters.AddWithValue("@url", website.Url);
                    cmdInsertWebsite.Parameters.AddWithValue("@logoId", website.Logo.Id);

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

        public Website CreateWebsiteLink(Website websiteLink)
        {
            string sql = "INSERT INTO websites (name, url, logo_id) VALUES (@name, @url, @logoId) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", websiteLink.Name);
                    cmd.Parameters.AddWithValue("@url", websiteLink.Url);
                    cmd.Parameters.AddWithValue("@logoId", websiteLink.Logo.Id);

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
        private Website MapRowToWebsite(NpgsqlDataReader reader)
        {
            Website website = new Website
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Url = Convert.ToString(reader["url"]),
                LogoId = Convert.ToInt32(reader["logo_id"])
            };

            if (reader["logo_id"] != DBNull.Value)
            {
                int logoId = Convert.ToInt32(reader["logo_id"]);
                website.Logo = _imageDao.GetImageById(logoId);
            }

            return website;
        }
    }
}
