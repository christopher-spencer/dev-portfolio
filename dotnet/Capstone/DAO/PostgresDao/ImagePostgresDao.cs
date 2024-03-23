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

        const string MainImage = "main image";
        const string AdditionalImage = "additional image";
        const string Logo = "logo";

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

            string sql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);
                        cmd.Parameters.AddWithValue("@type", image.Type);

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

            string sql = "SELECT id, name, url, type FROM images WHERE id = @id;";

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

            string sql = "SELECT id, name, url, type FROM images;";

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

            string sql = "UPDATE images SET name = @name, url = @url, type = @type WHERE id = @id;";

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
                        cmd.Parameters.AddWithValue("@type", image.Type);

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
                                            PORTFOLIO IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByPortfolioId(int portfolioId, Image image)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertPortfolioImageSql = "INSERT INTO portfolio_images (portfolio_id, image_id) VALUES (@portfolioId, @imageId);";

            // updatePortfolioMainImageSql only occurs if IsMainImage is true and a Main Image doesn't already exist
            string updatePortfolioMainImageSql = "UPDATE portfolios SET main_image_id = @imageId WHERE id = @portfolioId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            Image existingMainImage = GetMainImageByPortfolioId(portfolioId);

                            if (existingMainImage != null)
                            {
                                image.Type = AdditionalImage;
                            }

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);
                                cmdInsertImage.Transaction = transaction;
                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertPortfolioImage = new NpgsqlCommand(insertPortfolioImageSql, connection))
                            {
                                cmdInsertPortfolioImage.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmdInsertPortfolioImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertPortfolioImage.Transaction = transaction;
                                cmdInsertPortfolioImage.ExecuteNonQuery();
                            }

                            if ((image.Type == MainImage) && (existingMainImage == null))
                            {
                                using (NpgsqlCommand cmdUpdatePortfolioMainImage = new NpgsqlCommand(updatePortfolioMainImageSql, connection))
                                {
                                    cmdUpdatePortfolioMainImage.Parameters.AddWithValue("@portfolioId", portfolioId);
                                    cmdUpdatePortfolioMainImage.Parameters.AddWithValue("@imageId", imageId);
                                    cmdUpdatePortfolioMainImage.Transaction = transaction;
                                    cmdUpdatePortfolioMainImage.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the main image for the portfolio.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetMainImageByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            Image mainImage = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN portfolio_images pi ON i.id = pi.image_id " +
                         "WHERE pi.portfolio_id = @portfolioId AND i.type = @imageType;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@imageType", MainImage);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                mainImage = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Main Image by Portfolio ID.", ex);
            }

            return mainImage;
        }

        public Image GetImageByPortfolioId(int portfolioId, int imageId)
        {
            if (portfolioId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("PortfolioId and ImageId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN portfolio_images pi ON i.id = pi.image_id " +
                         "WHERE pi.portfolio_id = @portfolioId AND i.id = @imageId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
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
                throw new DaoException("An error occurred while retrieving the image by portfolio ID and image ID.", ex);
            }

            return image;
        }

        public List<Image> GetAllImagesByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            List<Image> images = new List<Image>();

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN portfolio_images pi ON i.id = pi.image_id " +
                         "WHERE pi.portfolio_id = @portfolioId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);

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
                throw new DaoException("An error occurred while retrieving all images by Portfolio ID.", ex);
            }

            return images;
        }

        public List<Image> GetAdditionalImagesByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            List<Image> additionalImages = new List<Image>();

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN portfolio_images pi ON i.id = pi.image_id " +
                         "WHERE pi.portfolio_id = @portfolioId AND i.type = @imageType;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@imageType", AdditionalImage);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Image image = MapRowToImage(reader);
                                additionalImages.Add(image);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the additional images by portfolio ID.", ex);
            }

            return additionalImages;
        }

        public Image UpdateImageByPortfolioId(int portfolioId, int imageId, Image image)
        {
            if (portfolioId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("PortfolioId and imageId must be greater than zero.");
            }

            string sql = "UPDATE images " +
                         "SET name = @name, url = @url, type = @type " +
                         "FROM portfolio_images " +
                         "WHERE images.id = portfolio_images.image_id AND portfolio_images.portfolio_id = @portfolioId " +
                         "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    Image existingMainImage = GetMainImageByPortfolioId(portfolioId);

                    if (existingMainImage != null)
                    {
                        image.Type = AdditionalImage;
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);
                        cmd.Parameters.AddWithValue("@type", image.Type);

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

        public Image UpdateMainImageByPortfolioId(int portfolioId, int mainImageId, Image mainImage)
        {
            if (mainImage.Type != MainImage)
            {
                throw new ArgumentException("The image provided is not a main image. Please provide a main image.");
            }
            else
            {
                DeleteImageByPortfolioId(portfolioId, mainImageId);
                CreateImageByPortfolioId(portfolioId, mainImage);
            }

            return mainImage;
        }

        public int DeleteImageByPortfolioId(int portfolioId, int imageId)
        {
            if (portfolioId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("PortfolioId and imageId must be greater than zero.");
            }

            // UpdateMainImageIdSql only runs if the image is the Main Image
            string updateMainImageIdSql = "UPDATE portfolios SET main_image_id = NULL WHERE main_image_id = @imageId;";

            string deletePortfolioImageSql = "DELETE FROM portfolio_images WHERE portfolio_id = @portfolioId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

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

                            Image image = GetImageByImageId(imageId);

                            if (image.Type == MainImage)
                            {
                                using (NpgsqlCommand cmd = new NpgsqlCommand(updateMainImageIdSql, connection))
                                {
                                    cmd.Transaction = transaction;
                                    cmd.Parameters.AddWithValue("@imageId", imageId);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deletePortfolioImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image from the portfolio.", ex);
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

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertSideProjectImageSql = "INSERT INTO sideproject_images (sideproject_id, image_id) VALUES (@sideProjectId, @imageId);";

            // updateSideProjectMainImageSql only occurs if image type is Main Image and a Main Image doesn't already exist
            string updateSideProjectMainImageSql = "UPDATE sideprojects SET main_image_id = @imageId WHERE id = @sideProjectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            Image existingMainImage = GetMainImageBySideProjectId(sideProjectId);

                            if (existingMainImage != null)
                            {
                                image.Type = AdditionalImage;
                            }

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);
                                cmdInsertImage.Transaction = transaction;
                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertSideProjectImage = new NpgsqlCommand(insertSideProjectImageSql, connection))
                            {
                                cmdInsertSideProjectImage.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmdInsertSideProjectImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertSideProjectImage.Transaction = transaction;
                                cmdInsertSideProjectImage.ExecuteNonQuery();
                            }

                            if ((image.Type == MainImage) && (existingMainImage == null))
                            {
                                using (NpgsqlCommand cmdUpdateSideProjectMainImage = new NpgsqlCommand(updateSideProjectMainImageSql, connection))
                                {
                                    cmdUpdateSideProjectMainImage.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                    cmdUpdateSideProjectMainImage.Parameters.AddWithValue("@imageId", imageId);
                                    cmdUpdateSideProjectMainImage.Transaction = transaction;
                                    cmdUpdateSideProjectMainImage.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image for the side project.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetMainImageBySideProjectId(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            Image mainImage = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN sideproject_images spi ON i.id = spi.image_id " +
                         "WHERE spi.sideproject_id = @sideProjectId AND i.type = @imageType;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@imageType", MainImage);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                mainImage = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Main Image by Side Project ID.", ex);
            }

            return mainImage;
        }

        public Image GetImageBySideProjectId(int sideProjectId, int imageId)
        {
            if (sideProjectId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("SideProjectId and ImageId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
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
                throw new DaoException("An error occurred while retrieving the image by sideProject ID and image ID.", ex);
            }

            return image;
        }

        public List<Image> GetAllImagesBySideProjectId(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            List<Image> images = new List<Image>();

            string sql = "SELECT i.id, i.name, i.url, i.type " +
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

        public List<Image> GetAdditionalImagesBySideProjectId(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            List<Image> additionalImages = new List<Image>();

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN sideproject_images spi ON i.id = spi.image_id " +
                         "WHERE spi.sideproject_id = @sideProjectId AND i.type = @imageType;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@imageType", AdditionalImage);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Image image = MapRowToImage(reader);
                                additionalImages.Add(image);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the additional images by side project ID.", ex);
            }

            return additionalImages;
        }

        public Image UpdateImageBySideProjectId(int sideProjectId, int imageId, Image image)
        {
            if (sideProjectId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("SideProjectId and imageId must be greater than zero.");
            }

            string sql = "UPDATE images " +
                         "SET name = @name, url = @url, type = @type " +
                         "FROM sideproject_images " +
                         "WHERE images.id = sideproject_images.image_id AND sideproject_images.sideproject_id = @sideProjectId " +
                         "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    Image existingMainImage = GetMainImageBySideProjectId(sideProjectId);

                    if (existingMainImage != null)
                    {
                        image.Type = AdditionalImage;
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);
                        cmd.Parameters.AddWithValue("@type", image.Type);

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

        public Image UpdateMainImageBySideProjectId(int sideProjectId, int mainImageId, Image mainImage)
        {
            if (mainImage.Type != MainImage)
            {
                throw new ArgumentException("The image provided is not a main image. Please provide a main image.");
            }
            else
            {
                DeleteImageBySideProjectId(sideProjectId, mainImageId);
                CreateImageBySideProjectId(sideProjectId, mainImage);
            }

            return mainImage;
        }

        public int DeleteImageBySideProjectId(int sideProjectId, int imageId)
        {
            if (sideProjectId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("SideProjectId and imageId must be greater than zero.");
            }

            // UpdateMainImageIdSql only runs if the image is the Main Image
            string updateMainImageIdSql = "UPDATE sideprojects SET main_image_id = NULL WHERE main_image_id = @imageId;";

            string deleteSideProjectImageSql = "DELETE FROM sideproject_images WHERE sideproject_id = @sideProjectId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

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

                            Image image = GetImageByImageId(imageId);

                            if (image.Type == MainImage)
                            {
                                using (NpgsqlCommand cmd = new NpgsqlCommand(updateMainImageIdSql, connection))
                                {
                                    cmd.Transaction = transaction;
                                    cmd.Parameters.AddWithValue("@imageId", imageId);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSideProjectImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image from the side project.", ex);
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
                                            BLOG POST IMAGE CRUD
            **********************************************************************************************
        */
        // FIXME add BLOGPOST Main Image checker and if statement checks for Main Image
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

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertBlogPostImageSql = "INSERT INTO blogpost_images (blogpost_id, image_id) VALUES (@blogPostId, @imageId);";
            string updateBlogPostMainImageSql = "UPDATE blogposts SET main_image_id = @imageId WHERE id = @blogPostId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);
                                cmdInsertImage.Transaction = transaction;
                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertBlogPostImage = new NpgsqlCommand(insertBlogPostImageSql, connection))
                            {
                                cmdInsertBlogPostImage.Parameters.AddWithValue("@blogPostId", blogPostId);
                                cmdInsertBlogPostImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertBlogPostImage.Transaction = transaction;
                                cmdInsertBlogPostImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateBlogPost = new NpgsqlCommand(updateBlogPostMainImageSql, connection))
                            {
                                cmdUpdateBlogPost.Parameters.AddWithValue("@blogPostId", blogPostId);
                                cmdUpdateBlogPost.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateBlogPost.Transaction = transaction;
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

            string sql = "SELECT i.id, i.name, i.url, i.type " +
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

            string sql = "SELECT i.id, i.name, i.url, i.type " +
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
                                    "SET name = @name, url = @url, type = @type " +
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
                        cmd.Parameters.AddWithValue("@type", image.Type);

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

            string updateImageIdSql = "UPDATE blogposts SET main_image_id = NULL WHERE main_image_id = @imageId;";
            string deleteBlogPostImageSql = "DELETE FROM blogpost_images WHERE blogpost_id = @blogPostId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteBlogPostImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@blogPostId", blogPostId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by blog post ID.", ex);
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

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertWebsiteImageSql = "INSERT INTO website_images (website_id, image_id) VALUES (@websiteId, @imageId);";
            string updateWebsiteLogoIdSql = "UPDATE websites SET logo_id = @imageId WHERE id = @websiteId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);
                                cmdInsertImage.Transaction = transaction;
                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertWebsiteImage = new NpgsqlCommand(insertWebsiteImageSql, connection))
                            {
                                cmdInsertWebsiteImage.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdInsertWebsiteImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertWebsiteImage.Transaction = transaction;
                                cmdInsertWebsiteImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateWebsiteLogoId = new NpgsqlCommand(updateWebsiteLogoIdSql, connection))
                            {
                                cmdUpdateWebsiteLogoId.Parameters.AddWithValue("@websiteId", websiteId);
                                cmdUpdateWebsiteLogoId.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateWebsiteLogoId.Transaction = transaction;
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

        public Image GetImageByWebsiteId(int websiteId, int imageId)
        {
            if (websiteId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("WebsiteId and ImageId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN website_images wi ON i.id = wi.image_id " +
                         "WHERE wi.website_id = @websiteId AND wi.image_id = @imageId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);
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
                throw new DaoException("An error occurred while retrieving the image by website ID and image ID.", ex);
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
                                    "SET name = @name, url = @url, type = @type " +
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
                        cmd.Parameters.AddWithValue("@type", image.Type);

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

            string updateWebsiteIdSql = "UPDATE websites SET logo_id = NULL WHERE logo_id = @imageId";
            string deleteWebsiteImageSql = "DELETE FROM website_images WHERE website_id = @websiteId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateWebsiteIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteWebsiteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@websiteId", websiteId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by website ID.", ex);
                        }
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

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
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
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);

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

            string sql = "SELECT i.id, i.name, i.url, i.type " +
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
                                    "SET name = @name, url = @url, type = @type " +
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
                        cmd.Parameters.AddWithValue("@type", image.Type);

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

            string updateSkillIconIdSql = "UPDATE skills SET icon_id = NULL WHERE icon_id = @imageId;";
            string deleteSkillImageSql = "DELETE FROM skill_images WHERE skill_id = @skillId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateSkillIconIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteSkillImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@skillId", skillId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by skill ID.", ex);
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

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
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
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);

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

            string sql = "SELECT i.id, i.name, i.url, i.type FROM images i " +
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
                                    "SET name = @name, url = @url, type = @type " +
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
                        cmd.Parameters.AddWithValue("@type", image.Type);

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

            string updateGoalIconIdSql = "UPDATE goals SET icon_id = NULL WHERE icon_id = @imageId;";
            string deleteGoalImageSql = "DELETE FROM goal_images WHERE goal_id = @goalId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateGoalIconIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteGoalImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@goalId", goalId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by goal ID.", ex);
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

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
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
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);

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

            string sql = "SELECT i.id, i.name, i.url, i.type " +
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
                                    "SET name = @name, url = @url, type = @type " +
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
                        cmd.Parameters.AddWithValue("@type", image.Type);

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

            string updateContributorImageIdSql = "UPDATE contributors SET contributor_image_id = NULL WHERE contributor_image_id = @imageId;";
            string deleteContributorImageSql = "DELETE FROM contributor_images WHERE contributor_id = @contributorId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateContributorImageIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteContributorImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@contributorId", contributorId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by contributor ID.", ex);
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

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
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
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);

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

            string sql = "SELECT i.id, i.name, i.url, i.type " +
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
                                    "SET name = @name, url = @url, type = @type " +
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
                        cmd.Parameters.AddWithValue("@type", image.Type);

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

            string updateApiServiceLogoIdSql = "UPDATE apis_and_services SET logo_id = NULL WHERE logo_id = @imageId;";
            string deleteApiServiceImageSql = "DELETE FROM api_service_images WHERE apiservice_id = @apiServiceId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateApiServiceLogoIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteApiServiceImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@apiServiceId", apiServiceId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by API/Service ID.", ex);
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

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
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
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);

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

            string sql = "SELECT i.id, i.name, i.url, i.type " +
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
                                    "SET name = @name, url = @url, type = @type " +
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
                        cmd.Parameters.AddWithValue("@type", image.Type);

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

            string updateDependencyLibraryLogoIdSql = "UPDATE dependencies_and_libraries SET logo_id = NULL WHERE logo_id = @imageId;";
            string deleteLibraryImageSql = "DELETE FROM dependency_library_images WHERE dependencylibrary_id = @dependencyLibraryId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateDependencyLibraryLogoIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteLibraryImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@dependencyLibraryId", dependencyLibraryId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by dependency library ID.", ex);
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
                                            EXPERIENCE IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByExperienceId(int experienceId, Image image)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("ExperienceId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Type))
            {
                throw new ArgumentException("Image Type cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertExperienceImageSql = "INSERT INTO experience_images (experience_id, image_id) VALUES (@experienceId, @imageId);";

            string updateExperienceImageIdSql = null;

            switch (image.Type)
            {
                case MainImage:
                    updateExperienceImageIdSql = "UPDATE experiences SET main_image_id = @imageId WHERE id = @experienceId;";
                    break;
                case Logo:
                    updateExperienceImageIdSql = "UPDATE experiences SET company_logo_id = @imageId WHERE id = @experienceId;";
                    break;
                case AdditionalImage:
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
                            Image existingMainImage = null;

                            if (image.Type == MainImage)
                            {
                                existingMainImage = GetMainImageOrCompanyLogoByExperienceId(experienceId, MainImage);

                                if (existingMainImage != null)
                                {
                                    image.Type = AdditionalImage;
                                }
                            }

                            Image existingLogo = null;

                            if (image.Type == Logo)
                            {
                                existingLogo = GetMainImageOrCompanyLogoByExperienceId(experienceId, Logo);

                                if (existingLogo != null)
                                {
                                    image.Type = AdditionalImage;
                                }
                            }

                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);
                                cmdInsertImage.Transaction = transaction;
                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertExperienceImage = new NpgsqlCommand(insertExperienceImageSql, connection))
                            {
                                cmdInsertExperienceImage.Parameters.AddWithValue("@experienceId", experienceId);
                                cmdInsertExperienceImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertExperienceImage.Transaction = transaction;
                                cmdInsertExperienceImage.ExecuteNonQuery();
                            }

                            if ((image.Type == MainImage && existingMainImage == null) || (image.Type == Logo && existingLogo == null))
                            {
                                using (NpgsqlCommand cmdUpdateExperience = new NpgsqlCommand(updateExperienceImageIdSql, connection))
                                {
                                    cmdUpdateExperience.Parameters.AddWithValue("@experienceId", experienceId);
                                    cmdUpdateExperience.Parameters.AddWithValue("@imageId", imageId);
                                    cmdUpdateExperience.Transaction = transaction;
                                    cmdUpdateExperience.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image or logo by experience ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetMainImageOrCompanyLogoByExperienceId(int experienceId, string imageType)
        {
            if (experienceId <= 0)
            {
                throw new ArgumentException("ExperienceId must be greater than zero.");
            }

            if (imageType != MainImage && imageType != Logo)
            {
                throw new ArgumentException("Image Type must be 'main image' or 'logo'.");
            }

            Image mainImageOrLogo = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                        "FROM images i " +
                        "JOIN experience_images ei ON i.id = ei.image_id " +
                        "WHERE ei.experience_id = @experienceId AND i.type = @imageType;";
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);
                        cmd.Parameters.AddWithValue("@imageType", imageType);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                mainImageOrLogo = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Main Image or Company Logo by Experience ID.", ex);
            }

            return mainImageOrLogo;
        }

        public Image GetImageByExperienceId(int experienceId, int imageId)
        {
            if (experienceId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("ExperienceId and ImageId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN experience_images ei ON i.id = ei.image_id " +
                         "WHERE ei.experience_id = @experienceId AND i.id = @imageId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);
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
                throw new DaoException("An error occurred while retrieving the image by experience ID and image ID.", ex);
            }

            return image;
        }

        public Image UpdateImageByExperienceId(int experienceId, int imageId, Image image)
        {
            if (experienceId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("ExperienceId and imageId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url, type = @type " +
                                    "FROM experience_images " +
                                    "WHERE images.id = experience_images.image_id AND experience_images.experience_id = @experienceId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);
                        cmd.Parameters.AddWithValue("@type", image.Type);

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
                throw new DaoException("An error occurred while updating the image by experience ID.", ex);
            }

            return null;
        }

        public Image UpdateMainImageOrLogoByExperienceId(int experienceId, int imageId, Image image)
        {
            if (image.Type != MainImage || image.Type != Logo)
            {
                throw new ArgumentException("The image provided is not a main image or company logo. Please provide a main image or company logo.");
            }
            else
            {
                DeleteImageByExperienceId(experienceId, imageId);
                CreateImageByExperienceId(experienceId, image);
            }

            return image;
        }

        public int DeleteImageByExperienceId(int experienceId, int imageId)
        {
            if (experienceId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("ExperienceId and imageId must be greater than zero.");
            }

            // UpdateExperienceImageIdSql only runs if the image is the Main Image or Logo
            string updateExperienceImageIdSql = null;

            string deleteExperienceImageSql = "DELETE FROM experience_images WHERE experience_id = @experienceId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            Image image = GetImageByImageId(imageId);

            switch (image.Type)
            {
                case MainImage:
                    updateExperienceImageIdSql = "UPDATE experiences SET main_image_id = NULL WHERE main_image_id = @imageId;";
                    break;
                case Logo:
                    updateExperienceImageIdSql = "UPDATE experiences SET company_logo_id = NULL WHERE company_logo_id = @imageId;";
                    break;
                case AdditionalImage:
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
                            int rowsAffected;

                            if (image.Type == MainImage || image.Type == Logo)
                            {
                                using (NpgsqlCommand cmd = new NpgsqlCommand(updateExperienceImageIdSql, connection))
                                {
                                    cmd.Transaction = transaction;
                                    cmd.Parameters.AddWithValue("@imageId", imageId);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteExperienceImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@experienceId", experienceId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by experience ID.", ex);
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
                                            CREDENTIAL IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByCredentialId(int credentialId, Image image)
        {
            if (credentialId <= 0)
            {
                throw new ArgumentException("CredentialId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Type))
            {
                throw new ArgumentException("Image Type cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertCredentialImageSql = "INSERT INTO credential_images (credential_id, image_id) VALUES (@credentialId, @imageId);";

            string updateCredentialImageIdSql = null;

            switch (image.Type)
            {
                case MainImage:
                    updateCredentialImageIdSql = "UPDATE credentials SET main_image_id = @imageId WHERE id = @credentialId;";
                    break;
                case Logo:
                    updateCredentialImageIdSql = "UPDATE credentials SET organization_logo_id = @imageId WHERE id = @credentialId;";
                    break;
                default:
                    throw new ArgumentException("Invalid image type.");
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
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);
                                cmdInsertImage.Transaction = transaction;
                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertCredentialImage = new NpgsqlCommand(insertCredentialImageSql, connection))
                            {
                                cmdInsertCredentialImage.Parameters.AddWithValue("@credentialId", credentialId);
                                cmdInsertCredentialImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertCredentialImage.Transaction = transaction;
                                cmdInsertCredentialImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateCredentialImageId = new NpgsqlCommand(updateCredentialImageIdSql, connection))
                            {
                                cmdUpdateCredentialImageId.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateCredentialImageId.Parameters.AddWithValue("@credentialId", credentialId);
                                cmdUpdateCredentialImageId.Transaction = transaction;
                                cmdUpdateCredentialImageId.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image by credential ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetMainImageOrOrganizationLogoByCredentialId(int credentialId, string imageType)
        {
            if (credentialId <= 0)
            {
                throw new ArgumentException("CredentialId must be greater than zero.");
            }

            if (imageType != MainImage && imageType != Logo)
            {
                throw new ArgumentException("Image Type must be 'main image' or 'logo'.");
            }

            Image mainImageOrLogo = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN credential_images ci ON i.id = ci.image_id " +
                         "WHERE ci.credential_id = @credentialId AND i.type = @imageType;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);
                        cmd.Parameters.AddWithValue("@imageType", imageType);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                mainImageOrLogo = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Main Image or Organization Logo by Credential ID.", ex);
            }

            return mainImageOrLogo;
        }

        public Image UpdateMainImageOrOrganizationLogoByCredentialId(int credentialId, int imageId, Image image)
        {
            if (image.Type != MainImage || image.Type != Logo)
            {
                throw new ArgumentException("The image provided is not a main image or organization logo. Please provide a main image or organization logo.");
            }
            else
            {
                DeleteImageByCredentialId(credentialId, imageId);
                CreateImageByCredentialId(credentialId, image);
            }

            return image;
        }

        public int DeleteImageByCredentialId(int credentialId, int imageId)
        {
            if (credentialId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("CredentialId and imageId must be greater than zero.");
            }

            // UpdateCredentialImageIdSql only runs if the image is the Main Image or Logo
            string updateCredentialImageIdSql = null;

            string deleteCredentialImageSql = "DELETE FROM credential_images WHERE credential_id = @credentialId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            Image image = GetImageByImageId(imageId);

            switch (image.Type)
            {
                case MainImage:
                    updateCredentialImageIdSql = "UPDATE credentials SET main_image_id = NULL WHERE main_image_id = @imageId;";
                    break;
                case Logo:
                    updateCredentialImageIdSql = "UPDATE credentials SET organization_logo_id = NULL WHERE organization_logo_id = @imageId;";
                    break;
                default:
                    throw new ArgumentException("Invalid image type.");

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
                            int rowsAffected = 0;

                            if (image.Type == MainImage || image.Type == Logo)
                            {
                                using (NpgsqlCommand cmd = new NpgsqlCommand(updateCredentialImageIdSql, connection))
                                {
                                    cmd.Transaction = transaction;
                                    cmd.Parameters.AddWithValue("@imageId", imageId);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteCredentialImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@credentialId", credentialId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by credential ID.", ex);
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
                                            EDUCATION IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByEducationId(int educationId, Image image)
        {
            if (educationId <= 0)
            {
                throw new ArgumentException("EducationId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Type))
            {
                throw new ArgumentException("Image Type cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertEducationImageSql = "INSERT INTO education_images (education_id, image_id) VALUES (@educationId, @imageId);";

            string updateEducationImageIdSql = null;

            switch (image.Type)
            {
                case MainImage:
                    updateEducationImageIdSql = "UPDATE educations SET main_image_id = @imageId WHERE id = @educationId;";
                    break;
                case Logo:
                    updateEducationImageIdSql = "UPDATE educations SET institution_logo_id = @imageId WHERE id = @educationId;";
                    break;
                case AdditionalImage:
                    break;
                default:
                    throw new ArgumentException("Invalid image type.");
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
                            Image existingMainImage = null;

                            if (image.Type == MainImage)
                            {
                                existingMainImage = GetMainImageOrInstitutionLogoByEducationId(educationId, MainImage);

                                if (existingMainImage != null)
                                {
                                    image.Type = AdditionalImage;
                                }
                            }

                            Image existingLogo = null;

                            if (image.Type == Logo)
                            {
                                existingLogo = GetMainImageOrInstitutionLogoByEducationId(educationId, Logo);

                                if (existingLogo != null)
                                {
                                    image.Type = AdditionalImage;
                                }
                            }

                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);
                                cmdInsertImage.Transaction = transaction;
                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertEducationImage = new NpgsqlCommand(insertEducationImageSql, connection))
                            {
                                cmdInsertEducationImage.Parameters.AddWithValue("@educationId", educationId);
                                cmdInsertEducationImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertEducationImage.Transaction = transaction;
                                cmdInsertEducationImage.ExecuteNonQuery();
                            }

                            if ((image.Type == MainImage && existingMainImage == null) || (image.Type == Logo && existingLogo == null))
                            {
                                using (NpgsqlCommand cmdUpdateEducationImageId = new NpgsqlCommand(updateEducationImageIdSql, connection))
                                {
                                    cmdUpdateEducationImageId.Parameters.AddWithValue("@imageId", imageId);
                                    cmdUpdateEducationImageId.Parameters.AddWithValue("@educationId", educationId);
                                    cmdUpdateEducationImageId.Transaction = transaction;
                                    cmdUpdateEducationImageId.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image by education ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetMainImageOrInstitutionLogoByEducationId(int educationId, string imageType)
        {
            if (educationId <= 0)
            {
                throw new ArgumentException("EducationId must be greater than zero.");
            }

            if (imageType != MainImage && imageType != Logo)
            {
                throw new ArgumentException("Image Type must be 'main image' or 'logo'.");
            }

            Image mainImageOrLogo = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN education_images ei ON i.id = ei.image_id " +
                         "WHERE ei.education_id = @educationId AND i.type = @imageType;";
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);
                        cmd.Parameters.AddWithValue("@imageType", imageType);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                mainImageOrLogo = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Main Image or Institution Logo by Education ID.", ex);
            }

            return mainImageOrLogo;
        }

        public Image GetImageByEducationId(int educationId, int imageId)
        {
            if (educationId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("EducationId and ImageId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN education_images ei ON i.id = ei.image_id " +
                         "WHERE ei.education_id = @educationId AND i.id = @imageId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);
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
                throw new DaoException("An error occurred while retrieving the image by education ID and image ID.", ex);
            }

            return image;
        }

        public List<Image> GetAdditionalImagesByEducationId(int educationId)
        {
            if (educationId <= 0)
            {
                throw new ArgumentException("EducationId must be greater than zero.");
            }

            List<Image> additionalImages = new List<Image>();

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN education_images ei ON i.id = ei.image_id " +
                         "WHERE ei.education_id = @educationId AND i.type = @imageType;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);
                        cmd.Parameters.AddWithValue("@imageType", AdditionalImage);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Image image = MapRowToImage(reader);
                                additionalImages.Add(image);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the additional images by education ID.", ex);
            }

            return additionalImages;
        }

        public Image UpdateImageByEducationId(int educationId, int imageId, Image image)
        {
            if (educationId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("EducationId and imageId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url, type = @type " +
                                    "FROM education_images " +
                                    "WHERE images.id = education_images.image_id AND education_images.education_id = @educationId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);
                        cmd.Parameters.AddWithValue("@type", image.Type);

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
                throw new DaoException("An error occurred while updating the image by education ID.", ex);
            }

            return null;
        }

        public Image UpdateMainImageOrLogoByEducationId(int educationId, int imageId, Image image)
        {
            if (image.Type != MainImage && image.Type != Logo)
            {
                throw new ArgumentException("The image provided is not a main image or institution logo. Please provide a main image or institution logo.");
            }
            else
            {
                DeleteImageByEducationId(educationId, imageId);
                CreateImageByEducationId(educationId, image);
            }

            return image;
        }

        public int DeleteImageByEducationId(int educationId, int imageId)
        {
            if (educationId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("EducationId and imageId must be greater than zero.");
            }

            // UpdateEducationImageIdSql only runs if the image is the Main Image or Logo
            string updateEducationImageIdSql = null;

            string deleteEducationImageSql = "DELETE FROM education_images WHERE education_id = @educationId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            Image image = GetImageByImageId(imageId);

            switch (image.Type)
            {
                case MainImage:
                    updateEducationImageIdSql = "UPDATE educations SET main_image_id = NULL WHERE main_image_id = @imageId;";
                    break;
                case Logo:
                    updateEducationImageIdSql = "UPDATE educations SET institution_logo_id = NULL WHERE institution_logo_id = @imageId;";
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
                            int rowsAffected;

                            if (image.Type == MainImage || image.Type == Logo)
                            {
                                using (NpgsqlCommand cmd = new NpgsqlCommand(updateEducationImageIdSql, connection))
                                {
                                    cmd.Transaction = transaction;
                                    cmd.Parameters.AddWithValue("@imageId", imageId);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteEducationImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@educationId", educationId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by education ID.", ex);
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
                                        OPEN SOURCE CONTRIBUTION IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByOpenSourceContributionId(int contributionId, Image image)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("ContributionId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Type))
            {
                throw new ArgumentException("Image Type cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertContributionImageSql = "INSERT INTO open_source_contribution_images (open_source_contribution_id, image_id) VALUES (@contributionId, @imageId);";

            string updateContributionImageIdSql = null;

            switch (image.Type)
            {
                case MainImage:
                    updateContributionImageIdSql = "UPDATE open_source_contributions SET main_image_id = @imageId WHERE id = @contributionId;";
                    break;
                case Logo:
                    updateContributionImageIdSql = "UPDATE open_source_contributions SET organization_logo_id = @imageId WHERE id = @contributionId;";
                    break;
                case AdditionalImage:
                    break;
                default:
                    throw new ArgumentException("Invalid image type.");
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
                            Image existingMainImage = null;

                            if (image.Type == MainImage)
                            {
                                existingMainImage = GetMainImageOrOrganizationLogoByOpenSourceContributionId(contributionId, MainImage);

                                if (existingMainImage != null)
                                {
                                    image.Type = AdditionalImage;
                                }
                            }

                            Image existingLogo = null;

                            if (image.Type == Logo)
                            {
                                existingLogo = GetMainImageOrOrganizationLogoByOpenSourceContributionId(contributionId, Logo);

                                if (existingLogo != null)
                                {
                                    image.Type = AdditionalImage;
                                }
                            }

                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);
                                cmdInsertImage.Transaction = transaction;
                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertContributionImage = new NpgsqlCommand(insertContributionImageSql, connection))
                            {
                                cmdInsertContributionImage.Parameters.AddWithValue("@contributionId", contributionId);
                                cmdInsertContributionImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertContributionImage.Transaction = transaction;
                                cmdInsertContributionImage.ExecuteNonQuery();
                            }

                            if ((image.Type == MainImage && existingMainImage == null) || (image.Type == Logo && existingLogo == null))
                            {
                                using (NpgsqlCommand cmdUpdateContributionImageId = new NpgsqlCommand(updateContributionImageIdSql, connection))
                                {
                                    cmdUpdateContributionImageId.Parameters.AddWithValue("@imageId", imageId);
                                    cmdUpdateContributionImageId.Parameters.AddWithValue("@contributionId", contributionId);
                                    cmdUpdateContributionImageId.Transaction = transaction;
                                    cmdUpdateContributionImageId.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image by open source contribution ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetMainImageOrOrganizationLogoByOpenSourceContributionId(int contributionId, string imageType)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("Open Source ContributionId must be greater than zero.");
            }

            if (imageType != MainImage && imageType != Logo)
            {
                throw new ArgumentException("Image Type must be 'main image' or 'logo'.");
            }

            Image mainImageOrLogo = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN open_source_contribution_images oci ON i.id = oci.image_id " +
                         "WHERE oci.open_source_contribution_id = @contributionId AND i.type = @imageType;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
                        cmd.Parameters.AddWithValue("@imageType", imageType);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                mainImageOrLogo = MapRowToImage(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Main Image or Organization Logo by Open Source Contribution ID.", ex);
            }

            return mainImageOrLogo;
        }

        public Image GetImageByOpenSourceContributionId(int contributionId, int imageId)
        {
            if (contributionId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("Open Source ContributionId and ImageId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN open_source_contribution_images oci ON i.id = oci.image_id " +
                         "WHERE oci.open_source_contribution_id = @contributionId AND i.id = @imageId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
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
                throw new DaoException("An error occurred while retrieving the image by open source contribution ID and image ID.", ex);
            }

            return image;
        }

        public List<Image> GetAdditionalImagesByOpenSourceContributionId(int contributionId)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("Open Source ContributionId must be greater than zero.");
            }

            List<Image> additionalImages = new List<Image>();

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN open_source_contribution_images oci ON i.id = oci.image_id " +
                         "WHERE oci.open_source_contribution_id = @contributionId AND i.type = @imageType;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
                        cmd.Parameters.AddWithValue("@imageType", AdditionalImage);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Image image = MapRowToImage(reader);
                                additionalImages.Add(image);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the additional images by open source contribution ID.", ex);
            }

            return additionalImages;
        }

        public Image UpdateImageByOpenSourceContributionId(int contributionId, int imageId, Image image)
        {
            if (contributionId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("Open Source ContributionId and imageId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url, type = @type " +
                                    "FROM open_source_contribution_images " +
                                    "WHERE images.id = open_source_contribution_images.image_id AND open_source_contribution_images.open_source_contribution_id = @contributionId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);
                        cmd.Parameters.AddWithValue("@type", image.Type);

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
                throw new DaoException("An error occurred while updating the image by open source contribution ID.", ex);
            }

            return null;
        }

        public Image UpdateMainImageOrLogoByOpenSourceContributionId(int contributionId, int imageId, Image image)
        {
            if (image.Type != MainImage && image.Type != Logo)
            {
                throw new ArgumentException("The image provided is not a main image or organization logo. Please provide a main image or organization logo.");
            }
            else
            {
                DeleteImageByOpenSourceContributionId(contributionId, imageId);
                CreateImageByOpenSourceContributionId(contributionId, image);
            }

            return image;
        }

        public int DeleteImageByOpenSourceContributionId(int openSourceContributionId, int imageId)
        {
            if (openSourceContributionId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("OpenSourceContributionId and imageId must be greater than zero.");
            }

            // UpdateOpenSourceContributionImageIdSql only runs if the image is the Main Image or Logo
            string updateOpenSourceContributionImageIdSql = null;

            string deleteOpenSourceContributionImageSql = "DELETE FROM open_source_contribution_images WHERE open_source_contribution_id = @openSourceContributionId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

            Image image = GetImageByImageId(imageId);

            switch (image.Type)
            {
                case MainImage:
                    updateOpenSourceContributionImageIdSql = "UPDATE open_source_contributions SET main_image_id = NULL WHERE main_image_id = @imageId;";
                    break;
                case Logo:
                    updateOpenSourceContributionImageIdSql = "UPDATE open_source_contributions SET organization_logo_id = NULL WHERE organization_logo_id = @imageId;";
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
                            int rowsAffected;

                            if (image.Type == MainImage || image.Type == Logo)
                            {
                                using (NpgsqlCommand cmd = new NpgsqlCommand(updateOpenSourceContributionImageIdSql, connection))
                                {
                                    cmd.Transaction = transaction;
                                    cmd.Parameters.AddWithValue("@imageId", imageId);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteOpenSourceContributionImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@openSourceContributionId", openSourceContributionId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by open source contribution ID.", ex);
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
                                            VOLUNTEER WORK IMAGE CRUD
            **********************************************************************************************
        */

        /*  
            **********************************************************************************************
                                            ACHIEVEMENT IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByAchievementId(int achievementId, Image image)
        {
            if (achievementId <= 0)
            {
                throw new ArgumentException("AchievementId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertAchievementImageSql = "INSERT INTO achievement_images (achievement_id, image_id) VALUES (@achievementId, @imageId);";
            string updateAchievementIconIdSql = "UPDATE achievements SET icon_id = @imageId WHERE id = @achievementId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);
                                cmdInsertImage.Transaction = transaction;
                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertAchievementImage = new NpgsqlCommand(insertAchievementImageSql, connection))
                            {
                                cmdInsertAchievementImage.Parameters.AddWithValue("@achievementId", achievementId);
                                cmdInsertAchievementImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertAchievementImage.Transaction = transaction;
                                cmdInsertAchievementImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateAchievementIconId = new NpgsqlCommand(updateAchievementIconIdSql, connection))
                            {
                                cmdUpdateAchievementIconId.Parameters.AddWithValue("@achievementId", achievementId);
                                cmdUpdateAchievementIconId.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateAchievementIconId.Transaction = transaction;
                                cmdUpdateAchievementIconId.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image for the achievement.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetImageByAchievementId(int achievementId)
        {
            if (achievementId <= 0)
            {
                throw new ArgumentException("AchievementId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN achievement_images ai ON i.id = ai.image_id " +
                         "WHERE ai.achievement_id = @achievementId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);

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
                throw new DaoException("An error occurred while retrieving the image by achievement ID.", ex);
            }

            return image;
        }

        public Image UpdateImageByAchievementId(int achievementId, int imageId, Image image)
        {
            if (achievementId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("AchievementId and imageId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url, type = @type " +
                                    "FROM achievement_images " +
                                    "WHERE images.id = achievement_images.image_id AND achievement_images.achievement_id = @achievementId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@achievementId", achievementId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);
                        cmd.Parameters.AddWithValue("@type", image.Type);

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
                throw new DaoException("An error occurred while updating the image by achievement ID.", ex);
            }

            return null;
        }

        public int DeleteImageByAchievementId(int achievementId, int imageId)
        {
            if (achievementId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("AchievementId and imageId must be greater than zero.");
            }

            string updateAchievementIconIdSql = "UPDATE achievements SET icon_id = NULL WHERE icon_id = @imageId;";
            string deleteAchievementImageSql = "DELETE FROM achievement_images WHERE achievement_id = @achievementId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateAchievementIconIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteAchievementImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@achievementId", achievementId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by achievement ID.", ex);
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
                                            HOBBY IMAGE CRUD
            **********************************************************************************************
        */

        public Image CreateImageByHobbyId(int hobbyId, Image image)
        {
            if (hobbyId <= 0)
            {
                throw new ArgumentException("HobbyId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(image.Name))
            {
                throw new ArgumentException("Image name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(image.Url))
            {
                throw new ArgumentException("Image URL cannot be null or empty.");
            }

            string insertImageSql = "INSERT INTO images (name, url, type) VALUES (@name, @url, @type) RETURNING id;";
            string insertHobbyImageSql = "INSERT INTO hobby_images (hobby_id, image_id) VALUES (@hobbyId, @imageId);";
            string updateHobbyIconIdSql = "UPDATE hobbies SET icon_id = @imageId WHERE id = @hobbyId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int imageId;

                            using (NpgsqlCommand cmdInsertImage = new NpgsqlCommand(insertImageSql, connection))
                            {
                                cmdInsertImage.Parameters.AddWithValue("@name", image.Name);
                                cmdInsertImage.Parameters.AddWithValue("@url", image.Url);
                                cmdInsertImage.Parameters.AddWithValue("@type", image.Type);
                                cmdInsertImage.Transaction = transaction;
                                imageId = Convert.ToInt32(cmdInsertImage.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertHobbyImage = new NpgsqlCommand(insertHobbyImageSql, connection))
                            {
                                cmdInsertHobbyImage.Parameters.AddWithValue("@hobbyId", hobbyId);
                                cmdInsertHobbyImage.Parameters.AddWithValue("@imageId", imageId);
                                cmdInsertHobbyImage.Transaction = transaction;
                                cmdInsertHobbyImage.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmdUpdateHobbyIconId = new NpgsqlCommand(updateHobbyIconIdSql, connection))
                            {
                                cmdUpdateHobbyIconId.Parameters.AddWithValue("@hobbyId", hobbyId);
                                cmdUpdateHobbyIconId.Parameters.AddWithValue("@imageId", imageId);
                                cmdUpdateHobbyIconId.Transaction = transaction;
                                cmdUpdateHobbyIconId.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            image.Id = imageId;

                            return image;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the image for the hobby.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public Image GetImageByHobbyId(int hobbyId)
        {
            if (hobbyId <= 0)
            {
                throw new ArgumentException("HobbyId must be greater than zero.");
            }

            Image image = null;

            string sql = "SELECT i.id, i.name, i.url, i.type " +
                         "FROM images i " +
                         "JOIN hobby_images hi ON i.id = hi.image_id " +
                         "WHERE hi.hobby_id = @hobbyId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@hobbyId", hobbyId);

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
                throw new DaoException("An error occurred while retrieving the image by hobby ID.", ex);
            }

            return image;
        }

        public Image UpdateImageByHobbyId(int hobbyId, int imageId, Image image)
        {
            if (hobbyId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("HobbyId and imageId must be greater than zero.");
            }

            string updateImageSql = "UPDATE images " +
                                    "SET name = @name, url = @url, type = @type " +
                                    "FROM hobby_images " +
                                    "WHERE images.id = hobby_images.image_id AND hobby_images.hobby_id = @hobbyId " +
                                    "AND images.id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateImageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@hobbyId", hobbyId);
                        cmd.Parameters.AddWithValue("@imageId", imageId);
                        cmd.Parameters.AddWithValue("@name", image.Name);
                        cmd.Parameters.AddWithValue("@url", image.Url);
                        cmd.Parameters.AddWithValue("@type", image.Type);

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
                throw new DaoException("An error occurred while updating the image by hobby ID.", ex);
            }

            return null;
        }

        public int DeleteImageByHobbyId(int hobbyId, int imageId)
        {
            if (hobbyId <= 0 || imageId <= 0)
            {
                throw new ArgumentException("HobbyId and imageId must be greater than zero.");
            }

            string updateHobbyIconIdSql = "UPDATE hobbies SET icon_id = NULL WHERE icon_id = @imageId;";
            string deleteHobbyImageSql = "DELETE FROM hobby_images WHERE hobby_id = @hobbyId AND image_id = @imageId;";
            string deleteImageSql = "DELETE FROM images WHERE id = @imageId;";

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

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateHobbyIconIdSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteHobbyImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@hobbyId", hobbyId);
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteImageSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@imageId", imageId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the image by hobby ID.", ex);
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

        public Image GetImageByImageId(int imageId)
        {
            if (imageId <= 0)
            {
                throw new ArgumentException("ImageId must be greater than zero.");
            }

            string sql = "SELECT id, name, url, type FROM images WHERE id = @imageId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@imageId", imageId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToImage(reader);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving image by image id: " + ex.Message);
                return null;
            }
        }

        public int? GetImageIdByWebsiteId(int websiteId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT logo_id FROM websites WHERE id = @websiteId;";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@websiteId", websiteId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving image ID: " + ex.Message);
                return null;
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
                Url = Convert.ToString(reader["url"]),
                Type = Convert.ToString(reader["type"])
            };
        }
    }
}
