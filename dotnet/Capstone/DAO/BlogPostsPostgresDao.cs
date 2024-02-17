using System.Collections.Generic;
using Capstone.DAO.Interfaces;
using Capstone.Models;

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

            

            return blogPosts;
        }

    }
}