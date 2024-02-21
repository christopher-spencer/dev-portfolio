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

        public ContributorPostgresDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Contributor CreateContributor(Contributor contributor)
        {
            string sql = "INSERT INTO contributors (first_name, last_name, contributor_image_name, contributor_image_url, email, bio, contribution_details, linkedin_url, github_url, portfolio_url) VALUES (@first_name, @last_name, @contributor_image_name, @contributor_image_url, @email, @bio, @contribution_details, @linkedin_url, @github_url, @portfolio_url) RETURNING id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@first_name", contributor.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", contributor.LastName);
                    cmd.Parameters.AddWithValue("@contributor_image_name", contributor.ContributorImage.Name);
                    cmd.Parameters.AddWithValue("@contributor_image_url", contributor.ContributorImage.Url);
                    cmd.Parameters.AddWithValue("@email", contributor.Email);
                    cmd.Parameters.AddWithValue("@bio", contributor.Bio);
                    cmd.Parameters.AddWithValue("@contribution_details", contributor.ContributionDetails);
                    cmd.Parameters.AddWithValue("@linkedin_url", contributor.LinkedInLink.Url);
                    cmd.Parameters.AddWithValue("@github_url", contributor.GitHubLink.Url);
                    cmd.Parameters.AddWithValue("@portfolio_url", contributor.PortfolioLink.Url);

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

            string sql = "SELECT c.id, c.first_name, c.last_name, c.contributor_image_name, c.contributor_image_url, " +
                         "c.email, c.bio, c.contribution_details, c.linkedin_url, c.github_url, c.portfolio_url " +
                         "FROM contributors c " +
                         "JOIN side_project_contributors pc ON c.id = pc.contributor_id " +
                         "WHERE pc.project_id = @projectId;";

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

        public Contributor GetContributorById(int contributorId)
        {
            string sql = "SELECT first_name, last_name, contributor_image_name, contributor_image_url, email, bio, contribution_details, linkedin_url, github_url, portfolio_url FROM contributors WHERE id = @id;";

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
            string sql = "SELECT id, first_name, last_name, contributor_image_name, contributor_image_url, email, bio, contribution_details, linkedin_url, github_url, portfolio_url FROM contributors;";

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

        public Contributor UpdateContributor(Contributor contributor)
        {
            string sql = "UPDATE contributors SET first_name = @first_name, last_name = @last_name, contributor_image_name = @contributor_image_name, contributor_image_url = @contributor_image_url, email = @email, bio = @bio, contribution_details = @contribution_details, linkedin_url = @linkedin_url, github_url = @github_url, portfolio_url = @portfolio_url WHERE id = @id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", contributor.Id);
                    cmd.Parameters.AddWithValue("@first_name", contributor.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", contributor.LastName);
                    cmd.Parameters.AddWithValue("@contributor_image_name", contributor.ContributorImage.Name);
                    cmd.Parameters.AddWithValue("@contributor_image_url", contributor.ContributorImage.Url);
                    cmd.Parameters.AddWithValue("@email", contributor.Email);
                    cmd.Parameters.AddWithValue("@bio", contributor.Bio);
                    cmd.Parameters.AddWithValue("@contribution_details", contributor.ContributionDetails);
                    cmd.Parameters.AddWithValue("@linkedin_url", contributor.LinkedInLink.Url);
                    cmd.Parameters.AddWithValue("@github_url", contributor.GitHubLink.Url);
                    cmd.Parameters.AddWithValue("@portfolio_url", contributor.PortfolioLink.Url);

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

        private Contributor MapRowToContributor(NpgsqlDataReader reader)
        {
            return new Contributor
            {
                Id = Convert.ToInt32(reader["id"]),
                FirstName = Convert.ToString(reader["first_name"]),
                LastName = Convert.ToString(reader["last_name"]),
                ContributorImage = new Image
                {
                    Name = Convert.ToString(reader["contributor_image_name"]),
                    Url = Convert.ToString(reader["contributor_image_url"])
                },
                Email = Convert.ToString(reader["email"]),
                Bio = Convert.ToString(reader["bio"]),
                ContributionDetails = Convert.ToString(reader["contribution_details"]),
                LinkedInLink = new Website { Url = Convert.ToString(reader["linkedin_url"]) },
                GitHubLink = new Website { Url = Convert.ToString(reader["github_url"]) },
                PortfolioLink = new Website { Url = Convert.ToString(reader["portfolio_url"]) }
            };
        }
    }
}
