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
        Image GetImageById(int imageId);
        List<Image> GetAllImages();
        Image UpdateImage(Image image);
        int DeleteImageById(int imageId);
        int DeleteImageByProjectId(int projectId, int imageId);
    }
}