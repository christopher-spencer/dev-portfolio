using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class ImagePostgresDao : IImageDao
    {
        private readonly string connectionString;

        public ImagePostgresDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Image CreateImage(Image image)
        {
            string sql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", image.Name);
                    cmd.Parameters.AddWithValue("@url", image.Url);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    image.Id = id;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the image.", ex);
            }

            return image;
        }

        public Image GetImageById(int imageId)
        {
            string sql = "SELECT name, url FROM images WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", imageId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return MapRowToImage(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image.", ex);
            }

            return null;
        }

        public List<Image> GetAllImages()
        {
            List<Image> images = new List<Image>();
            string sql = "SELECT id, name, url FROM images;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        images.Add(MapRowToImage(reader));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the images.", ex);
            }

            return images;
        }

        public Image UpdateImage(Image image)
        {
            string sql = "UPDATE images SET name = @name, url = @url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", image.Id);
                    cmd.Parameters.AddWithValue("@name", image.Name);
                    cmd.Parameters.AddWithValue("@url", image.Url);

                    int count = cmd.ExecuteNonQuery();
                    if (count == 1)
                    {
                        return image;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the image.", ex);
            }

            return null;
        }

        public int DeleteImageById(int imageId)
        {
            string sql = "DELETE FROM images WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", imageId);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the image.", ex);
            }
        }

        private Image MapRowToImage(DbDataReader reader)
        {
            return new Image
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Url = Convert.ToString(reader["url"])
            };
        }
    }
}
