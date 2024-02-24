using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class BlogPostsPostgresDao : IBlogPostsDao 
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;

        public BlogPostsPostgresDao(string dbConnectionString, IImageDao imageDao) {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
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

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        BlogPost blogPost = MapRowToBlogPost(reader);
                        blogPosts.Add(blogPost);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }

            return blogPosts;
        }

        public BlogPost GetBlogPostById(int blogPostId) 
        {
            BlogPost blogPost = null;

            string sql = "SELECT id, name, author, description, content, " +
                "main_image_id, created_at, updated_at " +
                "FROM blogposts WHERE id = @blogPostId";

            try 
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    
                    cmd.Parameters.AddWithValue("@blogPostId", blogPostId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        blogPost = MapRowToBlogPost(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }

            return blogPost;
        }

        public BlogPost AddBlogPost(BlogPost blogPost)
        {
            BlogPost newBlogPost = null;

            string sql = "INSERT into blogposts (name, author, description, content, " +
                         "main_image_id) " +
                         "VALUES (@name, @author, @description, @content, @main_image_id) " +
                         "RETURNING id;";

            int newBlogPostId = 0;    

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@name", blogPost.Name);
                    cmd.Parameters.AddWithValue("@author", blogPost.Author);
                    cmd.Parameters.AddWithValue("@description", blogPost.Description);
                    cmd.Parameters.AddWithValue("@content", blogPost.Content);
                    cmd.Parameters.AddWithValue("@main_image_id", blogPost.MainImageId);

                    newBlogPostId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                newBlogPost = GetBlogPostById(newBlogPostId);
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }
            return newBlogPost;
        }

        public BlogPost UpdateBlogPost(BlogPost blogPost, int blogPostId)
        {
            string sql = "UPDATE blogposts SET name = @name, author = @author, " +
                         "description = @description, content = @content, main_image_id = @main_image_id " +
                         "WHERE id = @blogPostId;";

             try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@blogPostId", blogPostId); 
                    cmd.Parameters.AddWithValue("@name", blogPost.Name);
                    cmd.Parameters.AddWithValue("@author", blogPost.Author);
                    cmd.Parameters.AddWithValue("@description", blogPost.Description);
                    cmd.Parameters.AddWithValue("@content", blogPost.Content);
                    cmd.Parameters.AddWithValue("@main_image_id", blogPost.MainImageId);

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
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the blog post.", ex);
            }    
        }

        public int DeleteBlogPostByBlogPostId(int blogPostId)
        {
            int numberOfRowsAffected = 0;

            string sql = "DELETE FROM blogposts WHERE id = @blogPostId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@blogPostId", blogPostId);

                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the blog post.", ex);
            }

            return numberOfRowsAffected;
        }

        private BlogPost MapRowToBlogPost(NpgsqlDataReader reader)
        {
            BlogPost blogPost = new BlogPost
            {
                Id = Convert.ToInt32(reader["blogpost_id"]),
                Name = Convert.ToString(reader["blogpost_name"]),
                Author = Convert.ToString(reader["blogpost_author"]),
                Description = Convert.ToString(reader["blogpost_description"]),
                Content = Convert.ToString(reader["blogpost_content"]),
                MainImageId = Convert.ToInt32(reader["main_image_id"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                UpdatedAt = Convert.ToDateTime(reader["updated_at"])
            };

            if (reader["main_image_id"] != DBNull.Value)
            {
                int mainImageId = Convert.ToInt32(reader["main_image_id"]);
                blogPost.MainImage = _imageDao.GetImageByImageIdAndBlogPostId(mainImageId, blogPost.Id);
            }

            return blogPost;
        }

    }
}