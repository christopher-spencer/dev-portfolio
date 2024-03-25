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