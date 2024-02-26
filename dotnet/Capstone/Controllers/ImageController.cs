using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Capstone.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageDao _imageDao;

        public ImageController(IImageDao imageDao)
        {
            _imageDao = imageDao;
        }
        // TODO uncomment and test out all IMAGE CONTROLLER METHODS
        /*  
            **********************************************************************************************
                                                IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        // [HttpPost]
        // public IActionResult CreateImage(Image image)
        // {
        //     try
        //     {
        //         Image createdImage = _imageDao.CreateImage(image);
        //         return Ok(createdImage);
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        // [HttpGet("{imageId}")]
        // public IActionResult GetImageById(int imageId)
        // {
        //     try
        //     {
        //         Image image = _imageDao.GetImageById(imageId);
        //         if (image == null)
        //         {
        //             return NotFound();
        //         }

        //         return Ok(image);
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, $"Internal server error: {ex.Message}");
        //     }
        // }

        [HttpGet("/get-all-images")]
        public IActionResult GetAllImages()
        {
            try
            {
                List<Image> images = _imageDao.GetAllImages();
                return Ok(images);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // [HttpPut]
        // public IActionResult UpdateImage(Image image)
        // {
        //     try
        //     {
        //         Image updatedImage = _imageDao.UpdateImage(image);
        //         if (updatedImage == null)
        //         {
        //             return NotFound();
        //         }

        //         return Ok(updatedImage);
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, $"Internal server error: {ex.Message}");
        //     }
        // }

        // [HttpDelete("{imageId}")]
        // public IActionResult DeleteImageById(int imageId)
        // {
        //     try
        //     {
        //         int rowsAffected = _imageDao.DeleteImageById(imageId);
        //         if (rowsAffected > 0)
        //         {
        //             return Ok();
        //         }
        //         else
        //         {
        //             return NotFound();
        //         }
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        /*  
            **********************************************************************************************
                                        SIDE PROJECT IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        // [HttpPost("project/{projectId}")]
        // public IActionResult CreateImageByProjectId(int projectId, Image image)
        // {
        //     try
        //     {
        //         Image createdImage = _imageDao.CreateImageByProjectId(projectId, image);
        //         return Ok(createdImage);
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        // [HttpGet("project/{projectId}")]
        // public IActionResult GetImagesByProjectId(int projectId)
        // {
        //     try
        //     {
        //         List<Image> images = _imageDao.GetImagesByProjectId(projectId);
        //         return Ok(images);
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        // [HttpPut("project/{projectId}")]
        // public IActionResult UpdateImageByProjectId(int projectId, Image image)
        // {
        //     try
        //     {
        //         Image updatedImage = _imageDao.UpdateImageByProjectId(projectId, image);
        //         if (updatedImage != null)
        //         {
        //             return Ok(updatedImage);
        //         }
        //         else
        //         {
        //             return NotFound();
        //         }
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        // [HttpDelete("project/{projectId}/{imageId}")]
        // public IActionResult DeleteImageByProjectId(int projectId, int imageId)
        // {
        //     try
        //     {
        //         int rowsAffected = _imageDao.DeleteImageByProjectId(projectId, imageId);
        //         if (rowsAffected > 0)
        //         {
        //             return Ok();
        //         }
        //         else
        //         {
        //             return NotFound();
        //         }
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        /*  
            **********************************************************************************************
                                        BLOG POST IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        // FIXME when you create a new image for a project, it replaces the old at mainImageId but the old image stays in database unconnected to anything

        [HttpPost("/blogpost/{blogPostId}/create-image")]
        public IActionResult CreateImageByBlogPostId(int blogPostId, Image image)
        {
            try
            {
                Image createdImage = _imageDao.CreateImageByBlogPostId(blogPostId, image);
                return Ok(createdImage);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/blogpost/{blogPostId}/image/{imageId}")]
        public IActionResult GetImageByBlogPostIdAndImageId(int blogPostId, int imageId)
        {
            try
            {
                Image image = _imageDao.GetImageByImageIdAndBlogPostId(imageId, blogPostId);
                if (image == null)
                {
                    return NotFound();
                }

                return Ok(image);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/blogpost/{blogPostId}/images")]
        public IActionResult GetImagesByBlogPostId(int blogPostId)
        {
            try
            {
                List<Image> images = _imageDao.GetImagesByBlogPostId(blogPostId);
                return Ok(images);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("/blogpost/{blogPostId}/update-image")]
        public IActionResult UpdateImageByBlogPostId(int blogPostId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByBlogPostId(blogPostId, image);
                if (updatedImage != null)
                {
                    return Ok(updatedImage);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("/blogpost/{blogPostId}/delete-image/{imageId}")]
        public IActionResult DeleteImageByBlogPostId(int blogPostId, int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageByBlogPostId(blogPostId, imageId);
                if (rowsAffected > 0)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /*  
            **********************************************************************************************
                                             WEBSITE IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        // [HttpPost("website/{websiteId}")]
        // public ActionResult<Image> CreateImageByWebsiteId(int websiteId, Image image)
        // {
        //     try
        //     {
        //         var createdImage = _imageDao.CreateImageByWebsiteId(websiteId, image);
        //         return Created($"/api/image/{createdImage.Id}", createdImage);
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        // [HttpGet("website/{websiteId}")]
        // public ActionResult<Image> GetImageByWebsiteId(int websiteId)
        // {
        //     var image = _imageDao.GetImageByWebsiteId(websiteId);
        //     if (image == null)
        //     {
        //         return NotFound();
        //     }
        //     return image;
        // }

        // [HttpGet("website/{websiteId}/all")]
        // public ActionResult<List<Image>> GetImagesByWebsiteId(int websiteId)
        // {
        //     var images = _imageDao.GetImagesByWebsiteId(websiteId);
        //     return images;
        // }

        // [HttpPut("website/{websiteId}")]
        // public ActionResult<Image> UpdateImageByWebsiteId(int websiteId, Image updatedImage)
        // {
        //     try
        //     {
        //         var image = _imageDao.UpdateImageByWebsiteId(websiteId, updatedImage);
        //         if (image == null)
        //         {
        //             return NotFound();
        //         }
        //         return image;
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        // [HttpDelete("website/{websiteId}/image/{imageId}")]
        // public IActionResult DeleteImageByWebsiteId(int websiteId, int imageId)
        // {
        //     try
        //     {
        //         var deletedRows = _imageDao.DeleteImageByWebsiteId(websiteId, imageId);
        //         if (deletedRows == 0)
        //         {
        //             return NotFound();
        //         }
        //         return NoContent();
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }
    }
}
