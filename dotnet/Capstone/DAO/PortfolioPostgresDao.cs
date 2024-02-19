using Capstone.DAO.Interfaces;

namespace Capstone.DAO
{
    public class PortfolioPostgresDao : IPortfolioDao
    {
        private readonly string connectionString;

        public PortfolioPostgresDao(string dbConnectionString) {
            connectionString = dbConnectionString;
        }

    }
}