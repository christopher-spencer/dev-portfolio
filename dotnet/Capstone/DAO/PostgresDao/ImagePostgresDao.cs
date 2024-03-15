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

        /*  
            **********************************************************************************************
                                                    IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImage(Image image)
        {
            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string sql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        image.Id = id;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the image.", ex);
            }

            return image;
        }

        public Image GetImage(int imageId)
        {
            if (imageId <= 0)
            {
                throw new ArgumentException("ImageId must be greater than zero.");
            }

            string sql = "SELECT id, name, url FROM images WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", imageId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image.", ex);
            }

            return null;
        }

        public List<Image> GetImages()
        {
            List<Image> images = new List<Image>();

            string sql = "SELECT id, name, url FROM images;";

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
                                images.Add(MapRowToImage(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the images.", ex);
            }

            return images;
        }

        public Image UpdateImage(Image image, int imageId)
        {
            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            if (imageId <= 0)
            {
                throw new ArgumentException("ImageId must be greater than zero.");
            }

            string sql = "UPDATE images SET name = @name, url = @url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return image;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the image.", ex);
            }

            return null;
        }

        public int DeleteImage(int imageId)
        {
            if (imageId <= 0)
            {
                throw new ArgumentException("ImageId must be greater than zero.");
            }

            string sql = "DELETE FROM images WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", imageId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the image.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                            SIDE PROJECT IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageBySideProjectId(int sideProjectId, Image image)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertSideProjectImageSql = "INSERT INTO sideproject_images (sideproject_id, image_id) VALUES (@sideProjectId, @imageId);";
            string updateSideProjectMainImageSql = "UPDATE sideprojects SET main_image_id = @imageId WHERE id = @sideProjectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);

                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertSideProjectImage = new NpgsqlCommand(insertSideProjectImageSql, connection))
                            {
                                cmdInsertSideProjectImage.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmdInsertSideProjectImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertSideProjectImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateSideProjectMainImage = new NpgsqlCommand(updateSideProjectMainImageSql, connection))
                            {
                                cmdUpdateSideProjectMainImage.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmdUpdateSideProjectMainImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateSideProjectMainImage.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetImageBySideProjectId(int sideProjectId, int imageId)
        {
            if (sideProjectId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("SideProjectId and ImageId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url " +
                         "FROM images i " +
                         "JOIN sideproject_images spi ON i.id = spi.image_id " +
                         "WHERE spi.sideproject_id = @sideProjectId AND i.id = @imageId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                image = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by sideProject ID.", ex);
            }

            return image;
        }

        public List<Image> GetImagesBySideProjectId(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            List<Image> images = new List<Image>();

            string sql = "SELECT i.id, i.name, i.url " +
                         "FROM images i " +
                         "JOIN sideproject_images spi ON i.id = spi.image_id " +
                         "WHERE spi.sideproject_id = @sideProjectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Image image = MapRowToImage(reader);
                                images.Add(image);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving images by SideProject ID.", ex);
            }

            return images;
        }

        public Image GetImageBySideProjectIdAndImageId(int sideProjectId, int imageId)
        {
            if (sideProjectId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("SideProjectId and imageId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url " +
                         "FROM images i " +
                         "JOIN sideproject_images spi ON i.id = spi.image_id " +
                         "WHERE spi.sideproject_id = @sideProjectId AND i.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                image = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by sideProject ID and image ID.", ex);
            }

            return image;
        }

        public Image UpdateImageBySideProjectId(int sideProjectId, int imageId, Image image)
        {
            if (sideProjectId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("SideProjectId and imageId must be greater than zero.");
            }

            string sql = "UPDATE images " +
                         "SET name = @name, url = @url " +
                         "FROM sideproject_images " +
                         "WHERE images.id = sideproject_images.image_id AND sideproject_images.sideproject_id = @sideProjectId " +
                         "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return image;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the image.", ex);
            }

            return null;
        }

        public int DeleteImageBySideProjectId(int sideProjectId, int imageId)
        {
            if (sideProjectId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("SideProjectId and imageId must be greater than zero.");
            }

            string deleteSideProjectImageSql = "DELETE FROM sideproject_images WHERE sideproject_id = @sideProjectId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSideProjectImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the image from the side project.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                            BLOG POST IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByBlogPostId(int blogPostId, Image image)
        {
            if (blogPostId <= 0)
            {
                throw new ArgumentException("BlogPostId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertBlogPostImageSql = "INSERT INTO blogpost_images (blogpost_id, image_id) VALUES (@blogPostId, @imageId);";
            string updateBlogPostMainImageSql = "UPDATE blogposts SET main_image_id = @imageId WHERE id = @blogPostId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);

                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertBlogPostImage = new NpgsqlCommand(insertBlogPostImageSql, connection))
                            {
                                cmdInsertBlogPostImage.Parameters.AddWithValue("@blogPostId", blogPostId);
                                cmdInsertBlogPostImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertBlogPostImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateBlogPost = new NpgsqlCommand(updateBlogPostMainImageSql, connection))
                            {
                                cmdUpdateBlogPost.Parameters.AddWithValue("@blogPostId", blogPostId);
                                cmdUpdateBlogPost.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateBlogPost.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetImageByImageIdAndBlogPostId(int imageId, int blogPostId)
        {
            if (blogPostId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("BlogPostId and imageId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url " +
                         "FROM images i " +
                         "JOIN blogpost_images bi ON i.id = bi.image_id " +
                         "WHERE bi.image_id = @imageId AND bi.blogpost_id = @blogPostId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@blogPostId", blogPostId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                image = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by image ID and blog post ID.", ex);
            }

            return image;
        }

        public List<Image> GetImagesByBlogPostId(int blogPostId)
        {
            if (blogPostId <= 0)
            {
                throw new ArgumentException("BlogPostId must be greater than zero.");
            }

            List<Image> images = new List<Image>();

            string sql = "SELECT i.id, i.name, i.url " +
                         "FROM images i " +
                         "JOIN blogpost_images bi ON i.id = bi.image_id " +
                         "WHERE bi.blogpost_id = @blogPostId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@blogPostId", blogPostId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Image image = MapRowToImage(reader);
                                images.Add(image);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving images by blog post ID.", ex);
            }

            return images;
        }

        public Image UpdateImageByBlogPostId(int blogPostId, int imageId, Image image)
        {
            if (blogPostId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("BlogPostId and imageId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url " +
                                    "FROM blogpost_images " +
                                    "WHERE images.id = blogpost_images.image_id AND blogpost_images.blogpost_id = @blogPostId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@blogPostId", blogPostId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return image;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the image by blog post ID.", ex);
            }

            return null;
        }

        public int DeleteImageByBlogPostId(int blogPostId, int imageId)
        {
            if (blogPostId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("BlogPostId and imageId must be greater than zero.");
            }

            string deleteBlogPostImageSql = "DELETE FROM blogpost_images WHERE blogpost_id = @blogPostId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteBlogPostImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@blogPostId", blogPostId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the image by blog post ID.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                             WEBSITE IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByWebsiteId(int websiteId, Image image)
        {
            if (websiteId <= 0)
            {
                throw new ArgumentException("WebsiteId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertWebsiteImageSql = "INSERT INTO website_images (website_id, image_id) VALUES (@websiteId, @imageId);";
            string updateWebsiteLogoIdSql = "UPDATE websites SET logo_id = @imageId WHERE id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);

                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertWebsiteImage = new NpgsqlCommand(insertWebsiteImageSql, connection))
                            {
                                cmdInsertWebsiteImage.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertWebsiteImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertWebsiteImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateWebsiteLogoId = new NpgsqlCommand(updateWebsiteLogoIdSql, connection))
                            {
                                cmdUpdateWebsiteLogoId.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdUpdateWebsiteLogoId.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateWebsiteLogoId.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image for the website.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetImageByWebsiteId(int websiteId)
        {
            if (websiteId <= 0)
            {
                throw new ArgumentException("WebsiteId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url " +
                         "FROM images i " +
                         "JOIN website_images wi ON i.id = wi.image_id " +
                         "WHERE wi.website_id = @websiteId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                image = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by website ID.", ex);
            }

            return image;
        }

        public Image UpdateImageByWebsiteId(int websiteId, int imageId, Image image)
        {
            if (imageId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("SideProjectId and websiteId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url " +
                                    "FROM website_images " +
                                    "WHERE images.id = website_images.image_id AND website_images.website_id = @websiteId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return image;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the image by website ID.", ex);
            }

            return null;
        }

        public int DeleteImageByWebsiteId(int websiteId, int imageId)
        {
            if (imageId <= 0 || websiteId <= 0)
            {
                throw new ArgumentException("ImageId and websiteId must be greater than zero.");
            }

            string deleteWebsiteImageSql = "DELETE FROM website_images WHERE website_id = @websiteId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the image by website ID.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                            SKILL IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageBySkillId(int skillId, Image image)
        {
            if (skillId <= 0)
            {
                throw new ArgumentException("SkillId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertSkillImageSql = "INSERT INTO skill_images (skill_id, image_id) VALUES (@skillId, @imageId);";
            string updateSkillImageIdSql = "UPDATE skills SET icon_id = @imageId WHERE id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);

                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertSkillImage = new NpgsqlCommand(insertSkillImageSql, connection))
                            {
                                cmdInsertSkillImage.Parameters.AddWithValue("@skillId", skillId);
                                cmdInsertSkillImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertSkillImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateSkillImageId = new NpgsqlCommand(updateSkillImageIdSql, connection))
                            {
                                cmdUpdateSkillImageId.Parameters.AddWithValue("@skillId", skillId);
                                cmdUpdateSkillImageId.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateSkillImageId.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image for the skill.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetImageBySkillId(int skillId)
        {
            if (skillId <= 0)
            {
                throw new ArgumentException("SkillId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url " +
                         "FROM images i " +
                         "JOIN skill_images si ON i.id = si.image_id " +
                         "WHERE si.skill_id = @skillId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@skillId", skillId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                image = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by skill ID.", ex);
            }

            return image;
        }

        public Image UpdateImageBySkillId(int skillId, int imageId, Image image)
        {
            if (skillId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("SkillId and imageId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url " +
                                    "FROM skill_images " +
                                    "WHERE images.id = skill_images.image_id AND skill_images.skill_id = @skillId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@skillId", skillId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return image;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the image by skill ID.", ex);
            }

            return null;
        }

        public int DeleteImageBySkillId(int skillId, int imageId)
        {
            if (skillId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("SkillId and imageId must be greater than zero.");
            }

            string deleteSkillImageSql = "DELETE FROM skill_images WHERE skill_id = @skillId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@skillId", skillId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the image by skill ID.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                            GOAL IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByGoalId(int goalId, Image image)
        {
            if (goalId <= 0)
            {
                throw new ArgumentException("GoalId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertGoalImageSql = "INSERT INTO goal_images (goal_id, image_id) VALUES (@goalId, @imageId);";
            string updateGoalImageIdSql = "UPDATE goals SET icon_id = @imageId WHERE id = @goalId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);

                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertGoalImage = new NpgsqlCommand(insertGoalImageSql, connection))
                            {
                                cmdInsertGoalImage.Parameters.AddWithValue("@goalId", goalId);
                                cmdInsertGoalImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertGoalImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateGoalImageId = new NpgsqlCommand(updateGoalImageIdSql, connection))
                            {
                                cmdUpdateGoalImageId.Parameters.AddWithValue("@goalId", goalId);
                                cmdUpdateGoalImageId.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateGoalImageId.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image for the goal.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetImageByGoalId(int goalId)
        {
            if (goalId <= 0)
            {
                throw new ArgumentException("GoalId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url FROM images i " +
                         "JOIN goal_images gi ON i.id = gi.image_id " +
                         "WHERE gi.goal_id = @goalId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@goalId", goalId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                image = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image for the goal.", ex);
            }

            return image;
        }

        public Image UpdateImageByGoalId(int goalId, int imageId, Image image)
        {
            if (goalId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("GoalId and imageId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url " +
                                    "FROM goal_images " +
                                    "WHERE images.id = goal_images.image_id AND goal_images.goal_id = @goalId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@goalId", goalId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return image;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the image by goal ID.", ex);
            }

            return null;
        }

        public int DeleteImageByGoalId(int goalId, int imageId)
        {
            if (goalId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("GoalId and imageId must be greater than zero.");
            }

            string deleteGoalImageSql = "DELETE FROM goal_images WHERE goal_id = @goalId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteGoalImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@goalId", goalId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the image for the goal.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                            CONTRIBUTOR IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByContributorId(int contributorId, Image image)
        {
            if (contributorId <= 0)
            {
                throw new ArgumentException("ContributorId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertContributorImageSql = "INSERT INTO contributor_images (contributor_id, image_id) VALUES (@contributorId, @imageId);";
            string updateContributorImageIdSql = "UPDATE contributors SET contributor_image_id = @imageId WHERE id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);

                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertContributorImage = new NpgsqlCommand(insertContributorImageSql, connection))
                            {
                                cmdInsertContributorImage.Parameters.AddWithValue("@contributorId", contributorId);
                                cmdInsertContributorImage.Parameters.AddWithValue("@imageId", imageId);

                                cmdInsertContributorImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateContributor = new NpgsqlCommand(updateContributorImageIdSql, connection))
                            {
                                cmdUpdateContributor.Parameters.AddWithValue("@contributorId", contributorId);
                                cmdUpdateContributor.Parameters.AddWithValue("@imageId", imageId);

                                cmdUpdateContributor.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new DaoException("An error occurred while creating the image for the contributor.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetImageByContributorId(int contributorId)
        {
            if (contributorId <= 0)
            {
                throw new ArgumentException("ContributorId must be greater than zero.");
            }

            Image image = null;
            string sql = "SELECT i.id, i.name, i.url " +
                         "FROM images i " +
                         "JOIN contributor_images ci ON i.id = ci.image_id " +
                         "WHERE ci.contributor_id = @contributorId";

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
                                image = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by contributor ID.", ex);
            }

            return image;
        }

        public Image UpdateImageByContributorId(int contributorId, int imageId, Image image)
        {
            if (contributorId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("ContributorId and imageId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url " +
                                    "FROM contributor_images " +
                                    "WHERE images.id = contributor_images.image_id AND contributor_images.contributor_id = @contributorId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return image;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the image by contributor ID.", ex);
            }

            return null;
        }

        public int DeleteImageByContributorId(int contributorId, int imageId)
        {
            if (contributorId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("ContributorId and imageId must be greater than zero.");
            }

            string deleteContributorImageSql = "DELETE FROM contributor_images WHERE contributor_id = @contributorId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteContributorImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the image by contributor ID.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                         API AND SERVICE IMAGE CRUD
            **********************************************************************************************
        */
        public Image CreateImageByApiServiceId(int apiServiceId, Image image)
        {
            if (apiServiceId <= 0)
            {
                throw new ArgumentException("ApiServiceId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertApiServiceImageSql = "INSERT INTO api_service_images (apiservice_id, image_id) VALUES (@apiServiceId, @imageId);";
            string updateApiServiceLogoIdSql = "UPDATE apis_and_services SET logo_id = @imageId WHERE id = @apiServiceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);

                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertApiServiceImage = new NpgsqlCommand(insertApiServiceImageSql, connection))
                            {
                                cmdInsertApiServiceImage.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                                cmdInsertApiServiceImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertApiServiceImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateApiServiceLogo = new NpgsqlCommand(updateApiServiceLogoIdSql, connection))
                            {
                                cmdUpdateApiServiceLogo.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                                cmdUpdateApiServiceLogo.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateApiServiceLogo.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image for the API/Service.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetImageByApiServiceId(int apiServiceId)
        {
            if (apiServiceId <= 0)
            {
                throw new ArgumentException("ApiServiceId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url " +
                         "FROM images i " +
                         "JOIN api_service_images asi ON i.id = asi.image_id " +
                         "WHERE asi.apiservice_id = @apiServiceId";

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
                                image = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by API/Service ID.", ex);
            }

            return image;
        }

        public Image UpdateImageByApiServiceId(int apiServiceId, int imageId, Image image)
        {
            if (apiServiceId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("ApiServiceId and imageId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url " +
                                    "FROM api_service_images " +
                                    "WHERE images.id = api_service_images.image_id AND api_service_images.apiservice_id = @apiServiceId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return image;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the image by API/Service ID.", ex);
            }

            return null;
        }

        public int DeleteImageByApiServiceId(int apiServiceId, int imageId)
        {
            if (apiServiceId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("ApiServiceId and imageId must be greater than zero.");
            }

            string deleteApiServiceImageSql = "DELETE FROM api_service_images WHERE apiservice_id = @apiServiceId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteApiServiceImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the image by API/Service ID.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                        DEPENDENCY AND LIBRARY IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByDependencyLibraryId(int dependencyLibraryId, Image image)
        {
            if (dependencyLibraryId <= 0)
            {
                throw new ArgumentException("DependencyLibraryId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertLibraryImageSql = "INSERT INTO dependency_library_images (dependencylibrary_id, image_id) VALUES (@dependencyLibraryId, @imageId);";
            string updateLibraryLogoIdSql = "UPDATE dependencies_and_libraries SET logo_id = @imageId WHERE id = @dependencyLibraryId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);

                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertLibraryImage = new NpgsqlCommand(insertLibraryImageSql, connection))
                            {
                                cmdInsertLibraryImage.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                                cmdInsertLibraryImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertLibraryImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateLibraryLogoId = new NpgsqlCommand(updateLibraryLogoIdSql, connection))
                            {
                                cmdUpdateLibraryLogoId.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                                cmdUpdateLibraryLogoId.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateLibraryLogoId.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image for the dependency library.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetImageByDependencyLibraryId(int dependencyLibraryId)
        {
            if (dependencyLibraryId <= 0)
            {
                throw new ArgumentException("DependencyLibraryId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url " +
                         "FROM images i " +
                         "JOIN dependency_library_images dli ON i.id = dli.image_id " +
                         "WHERE dli.dependencylibrary_id = @dependencyLibraryId";

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
                                image = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by dependency library ID.", ex);
            }

            return image;
        }

        public Image UpdateImageByDependencyLibraryId(int dependencyLibraryId, int imageId, Image image)
        {
            if (dependencyLibraryId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("DependencyLibraryId and imageId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url " +
                                    "FROM dependency_library_images " +
                                    "WHERE images.id = dependency_library_images.image_id AND " +
                                    "dependency_library_images.dependencylibrary_id = @dependencyLibraryId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return image;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the image by dependency library ID.", ex);
            }

            return null;
        }

        public int DeleteImageByDependencyLibraryId(int dependencyLibraryId, int imageId)
        {
            if (dependencyLibraryId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("DependencyLibraryId and imageId must be greater than zero.");
            }

            string deleteLibraryImageSql = "DELETE FROM dependency_library_images WHERE dependencylibrary_id = @dependencyLibraryId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteLibraryImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the image by dependency library ID.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                                MAP ROW TO IMAGE
            **********************************************************************************************
        */

        private Image MapRowToImage(NpgsqlDataReader reader)
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
