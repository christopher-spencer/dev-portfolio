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

        /*  
            **********************************************************************************************
                                                CONTRIBUTOR CRUD
            **********************************************************************************************
        */
        public Contributor CreateContributor(Contributor contributor)
        {
            if (string.IsNullOrEmpty(contributor.FirstName))
            {
                throw new ArgumentException("Contributor first name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(contributor.LastName))
            {
                throw new ArgumentException("Contributor last name cannot be null or empty.");
            }

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

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@first_name", contributor.FirstName);
                        cmd.Parameters.AddWithValue("@last_name", contributor.LastName);
                        cmd.Parameters.AddWithValue("@email", contributor.Email);
                        cmd.Parameters.AddWithValue("@bio", contributor.Bio);
                        cmd.Parameters.AddWithValue("@contribution_details", contributor.ContributionDetails);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        contributor.Id = id;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the contributor.", ex);
            }

            return contributor;
        }

        public Contributor GetContributor(int contributorId)
        {
            if (contributorId <= 0)
            {
                throw new ArgumentException("ContributorId must be greater than zero.");
            }

            string sql = "SELECT id, first_name, last_name, contributor_image_id, email, bio, contribution_details, " +
                         "linkedin_id, github_id, portfolio_id " +
                         "FROM contributors WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", contributorId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapRowToContributor(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the contributor.", ex);
            }

            return null;
        }

        public List<Contributor> GetContributors()
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

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                contributors.Add(MapRowToContributor(reader));
                            }
                        }
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
            if (string.IsNullOrEmpty(contributor.FirstName))
            {
                throw new ArgumentException("Contributor first name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(contributor.LastName))
            {
                throw new ArgumentException("Contributor last name cannot be null or empty.");
            }

            if (contributorId <= 0)
            {
                throw new ArgumentException("ContributorId must be greater than zero.");
            }

            string sql = "UPDATE contributors SET first_name = @first_name, last_name = @last_name, " +
                         "email = @email, bio = @bio, contribution_details = @contribution_details " +
                         "WHERE id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
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
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the contributor.", ex);
            }

            return null;
        }

        public int DeleteContributor(int contributorId)
        {
            if (contributorId <= 0)
            {
                throw new ArgumentException("ContributorId must be greater than zero.");
            }

            string sql = "DELETE FROM contributors WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", contributorId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected;
                    }
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
        public Contributor CreateContributorBySideProjectId(int sideProjectId, Contributor contributor)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(contributor.FirstName))
            {
                throw new ArgumentException("Contributor first name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(contributor.LastName))
            {
                throw new ArgumentException("Contributor last name cannot be null or empty.");
            }

            string insertContributorSql = "INSERT INTO contributors (first_name, last_name, email, " +
                                          "bio, contribution_details) " +
                                          "VALUES (@first_name, @last_name, @email, @bio, " +
                                          "@contribution_details) " +
                                          "RETURNING id;";
            string insertSideProjectContributorSql = "INSERT INTO sideproject_contributors (sideproject_id, contributor_id) " +
                                                     "VALUES (@sideProjectId, @contributorId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int contributorId;

                            using (NpgsqlCommand cmdInsertContributor = new NpgsqlCommand(insertContributorSql, connection))
                            {
                                cmdInsertContributor.Parameters.AddWithValue("@first_name", contributor.FirstName);
                                cmdInsertContributor.Parameters.AddWithValue("@last_name", contributor.LastName);
                                cmdInsertContributor.Parameters.AddWithValue("@email", contributor.Email);
                                cmdInsertContributor.Parameters.AddWithValue("@bio", contributor.Bio);
                                cmdInsertContributor.Parameters.AddWithValue("@contribution_details", contributor.ContributionDetails);
                                cmdInsertContributor.Transaction = transaction;
                                contributorId = Convert.ToInt32(cmdInsertContributor.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmdInsertSideProjectContributor = new NpgsqlCommand(insertSideProjectContributorSql, connection))
                            {
                                cmdInsertSideProjectContributor.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmdInsertSideProjectContributor.Parameters.AddWithValue("@contributorId", contributorId);
                                cmdInsertSideProjectContributor.Transaction = transaction;
                                cmdInsertSideProjectContributor.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            contributor.Id = contributorId;

                            return contributor;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the contributor for the side project.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Contributor> GetContributorsBySideProjectId(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            List<Contributor> contributors = new List<Contributor>();

            string sql = "SELECT c.id, c.first_name, c.last_name, c.contributor_image_id, c.email, c.bio, c.contribution_details, " +
                         "c.linkedin_id, c.github_id, c.portfolio_id " +
                         "FROM contributors c " +
                         "JOIN sideproject_contributors pc ON c.id = pc.contributor_id " +
                         "WHERE pc.sideproject_id = @sideProjectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                contributors.Add(MapRowToContributor(reader));
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving contributors by SideProjectId.", ex);
            }

            return contributors;
        }

        public Contributor GetContributorBySideProjectId(int sideProjectId, int contributorId)
        {
            if (sideProjectId <= 0 || contributorId <= 0)
            {
                throw new ArgumentException("SideProjectId and contributorId must be greater than zero.");
            }

            Contributor contributor = null;

            string sql = "SELECT c.id, c.first_name, c.last_name, c.contributor_image_id, c.email, c.bio, c.contribution_details, " +
                         "c.linkedin_id, c.github_id, c.portfolio_id FROM contributors c " +
                         "JOIN sideproject_contributors pc ON c.id = pc.contributor_id " +
                         "WHERE pc.sideproject_id = @sideProjectId AND c.id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                contributor = MapRowToContributor(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the contributor for the side project.", ex);
            }

            return contributor;
        }

        public Contributor UpdateContributorBySideProjectId(int sideProjectId, int contributorId, Contributor contributor)
        {
            if (sideProjectId <= 0 || contributorId <= 0)
            {
                throw new ArgumentException("SideProjectId and contributorId must be greater than zero.");
            }

            string sql = "UPDATE contributors " +
                         "SET first_name = @first_name, last_name = @last_name, " +
                         "email = @email, bio = @bio, contribution_details = @contribution_details " +
                         "FROM sideproject_contributors " +
                         "WHERE contributors.id = sideproject_contributors.contributor_id " +
                         "AND sideproject_contributors.sideproject_id = @sideProjectId " +
                         "AND contributors.id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
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
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the contributor for the side project.", ex);
            }

            return null;
        }

        public int DeleteContributorBySideProjectId(int sideProjectId, int contributorId)
        {
            if (sideProjectId <= 0 || contributorId <= 0)
            {
                throw new ArgumentException("SideProjectId and contributorId must be greater than zero.");
            }

            string deleteContributorFromSideProjectSql = "DELETE FROM sideproject_contributors WHERE sideproject_id = @sideProjectId AND contributor_id = @contributorId;";
            string deleteContributorSql = "DELETE FROM contributors WHERE id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int rowsAffected;

                            int? contributorImageId = GetContributorImageIdByContributorId(contributorId);
                            int? linkedInId = GetLinkedInIdByContributorId(contributorId);
                            int? gitHubId = GetGithubIdByContributorId(contributorId);
                            int? portfolioLinkId = GetPortfolioLinkIdByContributorId(contributorId);

                            if (contributorImageId.HasValue)
                            {
                                _imageDao.DeleteImageByContributorId(contributorId, contributorImageId.Value);
                            }

                            if (linkedInId.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByContributorId(contributorId, linkedInId.Value);
                            }

                            if (gitHubId.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByContributorId(contributorId, gitHubId.Value);
                            }

                            if (portfolioLinkId.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByContributorId(contributorId, portfolioLinkId.Value);
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteContributorFromSideProjectSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                                cmd.Parameters.AddWithValue("@contributorId", contributorId);

                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteContributorSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@contributorId", contributorId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the contributor by side project ID.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                                HELPER METHODS
            **********************************************************************************************
        */

        private int? GetContributorImageIdByContributorId(int contributorId)
        {
            string sql = "SELECT contributor_image_id FROM contributors WHERE id = @contributorId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving Contributor Image ID: " + ex.Message);
                return null;
            }
        }

        private int? GetLinkedInIdByContributorId(int contributorId)
        {
            string sql = "SELECT linkedin_id FROM contributors WHERE id = @contributorId;";
           
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving LinkedIn ID: " + ex.Message);
                return null;
            }
        }

        private int? GetGithubIdByContributorId(int contributorId)
        {
            string sql = "SELECT github_id FROM contributors WHERE id = @contributorId;";
           
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving GitHub ID: " + ex.Message);
                return null;
            }
        }

        private int? GetPortfolioLinkIdByContributorId(int contributorId)
        {
            string sql = "SELECT portfolio_id FROM contributors WHERE id = @contributorId;";
           
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributorId", contributorId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving Portfolio ID: " + ex.Message);
                return null;
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

            int contributorId = contributor.Id;

            SetContributorImageIdProperties(reader, contributor, contributorId);
            SetContributorLinkedInIdProperties(reader, contributor, contributorId);
            SetContributorGitHubIdProperties(reader, contributor, contributorId);
            SetContributorPortfolioIdProperties(reader, contributor, contributorId);

            return contributor;
        }

        private void SetContributorImageIdProperties(NpgsqlDataReader reader, Contributor contributor, int contributorId)
        {
            if (reader["contributor_image_id"] != DBNull.Value)
            {
                contributor.ContributorImageId = Convert.ToInt32(reader["contributor_image_id"]);

                contributor.ContributorImage = _imageDao.GetImageByContributorId(contributorId);
            }
            else
            {
                contributor.ContributorImageId = 0;
            }
        }

        private void SetContributorLinkedInIdProperties(NpgsqlDataReader reader, Contributor contributor, int contributorId)
        {
            if (reader["linkedin_id"] != DBNull.Value)
            {
                contributor.LinkedInId = Convert.ToInt32(reader["linkedin_id"]);

                int websiteId = contributor.LinkedInId;

                contributor.LinkedIn = _websiteDao.GetWebsiteByContributorId(contributorId, websiteId);
            }
            else
            {
                contributor.LinkedInId = 0;
            }
        }

        private void SetContributorGitHubIdProperties(NpgsqlDataReader reader, Contributor contributor, int contributorId)
        {
            if (reader["github_id"] != DBNull.Value)
            {
                contributor.GitHubId = Convert.ToInt32(reader["github_id"]);

                int websiteId = contributor.GitHubId;

                contributor.GitHub = _websiteDao.GetWebsiteByContributorId(contributorId, websiteId);
            }
            else
            {
                contributor.GitHubId = 0;
            }
        }

        private void SetContributorPortfolioIdProperties(NpgsqlDataReader reader, Contributor contributor, int contributorId)
        {
            if (reader["portfolio_id"] != DBNull.Value)
            {
                contributor.PortfolioId = Convert.ToInt32(reader["portfolio_id"]);

                int websiteId = contributor.PortfolioId;

                contributor.PortfolioLink = _websiteDao.GetWebsiteByContributorId(contributorId, websiteId);
            }
            else
            {
                contributor.PortfolioId = 0;
            }
        }
    }
}
