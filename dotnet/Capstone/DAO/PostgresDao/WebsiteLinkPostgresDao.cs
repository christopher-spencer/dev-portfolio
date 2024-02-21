using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class WebsiteLinkPostgresDao : IWebsiteLinkDao
    {
        private readonly string connectionString;

        public WebsiteLinkPostgresDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public WebsiteLink CreateWebsiteLink(WebsiteLink websiteLink)
        {
            string sql = "INSERT INTO website_links (name, url, icon_image_name, icon_image_url) VALUES (@name, @url, @icon_image_name, @icon_image_url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", websiteLink.Name);
                    cmd.Parameters.AddWithValue("@url", websiteLink.Url);
                    cmd.Parameters.AddWithValue("@icon_image_name", websiteLink.IconImageUrl.Name);
                    cmd.Parameters.AddWithValue("@icon_image_url", websiteLink.IconImageUrl.Url);

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

        public WebsiteLink GetWebsiteLinkById(int websiteLinkId)
        {
            string sql = "SELECT name, url, icon_image_name, icon_image_url FROM website_links WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", websiteLinkId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return MapRowToWebsiteLink(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website link.", ex);
            }

            return null;
        }

        public List<WebsiteLink> GetAllWebsiteLinks()
        {
            List<WebsiteLink> websiteLinks = new List<WebsiteLink>();
            string sql = "SELECT id, name, url, icon_image_name, icon_image_url FROM website_links;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        websiteLinks.Add(MapRowToWebsiteLink(reader));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website links.", ex);
            }

            return websiteLinks;
        }

        public WebsiteLink UpdateWebsiteLink(WebsiteLink websiteLink)
        {
            string sql = "UPDATE website_links SET name = @name, url = @url, icon_image_name = @icon_image_name, icon_image_url = @icon_image_url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", websiteLink.Id);
                    cmd.Parameters.AddWithValue("@name", websiteLink.Name);
                    cmd.Parameters.AddWithValue("@url", websiteLink.Url);
                    cmd.Parameters.AddWithValue("@icon_image_name", websiteLink.IconImageUrl.Name);
                    cmd.Parameters.AddWithValue("@icon_image_url", websiteLink.IconImageUrl.Url);

                    int count = cmd.ExecuteNonQuery();
                    if (count == 1)
                    {
                        return websiteLink;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the website link.", ex);
            }

            return null;
        }

        public int DeleteWebsiteLinkById(int websiteLinkId)
        {
            string sql = "DELETE FROM website_links WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", websiteLinkId);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the website link.", ex);
            }
        }

        private WebsiteLink MapRowToWebsiteLink(NpgsqlDataReader reader)
        {
            return new WebsiteLink
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Url = Convert.ToString(reader["url"]),
                IconImageUrl = new Image
                {
                    Name = Convert.ToString(reader["icon_image_name"]),
                    Url = Convert.ToString(reader["icon_image_url"])
                }
            };
        }
    }
}
