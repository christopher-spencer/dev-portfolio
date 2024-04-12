using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class OpenSourceContributionPostgresDao : IOpenSourceContributionDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;
        private readonly IWebsiteDao _websiteDao;
        private readonly IAchievementDao _achievementDao;
        private readonly ISkillDao _skillDao;


        public OpenSourceContributionPostgresDao(string dbConnectionString, IImageDao imageDao, IWebsiteDao websiteDao, IAchievementDao achievementDao, ISkillDao skillDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
            this._websiteDao = websiteDao;
            this._achievementDao = achievementDao;
            this._skillDao = skillDao;
        }

        const string MainImage = "main image";
        const string AdditionalImage = "additional image";
        const string Logo = "logo";

        /*  
            **********************************************************************************************
                                            OPEN SOURCE CONTRIBUTION CRUD
            **********************************************************************************************
        */

        public List<OpenSourceContribution> GetOpenSourceContributions()
        {
            List<OpenSourceContribution> contributions = new List<OpenSourceContribution>();

            string sql = "SELECT id, project_name, organization_name, start_date, " +
                         "end_date, project_description, contribution_details, " +
                         "organization_logo_id, organization_website_id, organization_github_id, main_image_id " +
                         "FROM open_source_contributions;";

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
                                OpenSourceContribution contribution = MapRowToOpenSourceContribution(reader);
                                contributions.Add(contribution);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving all Open Source Contributions.", ex);
            }

            return contributions;
        }

        public OpenSourceContribution GetOpenSourceContribution(int contributionId)
        {
            if (contributionId <= 0)
            {
                throw new ArgumentException("Contribution ID must be greater than zero.");
            }

            OpenSourceContribution contribution = null;

            string sql = "SELECT id, project_name, organization_name, start_date, " +
                         "end_date, project_description, contribution_details, " +
                         "organization_logo_id, organization_website_id, organization_github_id, main_image_id " +
                         "FROM open_source_contributions WHERE id = @contributionId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                contribution = MapRowToOpenSourceContribution(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Open Source Contribution.", ex);
            }

            return contribution;
        }

        /*  
            **********************************************************************************************
                                        PORTFOLIO OPEN SOURCE CONTRIBUTION CRUD
            **********************************************************************************************
        */

        public OpenSourceContribution CreateOpenSourceContributionByPortfolioId(int portfolioId, OpenSourceContribution contribution)
        {

            //FIXME add these null checks in each PGDAO to Helper Method******
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(contribution.ProjectName))
            {
                throw new ArgumentException("Project Name is required to create an Open Source Contribution.");
            }

            if (string.IsNullOrEmpty(contribution.OrganizationName))
            {
                throw new ArgumentException("Organization Name is required to create an Open Source Contribution.");
            }

            if (contribution.StartDate == DateTime.MinValue || contribution.StartDate > DateTime.Now)
            {
                throw new ArgumentException("Start Date must be a valid date in the past or present to create an Open Source Contribution.");
            }

            if (string.IsNullOrEmpty(contribution.ContributionDetails))
            {
                throw new ArgumentException("Contribution Details are required to create an Open Source Contribution.");
            }

            string insertContributionSql = "INSERT INTO open_source_contributions (project_name, organization_name, start_date, " +
                         "end_date, project_description, contribution_details) " +
                         "VALUES (@projectName, @organizationName, @startDate, @endDate, @projectDescription, " +
                         "@contributionDetails) " +
                         "RETURNING id;";

            string insertPortfolioContributionSql = "INSERT INTO portfolio_open_source_contributions (portfolio_id, contribution_id) " +
                                                    "VALUES (@portfolioId, @contributionId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int contributionId;

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertContributionSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@projectName", contribution.ProjectName);
                                cmd.Parameters.AddWithValue("@organizationName", contribution.OrganizationName);
                                cmd.Parameters.AddWithValue("@startDate", contribution.StartDate);
                                cmd.Parameters.AddWithValue("@endDate", contribution.EndDate);
                                cmd.Parameters.AddWithValue("@projectDescription", contribution.ProjectDescription);
                                cmd.Parameters.AddWithValue("@contributionDetails", contribution.ContributionDetails);
                                cmd.Transaction = transaction;

                                contributionId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertPortfolioContributionSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@contributionId", contributionId);
                                cmd.Transaction = transaction;
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            contribution.Id = contributionId;

                            return contribution;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the Open Source Contribution for the Portfolio.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<OpenSourceContribution> GetOpenSourceContributionsByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            List<OpenSourceContribution> contributions = new List<OpenSourceContribution>();

            string sql = "SELECT osc.id, osc.project_name, osc.organization_name, osc.start_date, " +
                         "osc.end_date, osc.project_description, osc.contribution_details, " +
                         "osc.organization_logo_id, osc.organization_website_id, osc.organization_github_id, osc.main_image_id " +
                         "FROM open_source_contributions osc " +
                         "JOIN portfolio_open_source_contributions psc ON osc.id = psc.contribution_id " +
                         "WHERE psc.portfolio_id = @portfolioId;";

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
                            while (reader.Read())
                            {
                                OpenSourceContribution contribution = MapRowToOpenSourceContribution(reader);
                                contributions.Add(contribution);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving all Open Source Contributions for the Portfolio.", ex);
            }

            return contributions;
        }

        public OpenSourceContribution GetOpenSourceContributionByPortfolioId(int portfolioId, int contributionId)
        {
            if (portfolioId <= 0 || contributionId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Contribution ID must be greater than zero.");
            }

            OpenSourceContribution contribution = null;

            string sql = "SELECT osc.id, osc.project_name, osc.organization_name, osc.start_date, " +
                         "osc.end_date, osc.project_description, osc.contribution_details, " +
                         "osc.organization_logo_id, osc.organization_website_id, osc.organization_github_id, osc.main_image_id " +
                         "FROM open_source_contributions osc " +
                         "JOIN portfolio_open_source_contributions psc ON osc.id = psc.contribution_id " +
                         "WHERE psc.portfolio_id = @portfolioId AND osc.id = @contributionId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                contribution = MapRowToOpenSourceContribution(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Open Source Contribution for the Portfolio.", ex);
            }

            return contribution;
        }

        public OpenSourceContribution UpdateOpenSourceContributionByPortfolioId(int portfolioId, int contributionId, OpenSourceContribution contribution)
        {
            if (portfolioId <= 0 || contributionId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Contribution ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(contribution.ProjectName))
            {
                throw new ArgumentException("Project Name is required to update an Open Source Contribution.");
            }

            if (string.IsNullOrEmpty(contribution.OrganizationName))
            {
                throw new ArgumentException("Organization Name is required to update an Open Source Contribution.");
            }

            if (contribution.StartDate == DateTime.MinValue || contribution.StartDate > DateTime.Now)
            {
                throw new ArgumentException("Start Date must be a valid date in the past or present to update an Open Source Contribution.");
            }

            if (string.IsNullOrEmpty(contribution.ContributionDetails))
            {
                throw new ArgumentException("Contribution Details are required to update an Open Source Contribution.");
            }

            string sql = "UPDATE open_source_contributions SET project_name = @projectName, " +
                         "organization_name = @organizationName, start_date = @startDate, " +
                         "end_date = @endDate, project_description = @projectDescription, " +
                         "contribution_details = @contributionDetails " +
                         "FROM portfolio_open_source_contributions psc " +
                         "WHERE psc.portfolio_id = @portfolioId AND psc.contribution_id = @contributionId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectName", contribution.ProjectName);
                        cmd.Parameters.AddWithValue("@organizationName", contribution.OrganizationName);
                        cmd.Parameters.AddWithValue("@startDate", contribution.StartDate);
                        cmd.Parameters.AddWithValue("@endDate", contribution.EndDate);
                        cmd.Parameters.AddWithValue("@projectDescription", contribution.ProjectDescription);
                        cmd.Parameters.AddWithValue("@contributionDetails", contribution.ContributionDetails);
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return contribution;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the open source contribution by portfolio ID.", ex);
            }

            return null;
        }

        public int DeleteOpenSourceContributionByPortfolioId(int portfolioId, int contributionId)
        {
            if (portfolioId <= 0 || contributionId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Contribution ID must be greater than zero.");
            }

            string deletePortfolioContributionSql = "DELETE FROM portfolio_open_source_contributions " +
                                                    "WHERE portfolio_id = @portfolioId " +
                                                    "AND contribution_id = @contributionId;";
            
            string deleteContributionSql = "DELETE FROM open_source_contributions WHERE id = @contributionId;";

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

                            int? organizationLogoId = GetOrganizationLogoIdByOpenSourceContributionId(contributionId);
                            int? organizationWebsiteId = GetOrganizationWebsiteIdByOpenSourceContributionId(contributionId);
                            int? organizationGitHubId = GetOrganizationGitHubIdByOpenSourceContributionId(contributionId);
                            int? mainImageId = GetMainImageIdByOpenSourceContributionId(contributionId);

                            if (organizationLogoId.HasValue)
                            {
                                _imageDao.DeleteImageByOpenSourceContributionId(contributionId, organizationLogoId.Value);

                            }

                            if (organizationWebsiteId.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByOpenSourceContributionId(contributionId, organizationWebsiteId.Value);
                            }

                            if (organizationGitHubId.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByOpenSourceContributionId(contributionId, organizationGitHubId.Value);
                            }

                            if (mainImageId.HasValue)
                            {
                                _imageDao.DeleteImageByOpenSourceContributionId(contributionId, mainImageId.Value);
                            }

                            DeleteReviewCommentsAndFeedbackReceivedByOpenSourceContributionId(contributionId);
                            DeleteTechSkillsUtilizedByOpenSourceContributionId(contributionId);
                            DeletePullRequestsLinksByOpenSourceContributionId(contributionId);
                            DeleteAdditionalImagesByOpenSourceContributionId(contributionId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deletePortfolioContributionSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@contributionId", contributionId);
                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteContributionSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@contributionId", contributionId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the Open Source Contribution by Portfolio ID.", ex);
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

        private int? GetOrganizationLogoIdByOpenSourceContributionId(int contributionId)
        {
            string sql = "SELECT organization_logo_id FROM open_source_contributions WHERE id = @contributionId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving Organization Logo ID by Open Source Contribution ID: " + ex.Message);
                return null;
            }
        }

        private int? GetOrganizationWebsiteIdByOpenSourceContributionId(int contributionId)
        {
            string sql = "SELECT organization_website_id FROM open_source_contributions WHERE id = @contributionId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving Organization Website ID by Open Source Contribution ID: " + ex.Message);
                return null;
            }
        }

        private int? GetOrganizationGitHubIdByOpenSourceContributionId(int contributionId)
        {
            string sql = "SELECT organization_github_id FROM open_source_contributions WHERE id = @contributionId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving Organization GitHub ID by Open Source Contribution ID: " + ex.Message);
                return null;
            }
        }

        private int? GetMainImageIdByOpenSourceContributionId(int contributionId)
        {
            string sql = "SELECT main_image_id FROM open_source_contributions WHERE id = @contributionId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@contributionId", contributionId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving Main Image ID by Open Source Contribution ID: " + ex.Message);
                return null;
            }
        }

        private int DeleteTechSkillsUtilizedByOpenSourceContributionId(int contributionId)
        {
            List<Skill> skills = _skillDao.GetSkillsByOpenSourceContributionId(contributionId);

            int skillsDeletedCount = 0;

            foreach (Skill skill in skills)
            {
                int skillId = skill.Id;

                try
                {
                    _skillDao.DeleteSkillByOpenSourceContributionId(contributionId, skillId);
                    skillsDeletedCount++;
                }
                catch
                {
                    Console.WriteLine("Error deleting Skill by Open Source Contribution ID: " + skillId);
                }
            }

            return skillsDeletedCount;
        }

        private int DeletePullRequestsLinksByOpenSourceContributionId(int contributionId)
        {
            List<Website> websites = _websiteDao.GetAllWebsitesByOpenSourceContributionId(contributionId);

            int websitesDeletedCount = 0;

            foreach (Website website in websites)
            {
                int websiteId = website.Id;

                try
                {
                    _websiteDao.DeleteWebsiteByOpenSourceContributionId(contributionId, websiteId);
                    websitesDeletedCount++;
                }
                catch
                {
                    Console.WriteLine("Error deleting Website by Open Source Contribution ID: " + websiteId);
                }
            }

            return websitesDeletedCount;
        }

        private int DeleteReviewCommentsAndFeedbackReceivedByOpenSourceContributionId(int contributionId)
        {
            List<Achievement> achievements = _achievementDao.GetAchievementsByOpenSourceContributionId(contributionId);

            int achievementsDeletedCount = 0;

            foreach (Achievement achievement in achievements)
            {
                int achievementId = achievement.Id;

                try
                {
                    _achievementDao.DeleteAchievementByOpenSourceContributionId(contributionId, achievementId);
                    achievementsDeletedCount++;
                }
                catch
                {
                    Console.WriteLine("Error deleting Achievement by Open Source Contribution ID: " + achievementId);
                }
            }

            return achievementsDeletedCount;
        }

        private int DeleteAdditionalImagesByOpenSourceContributionId(int contributionId)
        {
            List<Image> additionalImages = _imageDao.GetAdditionalImagesByOpenSourceContributionId(contributionId);

            int imagesDeletedCount = 0;

            foreach (Image image in additionalImages)
            {
                int imageId = image.Id;

                try
                {
                    _imageDao.DeleteImageByOpenSourceContributionId(contributionId, imageId);
                    imagesDeletedCount++;
                }
                catch
                {
                    Console.WriteLine("Error deleting Additional Image by Open Source Contribution ID: " + imageId);
                }
            }

            return imagesDeletedCount;
        }

        /*  
            **********************************************************************************************
                                            OPEN SOURCE CONTRIBUTION MAP ROW
            **********************************************************************************************
        */

        private OpenSourceContribution MapRowToOpenSourceContribution(NpgsqlDataReader reader)
        {
            OpenSourceContribution openSourceContribution = new OpenSourceContribution
            {
                Id = Convert.ToInt32(reader["id"]),
                ProjectName = Convert.ToString(reader["project_name"]),
                OrganizationName = Convert.ToString(reader["organization_name"]),
                StartDate = Convert.ToDateTime(reader["start_date"]),
                EndDate = Convert.ToDateTime(reader["end_date"]),
                ProjectDescription = Convert.ToString(reader["project_description"]),
                ContributionDetails = Convert.ToString(reader["contribution_details"])
            };

            int contributionId = openSourceContribution.Id;

            SetOpenSourceContributionLogoIdProperties(reader, openSourceContribution, contributionId);
            SetOpenSourceContributionOrganizationWebsiteIdProperties(reader, openSourceContribution, contributionId);
            SetOpenSourceContributionOrganizationGitHubIdProperties(reader, openSourceContribution, contributionId);
            SetOpenSourceContributionMainImageIdProperties(reader, openSourceContribution, contributionId);

            openSourceContribution.TechSkillsUtilized = _skillDao.GetSkillsByOpenSourceContributionId(contributionId);
            openSourceContribution.PullRequestsLinks = _websiteDao.GetPullRequestLinksByOpenSourceContributionId(contributionId);
            openSourceContribution.ReviewCommentsAndFeedbackReceived = _achievementDao.GetAchievementsByOpenSourceContributionId(contributionId);
            openSourceContribution.AdditionalImages = _imageDao.GetAdditionalImagesByOpenSourceContributionId(contributionId);

            return openSourceContribution;
        }

        private void SetOpenSourceContributionLogoIdProperties(NpgsqlDataReader reader, OpenSourceContribution contribution, int contributionId)
        {
            if (reader["organization_logo_id"] != DBNull.Value)
            {
                contribution.OrganizationLogoId = Convert.ToInt32(reader["organization_logo_id"]);

                contribution.OrganizationLogo = _imageDao.GetMainImageOrOrganizationLogoByOpenSourceContributionId(contributionId, Logo);
            }
            else
            {
                contribution.OrganizationLogoId = 0;
            }
        }

        private void SetOpenSourceContributionOrganizationWebsiteIdProperties(NpgsqlDataReader reader, OpenSourceContribution contribution, int contributionId)
        {
            if (reader["organization_website_id"] != DBNull.Value)
            {
                contribution.OrganizationWebsiteId = Convert.ToInt32(reader["organization_website_id"]);

                contribution.OrganizationWebsite = _websiteDao.GetWebsiteByOpenSourceContributionId(contributionId, contribution.OrganizationWebsiteId);
            }
            else
            {
                contribution.OrganizationWebsiteId = 0;
            }
        }

        private void SetOpenSourceContributionOrganizationGitHubIdProperties(NpgsqlDataReader reader, OpenSourceContribution contribution, int contributionId)
        {
            if (reader["organization_github_id"] != DBNull.Value)
            {
                contribution.OrganizationGitHubId = Convert.ToInt32(reader["organization_github_id"]);

                contribution.OrganizationGitHubRepo = _websiteDao.GetWebsiteByOpenSourceContributionId(contributionId, contribution.OrganizationGitHubId);
            }
            else
            {
                contribution.OrganizationGitHubId = 0;
            }
        }

        private void SetOpenSourceContributionMainImageIdProperties(NpgsqlDataReader reader, OpenSourceContribution contribution, int contributionId)
        {
            if (reader["main_image_id"] != DBNull.Value)
            {
                contribution.MainImageId = Convert.ToInt32(reader["main_image_id"]);

                contribution.MainImage = _imageDao.GetMainImageOrOrganizationLogoByOpenSourceContributionId(contributionId, MainImage);
            }
            else
            {
                contribution.MainImageId = 0;
            }
        }


    }
}