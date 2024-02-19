using System.Transactions;
using Npgsql;

namespace Capstone.UnitTests.DAO
{
    public abstract class PostgresDaoTestBase
    {
        protected string ConnectionString = @"Host=localhost;Port=5432;Database=dev_portfolio;Username=dev_portfolio_appuser;Password=finalcapstone";

        private TransactionScope transaction;

        [TestInitialize]
        public void Initialize()
        {
            transaction = new TransactionScope();

            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                string sql_insert = "INSERT INTO users (username, password_hash, salt, user_role) " +
                    "VALUES ('notauser', 'jjjjjjjjj', 'kkkkkkkkkk', 'user');";
                    
                NpgsqlCommand cmd = new NpgsqlCommand(sql_insert, conn);
                int count = cmd.ExecuteNonQuery();

                Assert.AreEqual(1, count, "Insert into user failed");
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            transaction.Dispose();
        }
    }
}