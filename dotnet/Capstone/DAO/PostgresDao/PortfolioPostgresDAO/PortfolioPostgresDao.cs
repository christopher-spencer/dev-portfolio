using System;
using System.Collections.Generic;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class PortfolioPostgresDao : IPortfolioDao
    {
        private readonly string connectionString;
        private readonly ISideProjectDao _sideProjectDao;
        private readonly IWebsiteDao _websiteDao;
        private readonly IImageDao _imageDao;
        private readonly ISkillDao _skillDao;

        public PortfolioPostgresDao(string dbConnectionString, ISideProjectDao sideProjectDao,
            IWebsiteDao websiteDao, IImageDao imageDao, ISkillDao skillDao)
        {
            connectionString = dbConnectionString;
            this._sideProjectDao = sideProjectDao;
            this._websiteDao = websiteDao;
            this._imageDao = imageDao;
            this._skillDao = skillDao;
        }

        /*  
            **********************************************************************************************
                                            PORTFOLIO CRUD
            **********************************************************************************************
        */
// TODO finish CRUD, helper methods and maprow
        public List<Portfolio> GetPortfolios()
        {
            List<Portfolio> portfolios = new List<Portfolio>();

            string sql = "SELECT id, name, main_image_id, location, professional_summary, email, " +
                "github_repo_link_id, linkedin_id FROM portfolios";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Portfolio portfolio = MapRowToPortfolio(reader);
                                portfolios.Add(portfolio);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the portfolios.", ex);
            }

            return portfolios;
        }

        public Portfolio GetPortfolio(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            Portfolio portfolio = null;

            string sql = "SELECT id, name, main_image_id, location, professional_summary, email, " +
                "github_repo_link_id, linkedin_id FROM portfolios WHERE id = @portfolioId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                portfolio = MapRowToPortfolio(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the portfolio.", ex);
            }

            return portfolio;
        }

        /*  
            **********************************************************************************************
                                        PORTFOLIO HELPER METHODS
            **********************************************************************************************
        */

        /*  
            **********************************************************************************************
                                            PORTFOLIO MAP ROW
            **********************************************************************************************
        */

        private Portfolio MapRowToPortfolio(NpgsqlDataReader reader)
        {
            Portfolio portfolio = new Portfolio
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Location = Convert.ToString(reader["location"]),
                ProfessionalSummary = Convert.ToString(reader["professional_summary"]),
                Email = Convert.ToString(reader["email"])
            };

            int portfolioId = portfolio.Id;

           // SetPortfolioMainImageIdProperties(reader, portfolio, portfolioId);


            return portfolio;
        }

        // private void SetPortfolioMainImageIdProperties(NpgsqlDataReader reader, Portfolio portfolio, int portfolioId)
        // {
        //     if (reader["main_image_id"] != DBNull.Value)
        //     {
        //         portfolio.MainImageId = Convert.ToInt32(reader["main_image_id"]);

        //         portfolio.MainImage = _imageDao.GetMainImageByPortfolioId(portfolioId);
        //     }
        //     else
        //     {
        //         portfolio.MainImageId = 0;
        //     }
        // }

        // private void SetPortfolioGitHubRepoLinkIdProperties(NpgsqlDataReader reader, Portfolio portfolio, int portfolioId)
        // {
        //     if (reader["github_repo_link_id"] != DBNull.Value)
        //     {
        //         portfolio.GitHubRepoLinkId = Convert.ToInt32(reader["github_repo_link_id"]);

        //         portfolio.GitHubRepoLink = _websiteDao.GetWebsiteByPortfolioId(portfolioId);
        //     }
        // }
    }
}