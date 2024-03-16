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
        // TODO when creating new project image, replaces old at mainImageId but old img stays in database unconnected to anything or sets it in additional images for models w/ additional images
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
                    return CreatedAtAction(nameof(GetImage), new { imageId = createdImage.Id }, createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the image.");
            }
        }

        [HttpGet("/image/{imageId}")]
        public ActionResult<Image> GetImage(int imageId)
        {
            Image image = _imageDao.GetImage(imageId);

            if (image == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(image);
            }
        }

        [HttpGet("/images")]
        public ActionResult<List<Image>> GetImages()
        {
            List<Image> images = _imageDao.GetImages();

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
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the image.");
            }
        }

        [Authorize]
        [HttpDelete("/delete-image/{imageId}")]
        public ActionResult DeleteImage(int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImage(imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the image.");
            }
        }

        /*  
            **********************************************************************************************
                                        SIDE PROJECT IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/sideproject/{sideProjectId}/create-image")]
        public ActionResult CreateImageBySideProjectId(int sideProjectId, Image image)
        {
            try
            {
                Image createdImage = _imageDao.CreateImageBySideProjectId(sideProjectId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetImageBySideProjectId), new { sideProjectId, imageId = createdImage.Id }, createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the side project image.");
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/images")]
        public ActionResult GetImagesBySideProjectId(int sideProjectId)
        {
            List<Image> images = _imageDao.GetImagesBySideProjectId(sideProjectId);

            if (images == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(images);
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/image/{imageId}")]
        public ActionResult<Image> GetImageBySideProjectId(int sideProjectId, int imageId)
        {
            Image image = _imageDao.GetImageBySideProjectId(sideProjectId, imageId);

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
        [HttpPut("/update-sideproject/{sideProjectId}/update-image/{imageId}")]
        public ActionResult UpdateImageBySideProjectId(int sideProjectId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageBySideProjectId(sideProjectId, imageId, image);

                if (updatedImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the side project image.");
            }
        }
// FIXME getting 500 error in postman
        [Authorize]
        [HttpDelete("/sideproject/{sideProjectId}/delete-image/{imageId}")]
        public ActionResult DeleteImageBySideProjectId(int sideProjectId, int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageBySideProjectId(sideProjectId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the blog post.");
            }
        }

        /*  
            **********************************************************************************************
                                        BLOG POST IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

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
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating blog post image.");
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
        [HttpPut("/update-blogpost/{blogPostId}/update-image/{imageId}")]
        public ActionResult UpdateImageByBlogPostId(int blogPostId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByBlogPostId(blogPostId, imageId, image);

                if (updatedImage == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(updatedImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the blog post image.");
            }
        }

        [Authorize]
        [HttpDelete("/blogpost/{blogPostId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByBlogPostId(int blogPostId, int imageId)
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
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the blog post image.");
            }
        }

        /*  
            **********************************************************************************************
                                          WEBSITE IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

//FIXME having similar issue from before in another where when i create a new imagebywebsiteid it doesnt link it and put in to websiteid in Dependency Library. The websiteId is linked, but not its associated image*********        
        [Authorize]
        [HttpPost("/website/{websiteId}/create-image")]
        public ActionResult CreateImageByWebsiteId(int websiteId, Image image)
        {
            try
            {
                Image createdLogo = _imageDao.CreateImageByWebsiteId(websiteId, image);

                if (createdLogo == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(createdLogo);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the website logo.");
            }
        }

        [HttpGet("/website/{websiteId}/image")]
        public ActionResult<Image> GetImageByWebsiteId(int websiteId)
        {
            Image logo = _imageDao.GetImageByWebsiteId(websiteId);

            if (logo == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(logo);
            }
        }

        [Authorize]
        [HttpPut("/update-website/{websiteId}/update-image/{imageId}")]
        public ActionResult<Image> UpdateImageByWebsiteId(int websiteId, int imageId, Image logo)
        {
            try
            {
                Image updatedLogo = _imageDao.UpdateImageByWebsiteId(websiteId, imageId, logo); 

                if (updatedLogo == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(updatedLogo);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the website logo.");
            }
        }

        [Authorize]
        [HttpDelete("/website/{websiteId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByWebsiteId(int websiteId, int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageByWebsiteId(websiteId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Website logo was deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the website logo.");
            }
        }

        /*  
            **********************************************************************************************
                                            SKILL IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/skill/{skillId}/create-image")]
        public ActionResult CreateImageBySkillId(int skillId, Image image)
        {
            try
            {
                Image createdImage = _imageDao.CreateImageBySkillId(skillId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetImageBySkillId), new { skillId, imageId = createdImage.Id }, createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the skill image.");
            }
        }

        [HttpGet("/skill/{skillId}/image")]
        public ActionResult<Image> GetImageBySkillId(int skillId)
        {
            Image image = _imageDao.GetImageBySkillId(skillId);

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
        [HttpPut("/skill/{skillId}/update-image/{imageId}")]
        public ActionResult<Image> UpdateImageBySkillId(int skillId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageBySkillId(skillId, imageId, image);

                if (updatedImage == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(updatedImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the skill image.");
            }
        }
//FIXME successfully deleted an image, but gave 500 error (DELETED image FROM skill, but image is still in GETIMAGES***)
        [Authorize]
        [HttpDelete("/skill/{skillId}/delete-image/{imageId}")]
        public ActionResult DeleteImageBySkillId(int skillId, int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageBySkillId(skillId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Skill image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the skill image.");
            }
        }

        /*  
            **********************************************************************************************
                                        GOAL IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/goal/{goalId}/create-image")]
        public ActionResult CreateImageByGoalId(int goalId, Image image)
        {
            try
            {
                Image createdImage = _imageDao.CreateImageByGoalId(goalId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the goal image.");
            }
        }

        [HttpGet("/goal/{goalId}/image")]
        public ActionResult<Image> GetImageByGoalId(int goalId)
        {
            Image image = _imageDao.GetImageByGoalId(goalId);

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
        [HttpPut("/goal/{goalId}/update-image/{imageId}")]
        public ActionResult UpdateImageByGoalId(int goalId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByGoalId(goalId, imageId, image);

                if (updatedImage == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(updatedImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the goal image.");
            }
        }

        [Authorize]
        [HttpDelete("/goal/{goalId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByGoalId(int goalId, int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageByGoalId(goalId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Goal image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the goal image.");
            }
        }

        /*  
            **********************************************************************************************
                                        CONTRIBUTOR IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/contributor/{contributorId}/create-image")]
        public ActionResult CreateImageByContributorId(int contributorId, Image image)
        {
            try
            {
                Image createdImage = _imageDao.CreateImageByContributorId(contributorId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the contributor image.");
            }
        }

        [HttpGet("/contributor/{contributorId}/image")]
        public ActionResult<Image> GetImageByContributorId(int contributorId)
        {
            Image image = _imageDao.GetImageByContributorId(contributorId);

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
        [HttpPut("/contributor/{contributorId}/update-image/{imageId}")]
        public ActionResult UpdateImageByContributorId(int contributorId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByContributorId(contributorId, imageId, image);

                if (updatedImage == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(updatedImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the contributor image.");
            }
        }

        [Authorize]
        [HttpDelete("/contributor/{contributorId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByContributorId(int contributorId, int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageByContributorId(contributorId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Contributor image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the contributor image.");
            }
        }

        /*  
            **********************************************************************************************
                                        API AND SERVICE IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/api-service/{apiServiceId}/create-image")]
        public ActionResult CreateImageByApiServiceId(int apiServiceId, Image image)
        {
            try
            {
                Image createdImage = _imageDao.CreateImageByApiServiceId(apiServiceId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the API/service image.");
            }
        }

        [HttpGet("/api-service/{apiServiceId}/image")]
        public ActionResult<Image> GetImageByApiServiceId(int apiServiceId)
        {
            Image image = _imageDao.GetImageByApiServiceId(apiServiceId);

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
        [HttpPut("/api-service/{apiServiceId}/update-image/{imageId}")]
        public ActionResult UpdateImageByApiServiceId(int apiServiceId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByApiServiceId(apiServiceId, imageId, image);

                if (updatedImage == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(updatedImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the API/service image.");
            }
        }

        [Authorize]
        [HttpDelete("/api-service/{apiServiceId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByApiServiceId(int apiServiceId, int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageByApiServiceId(apiServiceId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("API/service image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the API/service image.");
            }
        }

        /*  
            **********************************************************************************************
                                    DEPENDENCY AND LIBRARY IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/dependency-library/{dependencyLibraryId}/create-image")]
        public ActionResult CreateImageByDependencyLibraryId(int dependencyLibraryId, Image image)
        {
            try
            {
                Image createdImage = _imageDao.CreateImageByDependencyLibraryId(dependencyLibraryId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the dependency/library image.");
            }
        }

        [HttpGet("/dependency-library/{dependencyLibraryId}/image")]
        public ActionResult<Image> GetImageByDependencyLibraryId(int dependencyLibraryId)
        {
            Image image = _imageDao.GetImageByDependencyLibraryId(dependencyLibraryId);

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
        [HttpPut("/dependency-library/{dependencyLibraryId}/update-image/{imageId}")]
        public ActionResult UpdateImageByDependencyLibraryId(int dependencyLibraryId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByDependencyLibraryId(dependencyLibraryId, imageId, image);

                if (updatedImage == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(updatedImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the dependency/library image.");
            }
        }

        [Authorize]
        [HttpDelete("/dependency-library/{dependencyLibraryId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByDependencyLibraryId(int dependencyLibraryId, int imageId)
        {
            try
            {
                int rowsAffected = _imageDao.DeleteImageByDependencyLibraryId(dependencyLibraryId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Dependency/library image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the dependency/library image.");
            }
        }
    }
}
