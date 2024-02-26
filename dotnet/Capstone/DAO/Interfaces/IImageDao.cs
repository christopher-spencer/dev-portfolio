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
        Image UpdateImage(Image image);
        int DeleteImageById(int imageId);

        /*  
            **********************************************************************************************
                                            SIDE PROJECT IMAGE CRUD
            **********************************************************************************************
        */
        Image CreateImageBySideProjectId(int projectId, Image image);
        List<Image> GetImagesBySideProjectId(int projectId);
        Image GetImageBySideProjectId(int projectId);
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
        List<Image> GetImagesByWebsiteId(int websiteId);
        Image UpdateImageByWebsiteId(int websiteId, Image updatedImage);
        int DeleteImageByWebsiteId(int websiteId, int imageId);
    }
}