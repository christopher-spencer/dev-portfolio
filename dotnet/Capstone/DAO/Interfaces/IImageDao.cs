using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IImageDao
    {
        Image CreateImage(Image image);
        Image CreateImageByProjectId(int projectId, Image image);
        List<Image> GetImagesByProjectId(int projectId);
        Image GetImageByProjectId(int projectId);
        Image GetImageByProjectIdAndImageId(int projectId, int imageId);
        Image GetImageByImageIdAndBlogPostId(int imageId, int blogPostId);
        Image GetImageById(int imageId);
        List<Image> GetAllImages();
        Image UpdateImageByProjectId(int projectId, Image updatedImage);
        Image UpdateImage(Image image);
        int DeleteImageById(int imageId);
        int DeleteImageByProjectId(int projectId, int imageId);
    }
}