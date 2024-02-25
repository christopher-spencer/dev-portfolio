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

        [HttpPost]
        public IActionResult CreateImage(Image image)
        {
            try
            {
                Image createdImage = _imageDao.CreateImage(image);
                return Ok(createdImage);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("project/{projectId}")]
        public IActionResult CreateImageByProjectId(int projectId, Image image)
        {
            try
            {
                Image createdImage = _imageDao.CreateImageByProjectId(projectId, image);
                return Ok(createdImage);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("blogpost/{blogPostId}/create-image")]
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

        [HttpGet("project/{projectId}")]
        public IActionResult GetImagesByProjectId(int projectId)
        {
            try
            {
                List<Image> images = _imageDao.GetImagesByProjectId(projectId);
                return Ok(images);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("blogpost/{blogPostId}")]
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

        [HttpPut("project/{projectId}")]
        public IActionResult UpdateImageByProjectId(int projectId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByProjectId(projectId, image);
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

        [HttpPut("blogpost/{blogPostId}/update-image")]
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

        [HttpDelete("{imageId}")]
        public IActionResult DeleteImageById(int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageById(imageId);
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

        [HttpDelete("project/{projectId}/{imageId}")]
        public IActionResult DeleteImageByProjectId(int projectId, int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageByProjectId(projectId, imageId);
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

        [HttpDelete("blogpost/{blogPostId}/{imageId}")]
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
    }
}
