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


        private Portfolio MapRowToPortfolio(NpgsqlDataReader reader)
        {
            Portfolio portfolio = new Portfolio
            {
                Id = Convert.ToInt32(reader["portfolio_id"]),
                Name = Convert.ToString(reader["portfolio_name"]),
                Location = Convert.ToString(reader["location"]),
                ProfessionalSummary = Convert.ToString(reader["professional_summary"]),
                
            };

            return portfolio;
        }
    }
}