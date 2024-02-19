using System.Transactions;

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
        }

        [TestCleanup]
        public void Cleanup()
        {
            transaction.Dispose();
        }
    }
}