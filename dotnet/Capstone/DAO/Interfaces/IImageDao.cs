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
        Image UpdateImageBySideProjectId(int projectId, Image updatedImage);
        int DeleteImageBySideProjectId(int projectId, int imageId);

        /*  
            **********************************************************************************************
                                            BLOG POST IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageByBlogPostId(int blogPostId, Image image);
        Image GetImageByImageIdAndBlogPostId(int imageId, int blogPostId);
        List<Image> GetImagesByBlogPostId(int blogPostId);
        Image UpdateImageByBlogPostId(int blogPostId, Image updatedImage);
        int DeleteImageByBlogPostId(int blogPostId, int imageId);

          /*  
            **********************************************************************************************
                                             WEBSITE IMAGE CRUD
            **********************************************************************************************
        */

        Image CreateImageByWebsiteId(int websiteId, Image image);
        Image GetImageByWebsiteId(int websiteId);
        Image UpdateImageByWebsiteId(int websiteId, Image updatedImage);
        int DeleteImageByWebsiteId(int websiteId, int imageId);

        /*  
            **********************************************************************************************
                                            SKILL IMAGE CRUD
            **********************************************************************************************
        */

        Image CreateImageBySkillId(int skillId, Image image);
        Image GetImageBySkillId(int skillId);
        Image UpdateImageBySkillId(int skillId, Image updatedImage);
        int DeleteImageBySkillId(int skillId, int imageId);

        /*  
            **********************************************************************************************
                                            GOAL IMAGE CRUD
            **********************************************************************************************
        */

        Image CreateImageByGoalId(int goalId, Image image);
        Image GetImageByGoalId(int goalId);
        Image UpdateImageByGoalId(int goalId, Image updatedImage);
        int DeleteImageByGoalId(int goalId, int imageId);

    }
}