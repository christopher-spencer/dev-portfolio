using System.Transactions;
using Npgsql;

namespace Capstone.UnitTests.DAO
{
    public abstract class PostgresDaoTestBase
    {
        protected string TestConnectionString = @"Host=localhost;Port=5432;Database=test_dev_portfolio;Username=test_dev_portfolio_appuser;Password=test_password";
        private TransactionScope transaction;

        //FIXME getting a transaction issue after adding additional tests to PortfolioPostgresDaoTests
        [TestInitialize]
        public void Initialize()
        {
            try
            {
                var transactionOptions = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.FromMinutes(2) // Adjust as necessary
                };

                transaction = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);


                using (NpgsqlConnection conn = new NpgsqlConnection(TestConnectionString))
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Exception during Initialize: {ex}");
                throw;
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                if (transaction != null)
                {
                    transaction.Complete();
                    transaction.Dispose();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Exception during Cleanup: {ex}");
            }
        }
    }
}