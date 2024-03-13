using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
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
        // FIXME use WEBSITE POSTGRES DAO CRUD AS TEMPLATE FOR IMPROVING CRUD METHODS

        /*  
            **********************************************************************************************
                                                CONTRIBUTOR CRUD
            **********************************************************************************************
        */
        public Contributor CreateContributor(Contributor contributor)
        {
            string sql = "INSERT INTO contributors (first_name, last_name, email, " +
                         "bio, contribution_details) " +
                         "VALUES (@first_name, @last_name, @email, @bio, " +
                         "@contribution_details) " +
                         "RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@first_name", contributor.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", contributor.LastName);
                    cmd.Parameters.AddWithValue("@email", contributor.Email);
                    cmd.Parameters.AddWithValue("@bio", contributor.Bio);
                    cmd.Parameters.AddWithValue("@contribution_details", contributor.ContributionDetails);

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

        public Contributor UpdateContributor(int contributorId, Contributor contributor)
        {
            string sql = "UPDATE contributors SET first_name = @first_name, last_name = @last_name, " +
                         "email = @email, bio = @bio, contribution_details = @contribution_details " +
                         "WHERE id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@contributorId", contributorId);
                    cmd.Parameters.AddWithValue("@first_name", contributor.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", contributor.LastName);
                    cmd.Parameters.AddWithValue("@email", contributor.Email);
                    cmd.Parameters.AddWithValue("@bio", contributor.Bio);
                    cmd.Parameters.AddWithValue("@contribution_details", contributor.ContributionDetails);

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
                                          SIDE PROJECT CONTRIBUTOR CRUD
            **********************************************************************************************
        */
        public Contributor CreateContributorBySideProjectId(int projectId, Contributor contributor)
        {
            string insertContributorSql = "INSERT INTO contributors (first_name, last_name, email, " +
                                          "bio, contribution_details) " +
                                          "VALUES (@first_name, @last_name, @email, @bio, " +
                                          "@contribution_details) " +
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
                    cmdInsertContributor.Parameters.AddWithValue("@email", contributor.Email);
                    cmdInsertContributor.Parameters.AddWithValue("@bio", contributor.Bio);
                    cmdInsertContributor.Parameters.AddWithValue("@contribution_details", contributor.ContributionDetails);

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

        public List<Contributor> GetContributorsBySideProjectId(int projectId)
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

        public Contributor GetContributorBySideProjectId(int projectId, int contributorId)
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

        public Contributor UpdateContributorBySideProjectId(int projectId, int contributorId, Contributor contributor)
        {
            string sql = "UPDATE contributors " +
                         "SET first_name = @first_name, last_name = @last_name, " +
                         "email = @email, bio = @bio, contribution_details = @contribution_details " +
                         "FROM sideproject_contributors " +
                         "WHERE contributors.id = sideproject_contributors.contributor_id " +
                         "AND sideproject_contributors.sideproject_id = @projectId " +
                         "AND contributors.id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@contributorId", contributorId); 
                    cmd.Parameters.AddWithValue("@first_name", contributor.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", contributor.LastName);
                    cmd.Parameters.AddWithValue("@email", contributor.Email);
                    cmd.Parameters.AddWithValue("@bio", contributor.Bio);
                    cmd.Parameters.AddWithValue("@contribution_details", contributor.ContributionDetails);

                    int count = cmd.ExecuteNonQuery();

                    if (count > 0)
                    {
                        return contributor;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the contributor for the project.", ex);
            }

            return null;
        }

        public int DeleteContributorBySideProjectId(int projectId, int contributorId)
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
                Email = Convert.ToString(reader["email"]),
                Bio = Convert.ToString(reader["bio"]),
                ContributionDetails = Convert.ToString(reader["contribution_details"])
            };

            SetContributorImageIdProperties(reader, contributor);
            SetContributorLinkedInIdProperties(reader, contributor);
            SetContributorGitHubIdProperties(reader, contributor);
            SetContributorPortfolioIdProperties(reader, contributor);

            return contributor;
        }

        private void SetContributorImageIdProperties(NpgsqlDataReader reader, Contributor contributor)
        {
            contributor.ContributorImageId = Convert.ToInt32(reader["contributor_image_id"]);

            if (reader["contributor_image_id"] != DBNull.Value)
            {
                int contributorImageId = Convert.ToInt32(reader["contributor_image_id"]);
                contributor.ContributorImage = _imageDao.GetImageById(contributorImageId);
            }
            else
            {
                contributor.ContributorImageId = 0;
            }
        }

        private void SetContributorLinkedInIdProperties(NpgsqlDataReader reader, Contributor contributor)
        {
            contributor.LinkedInId = Convert.ToInt32(reader["linkedin_id"]);

            if (reader["linkedin_id"] != DBNull.Value)
            {
                int linkedInId = Convert.ToInt32(reader["linkedin_id"]);
                contributor.LinkedIn = _websiteDao.GetWebsite(linkedInId);
            }
            else
            {
                contributor.LinkedInId = 0;
            }
        }

        private void SetContributorGitHubIdProperties(NpgsqlDataReader reader, Contributor contributor)
        {
            contributor.GitHubId = Convert.ToInt32(reader["github_id"]);

            if (reader["github_id"] != DBNull.Value)
            {
                int githubId = Convert.ToInt32(reader["github_id"]);
                contributor.GitHub = _websiteDao.GetWebsite(githubId);
            }
            else
            {
                contributor.GitHubId = 0;
            }
        }

        private void SetContributorPortfolioIdProperties(NpgsqlDataReader reader, Contributor contributor)
        {
            contributor.PortfolioId = Convert.ToInt32(reader["portfolio_id"]);

            if (reader["portfolio_id"] != DBNull.Value)
            {
                int portfolioId = Convert.ToInt32(reader["portfolio_id"]);
                contributor.PortfolioLink = _websiteDao.GetWebsite(portfolioId);
            }
            else
            {
                contributor.PortfolioId = 0;
            }
        }
    }
}
