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
        private readonly IWorkExperienceDao _workExperienceDao;
        private readonly IEducationDao _educationDao;
        private readonly ICredentialDao _credentialDao;
        private readonly IVolunteerWorkDao _volunteerWorkDao;
        private readonly IOpenSourceContributionDao _openSourceContributionDao;
        private readonly IHobbyDao _hobbyDao;


        public PortfolioPostgresDao(string dbConnectionString, ISideProjectDao sideProjectDao,
            IWebsiteDao websiteDao, IImageDao imageDao, ISkillDao skillDao, 
            IWorkExperienceDao workExperienceDao, IEducationDao educationDao, ICredentialDao credentialDao,
            IVolunteerWorkDao volunteerWorkDao, IOpenSourceContributionDao openSourceContributionDao,
            IHobbyDao hobbyDao)
            {
                connectionString = dbConnectionString;
                _sideProjectDao = sideProjectDao;
                _websiteDao = websiteDao;
                _imageDao = imageDao;
                _skillDao = skillDao;
                _workExperienceDao = workExperienceDao;
                _educationDao = educationDao;
                _credentialDao = credentialDao;
                _volunteerWorkDao = volunteerWorkDao;
                _openSourceContributionDao = openSourceContributionDao;
                _hobbyDao = hobbyDao;
            }

        /*  
            **********************************************************************************************
                                            PORTFOLIO CRUD
            **********************************************************************************************
        */
// TODO finish CRUD, helper methods and maprow
        public Portfolio CreatePortfolio(Portfolio portfolio)
        {
            if (string.IsNullOrEmpty(portfolio.Name))
            {
                throw new ArgumentException("Portfolio name is required.");
            }

            if (string.IsNullOrEmpty(portfolio.ProfessionalSummary))
            {
                throw new ArgumentException("Professional summary is required.");
            }

            string sql = "INSERT INTO portfolios (name, location, professional_summary, email) " +
                         "VALUES (@name, @location, @professionalSummary, @email) " +
                         "RETURNING id";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", portfolio.Name);
                        cmd.Parameters.AddWithValue("@location", portfolio.Location);
                        cmd.Parameters.AddWithValue("@professionalSummary", portfolio.ProfessionalSummary);
                        cmd.Parameters.AddWithValue("@email", portfolio.Email);

                        int portfolioId = (int)cmd.ExecuteScalar();
                        portfolio.Id = portfolioId;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the portfolio.", ex);
            }

            return portfolio;
        }

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

        public Portfolio UpdatePortfolio(Portfolio portfolio, int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(portfolio.Name))
            {
                throw new ArgumentException("Portfolio name is required.");
            }

            if (string.IsNullOrEmpty(portfolio.ProfessionalSummary))
            {
                throw new ArgumentException("Professional summary is required.");
            }

            string sql = "UPDATE portfolios SET name = @name, location = @location, " +
                         "professional_summary = @professionalSummary, email = @email " +
                         "WHERE id = @portfolioId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", portfolio.Name);
                        cmd.Parameters.AddWithValue("@location", portfolio.Location);
                        cmd.Parameters.AddWithValue("@professionalSummary", portfolio.ProfessionalSummary);
                        cmd.Parameters.AddWithValue("@email", portfolio.Email);
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return portfolio;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the portfolio.", ex);
            }
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

            SetPortfolioMainImageIdProperties(reader, portfolio, portfolioId);


            return portfolio;
        }

        private void SetPortfolioMainImageIdProperties(NpgsqlDataReader reader, Portfolio portfolio, int portfolioId)
        {
            if (reader["main_image_id"] != DBNull.Value)
            {
                portfolio.MainImageId = Convert.ToInt32(reader["main_image_id"]);

                portfolio.MainImage = _imageDao.GetMainImageByPortfolioId(portfolioId);
            }
            else
            {
                portfolio.MainImageId = 0;
            }
        }

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