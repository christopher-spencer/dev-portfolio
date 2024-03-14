using System;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class PortfolioPostgresDao : IPortfolioDao
    {
        private readonly string connectionString;

        public PortfolioPostgresDao(string dbConnectionString) {
            connectionString = dbConnectionString;
        }
// TODO will need to model after SP and BP Daos
        // FIXME use SIDEPROJECT POSTGRES DAO CRUD AS TEMPLATE FOR IMPROVING CRUD METHODS

        private Portfolio MapRowToPortfolio(NpgsqlDataReader reader)
        {
            Portfolio portfolio = new Portfolio
            {
                Id = Convert.ToInt32(reader["portfolio_id"]),
                Name = Convert.ToString(reader["portfolio_name"]),
                PortfolioImage = new Image
                {
                    Url = Convert.ToString(reader["portfolio_image"])
                },
                Location = Convert.ToString(reader["location"]),
                ProfessionalSummary = Convert.ToString(reader["professional_summary"]),

            };

            return portfolio;
        }
    }
}