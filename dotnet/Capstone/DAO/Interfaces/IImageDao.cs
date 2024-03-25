using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IImageDao
    {
        /*  
            **********************************************************************************************
                                                    IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImage(Image image);
        Image GetImage(int imageId);
        List<Image> GetImages();
        Image UpdateImage(Image image, int imageId);
        int DeleteImage(int imageId);

        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                            PORTFOLIO IMAGE CRUD
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */
        Image CreateImageByPortfolioId(int portfolioId, Image image);
        Image GetMainImageByPortfolioId(int portfolioId);
        Image GetImageByPortfolioId(int portfolioId, int imageId);
        List<Image> GetAllImagesByPortfolioId(int portfolioId);
        List<Image> GetAdditionalImagesByPortfolioId(int portfolioId);
        Image UpdateImageByPortfolioId(int portfolioId, int imageId, Image image);
        Image UpdateMainImageByPortfolioId(int portfolioId, int mainImageId, Image mainImage);
        int DeleteImageByPortfolioId(int portfolioId, int imageId);
       /*  
            **********************************************************************************************
                                           WORK EXPERIENCE IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByWorkExperienceId(int experienceId, Image image);
        Image GetMainImageOrCompanyLogoByWorkExperienceId(int experienceId, string imageType);
        Image GetImageByWorkExperienceId(int experienceId, int imageId);
        List<Image> GetAdditionalImagesByWorkExperienceId(int experienceId);
        Image UpdateImageByWorkExperienceId(int experienceId, int imageId, Image image);
        Image UpdateMainImageOrLogoByWorkExperienceId(int experienceId, int imageId, Image image);
        int DeleteImageByWorkExperienceId(int experienceId, int imageId);

        /*  
            **********************************************************************************************
                                            CREDENTIAL IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByCredentialId(int credentialId, Image image);
        Image GetMainImageOrOrganizationLogoByCredentialId(int credentialId, string imageType);
        Image UpdateMainImageOrOrganizationLogoByCredentialId(int credentialId, int imageId, Image image);
        int DeleteImageByCredentialId(int credentialId, int imageId);

        /*  
            **********************************************************************************************
                                            EDUCATION IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByEducationId(int educationId, Image image);
        Image GetMainImageOrInstitutionLogoByEducationId(int educationId, string imageType);
        Image GetImageByEducationId(int educationId, int imageId);
        List<Image> GetAdditionalImagesByEducationId(int educationId);
        Image UpdateImageByEducationId(int educationId, int imageId, Image image);
        Image UpdateMainImageOrLogoByEducationId(int educationId, int imageId, Image image);
        int DeleteImageByEducationId(int educationId, int imageId);

        /*  
            **********************************************************************************************
                                        OPEN SOURCE CONTRIBUTION IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByOpenSourceContributionId(int contributionId, Image image);
        Image GetMainImageOrOrganizationLogoByOpenSourceContributionId(int contributionId, string imageType);
        Image GetImageByOpenSourceContributionId(int contributionId, int imageId);
        List<Image> GetAdditionalImagesByOpenSourceContributionId(int contributionId);
        Image UpdateImageByOpenSourceContributionId(int contributionId, int imageId, Image image);
        Image UpdateMainImageOrLogoByOpenSourceContributionId(int contributionId, int imageId, Image image);
        int DeleteImageByOpenSourceContributionId(int openSourceContributionId, int imageId);

        /*  
            **********************************************************************************************
                                            VOLUNTEER WORK IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByVolunteerWorkId(int volunteerWorkId, Image image);
        Image GetMainImageOrOrganizationLogoByVolunteerWorkId(int volunteerWorkId, string imageType);
        Image GetImageByVolunteerWorkId(int volunteerWorkId, int imageId);
        List<Image> GetAdditionalImagesByVolunteerWorkId(int volunteerWorkId);
        Image UpdateImageByVolunteerWorkId(int volunteerWorkId, int imageId, Image image);
        Image UpdateMainImageOrLogoByVolunteerWorkId(int volunteerWorkId, int imageId, Image image);
        int DeleteImageByVolunteerWorkId(int volunteerWorkId, int imageId);

        /*  
            **********************************************************************************************
                                            ACHIEVEMENT IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByAchievementId(int achievementId, Image image);
        Image GetImageByAchievementId(int achievementId);
        Image UpdateImageByAchievementId(int achievementId, int imageId, Image image);
        int DeleteImageByAchievementId(int achievementId, int imageId);

        /*  
            **********************************************************************************************
                                            HOBBY IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByHobbyId(int hobbyId, Image image);
        Image GetImageByHobbyId(int hobbyId);
        Image UpdateImageByHobbyId(int hobbyId, int imageId, Image image);
        int DeleteImageByHobbyId(int hobbyId, int imageId);

        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                            SIDE PROJECT IMAGE CRUD
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */
        Image CreateImageBySideProjectId(int sideProjectId, Image image);
        Image GetMainImageBySideProjectId(int sideProjectId);
        List<Image> GetAllImagesBySideProjectId(int sideProjectId);
        List<Image> GetAdditionalImagesBySideProjectId(int sideProjectId);
        Image GetImageBySideProjectId(int sideProjectId, int imageId);
        Image UpdateImageBySideProjectId(int sideProjectId, int imageId, Image image);
        Image UpdateMainImageBySideProjectId(int sideProjectId, int mainImageId, Image mainImage);
        int DeleteImageBySideProjectId(int sideProjectId, int imageId);

        /*  
            **********************************************************************************************
                                            BLOG POST IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByBlogPostId(int blogPostId, Image image);
        Image GetImageByImageIdAndBlogPostId(int imageId, int blogPostId);
        List<Image> GetImagesByBlogPostId(int blogPostId);
        Image UpdateImageByBlogPostId(int blogPostId, int imageId, Image image);
        int DeleteImageByBlogPostId(int blogPostId, int imageId);

          /*  
            **********************************************************************************************
                                             WEBSITE IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByWebsiteId(int websiteId, Image image);
        Image GetImageByWebsiteId(int websiteId, int imageId);
        Image UpdateImageByWebsiteId(int websiteId, int imageId, Image image);
        int DeleteImageByWebsiteId(int websiteId, int imageId);

        /*  
            **********************************************************************************************
                                            SKILL IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageBySkillId(int skillId, Image image);
        Image GetImageBySkillId(int skillId);
        Image UpdateImageBySkillId(int skillId, int imageId, Image image);
        int DeleteImageBySkillId(int skillId, int imageId);

        /*  
            **********************************************************************************************
                                            GOAL IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByGoalId(int goalId, Image image);
        Image GetImageByGoalId(int goalId);
        Image UpdateImageByGoalId(int goalId, int imageId, Image image);
        int DeleteImageByGoalId(int goalId, int imageId);

        /*  
            **********************************************************************************************
                                            CONTRIBUTOR IMAGE CRUD
            **********************************************************************************************
        */ 
        Image CreateImageByContributorId(int contributorId, Image image);
        Image GetImageByContributorId(int contributorId);
        Image UpdateImageByContributorId(int contributorId, int imageId, Image image);
        int DeleteImageByContributorId(int contributorId, int imageId);

        /*  
            **********************************************************************************************
                                        API AND SERVICE IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByApiServiceId(int apiServiceId, Image image);
        Image GetImageByApiServiceId(int apiServiceId);
        Image UpdateImageByApiServiceId(int apiServiceId, int imageId, Image image);
        int DeleteImageByApiServiceId(int apiServiceId, int imageId);

        /*  
            **********************************************************************************************
                                        DEPENDENCY AND LIBRARY IMAGE CRUD
            **********************************************************************************************
        */

        Image CreateImageByDependencyLibraryId(int dependencyLibraryId, Image image);
        Image GetImageByDependencyLibraryId(int dependencyLibraryId);
        Image UpdateImageByDependencyLibraryId(int dependencyLibraryId, int imageId, Image image);
        int DeleteImageByDependencyLibraryId(int dependencyLibraryId, int imageId);

        /*  
            **********************************************************************************************
                                                HELPER METHODS
            **********************************************************************************************
        */

        int? GetImageIdByWebsiteId(int websiteId);
    }
}