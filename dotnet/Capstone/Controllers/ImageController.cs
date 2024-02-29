using System.Collections.Generic;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [Authorize]
        [HttpPost("/create-image")]
        public ActionResult CreateImage(Image image)
        {
             Image createdImage = _imageDao.CreateImage(image);
            
            if (createdImage == null)
            {
                return BadRequest();
            }
            else
            {
                return CreatedAtAction(nameof(GetImageById), new { imageId = createdImage.Id }, createdImage);
            }
        }

        [HttpGet("/image/{imageId}")]
        public ActionResult<Image> GetImageById(int imageId)
        {
            Image image = _imageDao.GetImageById(imageId);

            if (image == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(image);
            }
        }

        [HttpGet("/get-all-images")]
        public ActionResult<List<Image>> GetAllImages()
        {
            List<Image> images = _imageDao.GetAllImages();

            if (images == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(images);
            }
        }

        [Authorize]
        [HttpPut("/update-image/{imageId}/")]
        public ActionResult UpdateImage(Image image, int imageId)
        {
            Image updatedImage = _imageDao.UpdateImage(image, imageId);

            if (updatedImage == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(updatedImage);
            }
        }

        [Authorize]
        [HttpDelete("/image/delete/{imageId}")]
        public IActionResult DeleteImageById(int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageById(imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while deleting the image.");
            }
        }

        /*  
            **********************************************************************************************
                                        SIDE PROJECT IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [HttpPost("sideproject/{projectId}/create-image")]
        public ActionResult CreateImageBySideProjectId(int projectId, Image image)
        {
            Image createdImage = _imageDao.CreateImageBySideProjectId(projectId, image);
        
            if (createdImage == null)
            {
                return BadRequest();
            }
            else
            {
                return CreatedAtAction(nameof(GetImageBySideProjectId), new { imageId = createdImage.Id}, createdImage);
            }
        }

        // [HttpGet("project/{projectId}")]
        // public IActionResult GetImagesBySideProjectId(int projectId)
        // {
        //     try
        //     {
        //         List<Image> images = _imageDao.GetImagesBySideProjectId(projectId);
        //         return Ok(images);
        //     }
        //     catch (DaoException ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

//TODO wrap all in try catch
        [HttpGet("/sideproject/{projectId}/image/{imageId}")]
        public ActionResult<Image> GetImageBySideProjectId(int projectId, int imageId)
        {
            try
            {
                Image image = _imageDao.GetImageBySideProjectId(projectId, imageId);
                if (image == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(image);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // [HttpPut("project/{projectId}")]
        // public IActionResult UpdateImageBySideProjectId(int projectId, Image image)
        // {
        //     try
        //     {
        //         Image updatedImage = _imageDao.UpdateImageBySideProjectId(projectId, image);
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
        // public IActionResult DeleteImageBySideProjectId(int projectId, int imageId)
        // {
        //     try
        //     {
        //         int rowsAffected = _imageDao.DeleteImageBySideProjectId(projectId, imageId);
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
