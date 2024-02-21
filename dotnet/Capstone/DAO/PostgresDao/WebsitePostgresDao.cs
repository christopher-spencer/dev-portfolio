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

        public WebsitePostgresDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Website CreateWebsiteLink(Website websiteLink)
        {
            string sql = "INSERT INTO website_links (name, url, icon_name, icon_url) VALUES (@name, @url, @icon_name, @icon_url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", websiteLink.Name);
                    cmd.Parameters.AddWithValue("@url", websiteLink.Url);
                    cmd.Parameters.AddWithValue("@icon_name", websiteLink.Icon.Name);
                    cmd.Parameters.AddWithValue("@icon_url", websiteLink.Icon.Url);

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

        public Website GetWebsiteLinkById(int websiteLinkId)
        {
            string sql = "SELECT name, url, icon_name, icon_url FROM website_links WHERE id = @id;";

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

        public List<Website> GetAllWebsiteLinks()
        {
            List<Website> websiteLinks = new List<Website>();
            string sql = "SELECT id, name, url, icon_name, icon_url FROM website_links;";

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

        public Website UpdateWebsiteLink(Website websiteLink)
        {
            string sql = "UPDATE website_links SET name = @name, url = @url, icon_name = @icon_name, icon_url = @icon_url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", websiteLink.Id);
                    cmd.Parameters.AddWithValue("@name", websiteLink.Name);
                    cmd.Parameters.AddWithValue("@url", websiteLink.Url);
                    cmd.Parameters.AddWithValue("@icon_name", websiteLink.Icon.Name);
                    cmd.Parameters.AddWithValue("@icon_url", websiteLink.Icon.Url);

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

        private Website MapRowToWebsiteLink(NpgsqlDataReader reader)
        {
            return new Website
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Url = Convert.ToString(reader["url"]),
                Icon = new Image
                {
                    Name = Convert.ToString(reader["icon_name"]),
                    Url = Convert.ToString(reader["icon_url"])
                }
            };
        }
    }
}
