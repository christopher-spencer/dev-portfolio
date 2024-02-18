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

        public BlogPostsPostgresDao(string dbConnectionString) {
            connectionString = dbConnectionString;
        }

        public List<BlogPost> GetBlogPosts()
        {
            List<BlogPost> blogPosts = new List<BlogPost>();

            string sql = "SELECT blogpost_id, blogpost_name, blogpost_author, blogpost_description, " +
                "blogpost_content, image_name, image_url, created_at, updated_at " +
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

            string sql = "SELECT blogpost_id, blogpost_name, blogpost_author, blogpost_description, blogpost_content, " +
                "image_name, image_url, created_at, updated_at " +
                "FROM blogposts WHERE blogpost_id = @blogpost_id";

            try 
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    
                    cmd.Parameters.AddWithValue("@blogpost_id", blogPostId);

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

        private BlogPost MapRowToBlogPost(NpgsqlDataReader reader)
        {
            BlogPost blogPost = new BlogPost
            {
                Id = Convert.ToInt32(reader["blogpost_id"]),
                Name = Convert.ToString(reader["blogpost_name"]),
                Author = Convert.ToString(reader["blogpost_author"]),
                Description = Convert.ToString(reader["blogpost_description"]),
                Content = Convert.ToString(reader["blogpost_content"]),
                ImageName = Convert.ToString(reader["image_name"]),
                ImageUrl = Convert.ToString(reader["image_url"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                UpdatedAt = Convert.ToDateTime(reader["updated_at"])
            };

            return blogPost;
        }

    }
}