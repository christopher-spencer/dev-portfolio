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
        Image GetImageById(int imageId);
        List<Image> GetAllImages();
        Image UpdateImage(Image image, int imageId);
        int DeleteImageById(int imageId);

        /*  
            **********************************************************************************************
                                            SIDE PROJECT IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageBySideProjectId(int projectId, Image image);
        List<Image> GetImagesBySideProjectId(int projectId);
        Image GetImageBySideProjectId(int sideProjectId, int imageId);
        Image GetImageBySideProjectIdAndImageId(int projectId, int imageId);
        Image UpdateImageBySideProjectId(int projectId, int imageId, Image image);
        int DeleteImageBySideProjectId(int projectId, int imageId);

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
        Image GetImageByWebsiteId(int websiteId);
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
        Image UpdateImageByGoalId(int goalId, Image image);
        int DeleteImageByGoalId(int goalId, int imageId);

        /*  
            **********************************************************************************************
                                            CONTRIBUTOR IMAGE CRUD
            **********************************************************************************************
        */ 
        Image CreateImageByContributorId(int contributorId, Image image);
        Image GetImageByContributorId(int contributorId);
        Image UpdateImageByContributorId(int contributorId, Image image);
        int DeleteImageByContributorId(int contributorId, int imageId);

        /*  
            **********************************************************************************************
                                        API AND SERVICE IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByApiServiceId(int apiServiceId, Image image);
        Image GetImageByApiServiceId(int apiServiceId);
        Image UpdateImageByApiServiceId(int apiServiceId, Image image);
        int DeleteImageByApiServiceId(int apiServiceId, int imageId);

        /*  
            **********************************************************************************************
                                        DEPENDENCY AND LIBRARY IMAGE CRUD
            **********************************************************************************************
        */

        Image CreateImageByDependencyLibraryId(int dependencyLibraryId, Image image);
        Image GetImageByDependencyLibraryId(int dependencyLibraryId);
        Image UpdateImageByDependencyLibraryId(int dependencyLibraryId, Image image);
        int DeleteImageByDependencyLibraryId(int dependencyLibraryId, int imageId);
    }
}