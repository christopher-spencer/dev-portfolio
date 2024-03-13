using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class BlogPostPostgresDao : IBlogPostDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;

        public BlogPostPostgresDao(string dbConnectionString, IImageDao imageDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
        }

        /*  
            **********************************************************************************************
                                                    BLOG POST CRUD
            **********************************************************************************************
        */

        public BlogPost CreateBlogPost(BlogPost blogPost)
        {
            if (string.IsNullOrEmpty(blogPost.Name))
            {
                throw new ArgumentException("BlogPost name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(blogPost.Author))
            {
                throw new ArgumentException("BlogPost author cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(blogPost.Description))
            {
                throw new ArgumentException("BlogPost description cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(blogPost.Content))
            {
                throw new ArgumentException("BlogPost content cannot be null or empty.");
            }

            BlogPost newBlogPost = null;

            string sql = "INSERT into blogposts (name, author, description, content) " +
                         "VALUES (@name, @author, @description, @content) " +
                         "RETURNING id;";

            int newBlogPostId = 0;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", blogPost.Name);
                        cmd.Parameters.AddWithValue("@author", blogPost.Author);
                        cmd.Parameters.AddWithValue("@description", blogPost.Description);
                        cmd.Parameters.AddWithValue("@content", blogPost.Content);

                        newBlogPostId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                newBlogPost = GetBlogPost(newBlogPostId);
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the blog post.", ex);
            }

            return newBlogPost;
        }

        public List<BlogPost> GetBlogPosts()
        {
            List<BlogPost> blogPosts = new List<BlogPost>();

            string sql = "SELECT id, name, author, description, " +
                    "content, main_image_id, created_at, updated_at " +
                    "FROM blogposts;";

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
                                BlogPost blogPost = MapRowToBlogPost(reader);
                                blogPosts.Add(blogPost);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the blog posts.", ex);
            }

            return blogPosts;
        }

        public BlogPost GetBlogPost(int blogPostId)
        {
            if (blogPostId <= 0)
            {
                throw new ArgumentException("BlogPostId must be greater than zero.");
            }

            BlogPost blogPost = null;

            string sql = "SELECT id, name, author, description, content, " +
                "main_image_id, created_at, updated_at " +
                "FROM blogposts WHERE id = @blogPostId";

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
                            if (reader.Read())
                            {
                                blogPost = MapRowToBlogPost(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the blog post.", ex);
            }

            return blogPost;
        }

        public BlogPost UpdateBlogPost(BlogPost blogPost, int blogPostId)
        {
            if (string.IsNullOrEmpty(blogPost.Name))
            {
                throw new ArgumentException("BlogPost name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(blogPost.Author))
            {
                throw new ArgumentException("BlogPost author cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(blogPost.Description))
            {
                throw new ArgumentException("BlogPost description cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(blogPost.Content))
            {
                throw new ArgumentException("BlogPost content cannot be null or empty.");
            }

            if (blogPostId <= 0)
            {
                throw new ArgumentException("BlogPostId must be greater than zero.");
            }

            string sql = "UPDATE blogposts SET name = @name, author = @author, " +
                         "description = @description, content = @content " +
                         "WHERE id = @blogPostId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@blogPostId", blogPostId);
                        cmd.Parameters.AddWithValue("@name", blogPost.Name);
                        cmd.Parameters.AddWithValue("@author", blogPost.Author);
                        cmd.Parameters.AddWithValue("@description", blogPost.Description);
                        cmd.Parameters.AddWithValue("@content", blogPost.Content);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return blogPost;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the blog post.", ex);
            }
        }

        public int DeleteBlogPost(int blogPostId)
        {
            if (blogPostId <= 0)
            {
                throw new ArgumentException("BlogPostId must be greater than zero.");
            }

            string sql = "DELETE FROM blogposts WHERE id = @blogPostId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {                    
                        cmd.Parameters.AddWithValue("@blogPostId", blogPostId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the blog post.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                                BLOGPOST MAP ROW
            **********************************************************************************************
        */

        private BlogPost MapRowToBlogPost(NpgsqlDataReader reader)
        {
            BlogPost blogPost = new BlogPost
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Author = Convert.ToString(reader["author"]),
                Description = Convert.ToString(reader["description"]),
                Content = Convert.ToString(reader["content"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                UpdatedAt = Convert.ToDateTime(reader["updated_at"])
            };

            SetBlogPostMainImageIdProperties(reader, blogPost);

            return blogPost;
        }

        private void SetBlogPostMainImageIdProperties(NpgsqlDataReader reader, BlogPost blogPost)
        {
            if (reader["main_image_id"] != DBNull.Value)
            {
                blogPost.MainImageId = Convert.ToInt32(reader["main_image_id"]);
                
                int mainImageId = Convert.ToInt32(reader["main_image_id"]);
                blogPost.MainImage = _imageDao.GetImageByImageIdAndBlogPostId(mainImageId, blogPost.Id);
            }
            else
            {
                blogPost.MainImageId = 0; 
            }
        }

    }
}