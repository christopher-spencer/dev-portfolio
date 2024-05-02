using System.Transactions;
using Npgsql;

namespace Capstone.UnitTests.DAO
{
    public abstract class PostgresDaoTestBase
    {
        protected string TestConnectionString = @"Host=localhost;Port=5432;Database=test_dev_portfolio;Username=test_dev_portfolio_appuser;Password=test_password";

        private TransactionScope transaction;

        [TestInitialize]
        public void Initialize()
        {
            transaction = new TransactionScope();

            using (NpgsqlConnection conn = new NpgsqlConnection(TestConnectionString))
            {
                conn.Open();

                // string sql_insert = "INSERT INTO users (username, password_hash, salt, user_role) " +
                //     "VALUES ('notauser', 'jjjjjjjjj', 'kkkkkkkkkk', 'user');";
                    
                // NpgsqlCommand cmd = new NpgsqlCommand(sql_insert, conn);
                // int count = cmd.ExecuteNonQuery();

                // Assert.AreEqual(1, count, "Insert into user failed");

                // Read user-related SQL queries from the test_users.sql file
                string userSqlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\test_database\test_users.sql");
                string userSqlQueries = File.ReadAllText(userSqlFilePath);

                // Split user-related SQL queries by semicolon and execute them one by one
                string[] userQueries = userSqlQueries.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (string query in userQueries)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }

                // Read SQL queries from the test_data.sql file
                string dataSqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\test_database\test_data.sql");
                string sqlQueries = File.ReadAllText(dataSqlFile);

                // Split SQL queries by semicolon and execute them one by one
                string[] queries = sqlQueries.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (string query in queries)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            transaction.Dispose();
        }
    }
}