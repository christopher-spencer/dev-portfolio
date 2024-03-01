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

        public Image UpdateImage(Image image, int imageId)
        {
            string sql = "UPDATE images SET name = @name, url = @url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
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

        /*  
            **********************************************************************************************
                                            SIDE PROJECT IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageBySideProjectId(int sideProjectId, Image image)
        {
            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertSideProjectImageSql = "INSERT INTO sideproject_images (sideproject_id, image_id) VALUES (@sideProjectId, @imageId);";
            string updateSideProjectMainImageSql = "UPDATE sideprojects SET main_image_id = @imageId WHERE id = @sideProjectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        image.Id = id;
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertSideProjectImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@imageId", image.Id);

                        cmd.ExecuteNonQuery();
                    }

                    // Set the main_image_id in the sideprojects table
                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateSideProjectMainImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@imageId", image.Id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the image.", ex);
            }

            return image;
        }

        public Image GetImageBySideProjectId(int sideProjectId, int imageId)
        {
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                    cmd.Parameters.AddWithValue("@imageId", imageId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        image = MapRowToImage(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by project ID.", ex);
            }

            return image;
        }

        public List<Image> GetImagesBySideProjectId(int sideProjectId)
        {
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Image image = MapRowToImage(reader);
                        images.Add(image);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving images by project ID.", ex);
            }

            return images;
        }

        public Image GetImageBySideProjectIdAndImageId(int sideProjectId, int imageId)
        {
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                    cmd.Parameters.AddWithValue("@imageId", imageId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        image = MapRowToImage(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by project ID and image ID.", ex);
            }

            return image;
        }

        public Image UpdateImageBySideProjectId(int sideProjectId, Image updatedImage)
        {
            string sql = "UPDATE images " +
                         "SET name = @name, url = @url " +
                         "FROM sideproject_images " +
                         "WHERE images.id = sideproject_images.image_id AND sideproject_images.sideproject_id = @sideProjectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                    cmd.Parameters.AddWithValue("@name", updatedImage.Name);
                    cmd.Parameters.AddWithValue("@url", updatedImage.Url);

                    int count = cmd.ExecuteNonQuery();

                    if (count > 0)
                    {
                        return updatedImage;
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
                throw new DaoException("An error occurred while deleting the image.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                            BLOG POST IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByBlogPostId(int blogPostId, Image image)
        {
            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertBlogPostImageSql = "INSERT INTO blogpost_images (blogpost_id, image_id) VALUES (@blogPostId, @imageId);";
            string updateBlogPostMainImageSql = "UPDATE blogposts SET main_image_id = @imageId WHERE id = @blogPostId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        image.Id = id;
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertBlogPostImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@blogPostId", blogPostId);
                        cmd.Parameters.AddWithValue("@imageId", image.Id);

                        cmd.ExecuteNonQuery();
                    }

                    // Set the main_image_id in the blogposts table
                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateBlogPostMainImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@blogPostId", blogPostId);
                        cmd.Parameters.AddWithValue("@imageId", image.Id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the image.", ex);
            }

            return image;
        }

        public Image GetImageByImageIdAndBlogPostId(int imageId, int blogPostId)
        {
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@imageId", imageId);
                    cmd.Parameters.AddWithValue("@blogPostId", blogPostId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        image = MapRowToImage(reader);
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

        public Image UpdateImageByBlogPostId(int blogPostId, Image updatedImage)
        {
            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url " +
                                    "FROM blogpost_images " +
                                    "WHERE images.id = blogpost_images.image_id AND blogpost_images.blogpost_id = @blogPostId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@blogPostId", blogPostId);
                        cmd.Parameters.AddWithValue("@name", updatedImage.Name);
                        cmd.Parameters.AddWithValue("@url", updatedImage.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return updatedImage;
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
            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertWebsiteImageSql = "INSERT INTO website_images (website_id, image_id) VALUES (@websiteId, @imageId);";
            string updateWebsiteLogoIdSql = "UPDATE websites SET logo_id = @imageId WHERE id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        image.Id = id;
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertWebsiteImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@imageId", image.Id);

                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateWebsiteLogoIdSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@imageId", image.Id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the image for the website.", ex);
            }

            return image;
        }

        public Image GetImageByWebsiteId(int websiteId)
        {
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@websiteId", websiteId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        image = MapRowToImage(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by website ID.", ex);
            }

            return image;
        }

        public Image UpdateImageByWebsiteId(int websiteId, Image updatedImage)
        {
            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url " +
                                    "FROM website_images " +
                                    "WHERE images.id = website_images.image_id AND website_images.website_id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
                        cmd.Parameters.AddWithValue("@name", updatedImage.Name);
                        cmd.Parameters.AddWithValue("@url", updatedImage.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return updatedImage;
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
            string insertImageSql = "INSERT INTO images (name, url) VALUES (@name, @url) RETURNING id;";
            string insertSkillImageSql = "INSERT INTO skill_images (skill_id, image_id) VALUES (@skillId, @imageId);";
            string updateSkillImageIdSql = "UPDATE skills SET icon_id = @imageId WHERE id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                    {
                        cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                        cmdInsertImage.Parameters.AddWithValue("@url", image.Url);

                        int id = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                        image.Id = id;
                    }

                    using (NpgsqlCommand cmdInsertSkillImage = new NpgsqlCommand(insertSkillImageSql, connection))
                    {
                        cmdInsertSkillImage.Parameters.AddWithValue("@skillId", skillId);
                        cmdInsertSkillImage.Parameters.AddWithValue("@imageId", image.Id);

                        cmdInsertSkillImage.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmdUpdateSkillImageId = new NpgsqlCommand(updateSkillImageIdSql, connection))
                    {
                        cmdUpdateSkillImageId.Parameters.AddWithValue("@skillId", skillId);
                        cmdUpdateSkillImageId.Parameters.AddWithValue("@imageId", image.Id);

                        cmdUpdateSkillImageId.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the image for the skill.", ex);
            }

            return image;
        }


        public Image GetImageBySkillId(int skillId)
        {
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@skillId", skillId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        image = MapRowToImage(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the image by skill ID.", ex);
            }

            return image;
        }

        public Image UpdateImageBySkillId(int skillId, Image updatedImage)
        {
            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url " +
                                    "FROM skill_images " +
                                    "WHERE images.id = skill_images.image_id AND skill_images.skill_id = @skillId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@skillId", skillId);
                        cmd.Parameters.AddWithValue("@name", updatedImage.Name);
                        cmd.Parameters.AddWithValue("@url", updatedImage.Url);

                        int count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            return updatedImage;
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
