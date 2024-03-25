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

        /*  
            **********************************************************************************************
                                        PORTFOLIO OPEN SOURCE CONTRIBUTION CRUD
            **********************************************************************************************
        */

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
                        cmd.Parameters.AddWithValue("contributionId", contributionId);

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
                        cmd.Parameters.AddWithValue("contributionId", contributionId);

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
                        cmd.Parameters.AddWithValue("contributionId", contributionId);

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
                        cmd.Parameters.AddWithValue("contributionId", contributionId);

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
            List<Website> websites = _websiteDao.GetWebsitesByOpenSourceContributionId(contributionId);

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
            openSourceContribution.PullRequestsLinks = _websiteDao.GetWebsitesByOpenSourceContributionId(contributionId);
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