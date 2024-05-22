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
            // FIXME check exceptions here and figure out updated infrastructure to get tests rolling again
            using (NpgsqlConnection conn = new NpgsqlConnection(TestConnectionString))
            {
                conn.Open();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            transaction.Dispose();
        }
    }
}