using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    // TODO Organize Methods By CRUD and BLogPost or SideProject, etc.
    public class ContributorPostgresDao : IContributorDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;
        private readonly IWebsiteDao _websiteDao;

        public ContributorPostgresDao(string dbConnectionString, IImageDao imageDao, IWebsiteDao websiteDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
            this._websiteDao = websiteDao;
        }

        public Contributor CreateContributorByProjectId(int projectId, Contributor contributor)
        {
            string insertContributorSql = "INSERT INTO contributors (first_name, last_name, contributor_image_id, email, " +
                                          "bio, contribution_details, linkedin_id, github_id, portfolio_id) " +
                                          "VALUES (@first_name, @last_name, @contributor_image_id, @email, @bio, " +
                                          "@contribution_details, @linkedin_id, @github_id, @portfolio_id) " +
                                          "RETURNING id;";
            string insertSideProjectContributorSql = "INSERT INTO sideproject_contributors (sideproject_id, contributor_id) " +
                                                     "VALUES (@projectId, @contributorId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmdInsertContributor = new NpgsqlCommand(insertContributorSql, connection);
                    cmdInsertContributor.Parameters.AddWithValue("@first_name", contributor.FirstName);
                    cmdInsertContributor.Parameters.AddWithValue("@last_name", contributor.LastName);
                    cmdInsertContributor.Parameters.AddWithValue("@contributor_image_id", contributor.ContributorImageId);
                    cmdInsertContributor.Parameters.AddWithValue("@email", contributor.Email);
                    cmdInsertContributor.Parameters.AddWithValue("@bio", contributor.Bio);
                    cmdInsertContributor.Parameters.AddWithValue("@contribution_details", contributor.ContributionDetails);
                    cmdInsertContributor.Parameters.AddWithValue("@linkedin_id", contributor.LinkedInId);
                    cmdInsertContributor.Parameters.AddWithValue("@github_id", contributor.GitHubId);
                    cmdInsertContributor.Parameters.AddWithValue("@portfolio_id", contributor.PortfolioId);

                    int contributorId = (int)cmdInsertContributor.ExecuteScalar();

                    NpgsqlCommand cmdInsertSideProjectContributor = new NpgsqlCommand(insertSideProjectContributorSql, connection);
                    cmdInsertSideProjectContributor.Parameters.AddWithValue("@projectId", projectId);
                    cmdInsertSideProjectContributor.Parameters.AddWithValue("@contributorId", contributorId);

                    cmdInsertSideProjectContributor.ExecuteNonQuery();

                    contributor.Id = contributorId;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the contributor for the project.", ex);
            }

            return contributor;
        }

        public Contributor CreateContributor(Contributor contributor)
        {
            string sql = "INSERT INTO contributors (first_name, last_name, contributor_image_id, email, " +
                         "bio, contribution_details, linkedin_id, github_id, portfolio_id) " +
                         "VALUES (@first_name, @last_name, @contributor_image_id, @email, @bio, " +
                         "@contribution_details, @linkedin_id, @github_id, @portfolio_id) " +
                         "RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@first_name", contributor.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", contributor.LastName);
                    cmd.Parameters.AddWithValue("@contributor_image_id", contributor.ContributorImageId);
                    cmd.Parameters.AddWithValue("@email", contributor.Email);
                    cmd.Parameters.AddWithValue("@bio", contributor.Bio);
                    cmd.Parameters.AddWithValue("@contribution_details", contributor.ContributionDetails);
                    cmd.Parameters.AddWithValue("@linkedin_id", contributor.LinkedInId);
                    cmd.Parameters.AddWithValue("@github_id", contributor.GitHubId);
                    cmd.Parameters.AddWithValue("@portfolio_id", contributor.PortfolioId);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    contributor.Id = id;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the contributor.", ex);
            }

            return contributor;
        }

        public List<Contributor> GetContributorsByProjectId(int projectId)
        {
            List<Contributor> contributors = new List<Contributor>();

            string sql = "SELECT c.id, c.first_name, c.last_name, c.contributor_image_id, c.email, c.bio, c.contribution_details, " +
                         "c.linkedin_id, c.github_id, c.portfolio_id " +
                         "FROM contributors c " +
                         "JOIN sideproject_contributors pc ON c.id = pc.contributor_id " +
                         "WHERE pc.sideproject_id = @projectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        contributors.Add(MapRowToContributor(reader));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving contributors by project ID.", ex);
            }

            return contributors;
        }

        public Contributor GetContributorByProjectId(int projectId, int contributorId)
        {
            Contributor contributor = null;

            string sql = "SELECT c.id, c.first_name, c.last_name, c.contributor_image_id, c.email, c.bio, c.contribution_details, " +
                         "c.linkedin_id, c.github_id, c.portfolio_id FROM contributors c " +
                         "JOIN sideproject_contributors pc ON c.id = pc.contributor_id " +
                         "WHERE pc.sideproject_id = @projectId AND c.id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@contributorId", contributorId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        contributor = MapRowToContributor(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the contributor for the project.", ex);
            }

            return contributor;
        }

        public Contributor GetContributorById(int contributorId)
        {
            string sql = "SELECT first_name, last_name, contributor_image_id, email, bio, contribution_details, " +
                         "linkedin_id, github_id, portfolio_id " +
                         "FROM contributors WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", contributorId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return MapRowToContributor(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the contributor.", ex);
            }

            return null;
        }

        public List<Contributor> GetAllContributors()
        {
            List<Contributor> contributors = new List<Contributor>();
            string sql = "SELECT id, first_name, last_name, contributor_image_id, email, bio, " +
                         "contribution_details, linkedin_id, github_id, portfolio_id " +
                         "FROM contributors;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        contributors.Add(MapRowToContributor(reader));
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the contributors.", ex);
            }

            return contributors;
        }

        public Contributor UpdateContributorByProjectId(int projectId, Contributor updatedContributor)
        {
            string sql = "UPDATE contributors " +
                         "SET first_name = @first_name, last_name = @last_name, contributor_image_id = @contributor_image_id, " +
                         "email = @email, bio = @bio, contribution_details = @contribution_details, " +
                         "linkedin_id = @linkedin_id, github_id = @github_id, portfolio_id = @portfolio_id " +
                         "FROM sideproject_contributors " +
                         "WHERE contributors.id = sideproject_contributors.contributor_id " +
                         "AND sideproject_contributors.sideproject_id = @projectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@first_name", updatedContributor.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", updatedContributor.LastName);
                    cmd.Parameters.AddWithValue("@contributor_image_id", updatedContributor.ContributorImageId);
                    cmd.Parameters.AddWithValue("@email", updatedContributor.Email);
                    cmd.Parameters.AddWithValue("@bio", updatedContributor.Bio);
                    cmd.Parameters.AddWithValue("@contribution_details", updatedContributor.ContributionDetails);
                    cmd.Parameters.AddWithValue("@linkedin_id", updatedContributor.LinkedInId);
                    cmd.Parameters.AddWithValue("@github_id", updatedContributor.GitHubId);
                    cmd.Parameters.AddWithValue("@portfolio_id", updatedContributor.PortfolioId);

                    int count = cmd.ExecuteNonQuery();

                    if (count > 0)
                    {
                        return updatedContributor;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the contributor for the project.", ex);
            }

            return null;
        }
        public Contributor UpdateContributor(Contributor contributor)
        {
            string sql = "UPDATE contributors SET first_name = @first_name, last_name = @last_name, contributor_image_id = @contributor_image_id, " +
                         "email = @email, bio = @bio, contribution_details = @contribution_details, " +
                         "linkedin_id = @linkedin_id, github_id = @github_id, portfolio_id = @portfolio_id " +
                         "WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", contributor.Id);
                    cmd.Parameters.AddWithValue("@first_name", contributor.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", contributor.LastName);
                    cmd.Parameters.AddWithValue("@contributor_image_id", contributor.ContributorImageId);
                    cmd.Parameters.AddWithValue("@email", contributor.Email);
                    cmd.Parameters.AddWithValue("@bio", contributor.Bio);
                    cmd.Parameters.AddWithValue("@contribution_details", contributor.ContributionDetails);
                    cmd.Parameters.AddWithValue("@linkedin_id", contributor.LinkedInId);
                    cmd.Parameters.AddWithValue("@github_id", contributor.GitHubId);
                    cmd.Parameters.AddWithValue("@portfolio_id", contributor.PortfolioId);

                    int count = cmd.ExecuteNonQuery();
                    if (count == 1)
                    {
                        return contributor;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the contributor.", ex);
            }

            return null;
        }

        public int DeleteContributorByProjectId(int projectId, int contributorId)
        {
            string sql = "DELETE FROM sideproject_contributors WHERE sideproject_id = @projectId AND contributor_id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@contributorId", contributorId);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the contributor from the project.", ex);
            }
        }

        public int DeleteContributorById(int contributorId)
        {
            string sql = "DELETE FROM contributors WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", contributorId);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the contributor.", ex);
            }
        }
        /*  
            **********************************************************************************************
                                            CONTRIBUTOR MAP ROW
            **********************************************************************************************
        */
        private Contributor MapRowToContributor(NpgsqlDataReader reader)
        {
            Contributor contributor = new Contributor
            {
                Id = Convert.ToInt32(reader["id"]),
                FirstName = Convert.ToString(reader["first_name"]),
                LastName = Convert.ToString(reader["last_name"]),
                ContributorImageId = Convert.ToInt32(reader["contributor_image_id"]),
                Email = Convert.ToString(reader["email"]),
                Bio = Convert.ToString(reader["bio"]),
                ContributionDetails = Convert.ToString(reader["contribution_details"]),
                LinkedInId = Convert.ToInt32(reader["linkedin_id"]),
                GitHubId = Convert.ToInt32(reader["github_id"]),
                PortfolioId = Convert.ToInt32(reader["portfolio_id"])
            };

            if (reader["contributor_image_id"] != DBNull.Value)
            {
                int contributorImageId = Convert.ToInt32(reader["contributor_image_id"]);
                contributor.ContributorImage = _imageDao.GetImageById(contributorImageId);
            }

            if (reader["linkedin_id"] != DBNull.Value)
            {
                int linkedInId = Convert.ToInt32(reader["linkedin_id"]);
                contributor.LinkedIn = _websiteDao.GetWebsiteById(linkedInId);
            }

            if (reader["github_id"] != DBNull.Value)
            {
                int githubId = Convert.ToInt32(reader["github_id"]);
                contributor.GitHub = _websiteDao.GetWebsiteById(githubId);
            }

            if (reader["portfolio_id"] != DBNull.Value)
            {
                int portfolioId = Convert.ToInt32(reader["portfolio_id"]);
                contributor.Portfolio = _websiteDao.GetWebsiteById(portfolioId);
            }

            return contributor;
        }
    }
}
