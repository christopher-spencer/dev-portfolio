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
//TODO wrap all CREATE/UPDATE/DELETE in try catch
        /*  
            **********************************************************************************************
                                                IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/create-image")]
        public ActionResult CreateImage(Image image)
        {
            try
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
            catch (DaoException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
            try 
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
            catch (DaoException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

        // TODO check out "ex.Message"
        [Authorize]
        [HttpPost("sideproject/{projectId}/create-image")]
        public ActionResult CreateImageBySideProjectId(int projectId, Image image)
        {
            try
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
            catch (DaoException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("sideproject/{projectId}/images")]
        public IActionResult GetImagesBySideProjectId(int projectId)
        {
            List<Image> images = _imageDao.GetImagesBySideProjectId(projectId);

            if (images == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(images);               
            }
        }

        [HttpGet("/sideproject/{projectId}/image/{imageId}")]
        public ActionResult<Image> GetImageBySideProjectId(int projectId, int imageId)
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

        [Authorize]
        [HttpPut("update-sideproject/{projectId}")]
        public ActionResult UpdateImageBySideProjectId(int projectId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageBySideProjectId(projectId, image);
                if (updatedImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedImage);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while updating the side project image.");
            }
        }

        [Authorize]
        [HttpDelete("sideproject/{projectId}/delete-image/{imageId}")]
        public IActionResult DeleteImageBySideProjectId(int projectId, int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageBySideProjectId(projectId, imageId);

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
                return StatusCode(500, "An error occurred while deleting the blog post.");
            }
        }

        /*  
            **********************************************************************************************
                                        BLOG POST IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        // FIXME when you create a new image for a project, it replaces the old at mainImageId but the old image stays in database unconnected to anything

        
        [Authorize]
        [HttpPost("/blogpost/{blogPostId}/create-image")]
        public ActionResult CreateImageByBlogPostId(int blogPostId, Image image)
        {
            try
            {
                Image createdImage = _imageDao.CreateImageByBlogPostId(blogPostId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(createdImage);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/blogpost/{blogPostId}/image/{imageId}")]
        public ActionResult GetImageByBlogPostIdAndImageId(int blogPostId, int imageId)
        {
            Image image = _imageDao.GetImageByImageIdAndBlogPostId(imageId, blogPostId);

            if (image == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(image);
            }

        }

        [HttpGet("/blogpost/{blogPostId}/images")]
        public ActionResult GetImagesByBlogPostId(int blogPostId)
        {
            List<Image> images = _imageDao.GetImagesByBlogPostId(blogPostId);

            if (images == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(images);
            }
        }

        [Authorize]
        [HttpPut("/update-blogpost/{blogPostId}")]
        public ActionResult UpdateImageByBlogPostId(int blogPostId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByBlogPostId(blogPostId, image);

                if (updatedImage == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(updatedImage);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while updating the blog post image.");
            }
        }

        [Authorize]
        [HttpDelete("/blogpost/{blogPostId}/delete-image/{imageId}")]
        public IActionResult DeleteImageByBlogPostId(int blogPostId, int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageByBlogPostId(blogPostId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Blog post image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while deleting the blog post image.");
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
