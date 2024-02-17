using Capstone.DAO.Interfaces;

namespace Capstone.DAO
{
    public class BlogPostsPostgresDao : IBlogPostsDao 
    {
        private readonly string connectionString;

        public BlogPostsPostgresDao(string dbConnectionString) {
            connectionString = dbConnectionString;
        }

    }
}