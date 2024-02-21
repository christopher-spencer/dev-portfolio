using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IImageDao
    {
        Image CreateImage(Image image);
        Image GetImageById(int imageId);
        List<Image> GetAllImages();
        Image UpdateImage(Image image);
        int DeleteImageById(int imageId);
    }
}