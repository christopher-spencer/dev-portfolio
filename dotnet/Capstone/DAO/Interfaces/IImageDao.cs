using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IImageDao
    {
        /*  
            *******************
                IMAGE CRUD
            *******************
        */
        Image CreateImage(Image image);
        Image GetImageById(int imageId);
        List<Image> GetAllImages();
        Image UpdateImage(Image image);
        int DeleteImageById(int imageId);


        /*  
            ****************************
               SIDE PROJECT IMAGE CRUD
            ****************************
        */
        Image CreateImageByProjectId(int projectId, Image image);
        List<Image> GetImagesByProjectId(int projectId);
        Image GetImageByProjectId(int projectId);
        Image GetImageByProjectIdAndImageId(int projectId, int imageId);
        Image UpdateImageByProjectId(int projectId, Image updatedImage);
        int DeleteImageByProjectId(int projectId, int imageId);


        /*  
            ****************************
                BLOGPOST IMAGE CRUD
            ****************************
        */
        Image CreateImageByBlogPostId(int blogPostId, Image image);
        Image GetImageByImageIdAndBlogPostId(int imageId, int blogPostId);
        List<Image> GetImagesByBlogPostId(int blogPostId);
        Image UpdateImageByBlogPostId(int blogPostId, Image updatedImage);
        int DeleteImageByBlogPostId(int blogPostId, int imageId);
    }
}