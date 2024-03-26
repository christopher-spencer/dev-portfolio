using System.Collections.Generic;
using Capstone.DAO;
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

        const string MainImage = "main image";
        const string AdditionalImage = "additional image";
        const string Logo = "logo";

        /*  
            **********************************************************************************************
                                                IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */ 

        // [Authorize]
        // [HttpPost("/create-image")]
        // public ActionResult CreateImage(Image image)
        // {
        //     try
        //     {
        //         Image createdImage = _imageDao.CreateImage(image);

        //         if (createdImage == null)
        //         {
        //             return BadRequest();
        //         }
        //         else
        //         {
        //             return CreatedAtAction(nameof(GetImage), new { imageId = createdImage.Id }, createdImage);
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while creating the image.");
        //     }
        // }

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

        // [Authorize]
        // [HttpPut("/update-image/{imageId}/")]
        // public ActionResult UpdateImage(Image image, int imageId)
        // {
        //     try
        //     {
        //         Image updatedImage = _imageDao.UpdateImage(image, imageId);

        //         if (updatedImage == null)
        //         {
        //             return BadRequest();
        //         }
        //         else
        //         {
        //             return Ok(updatedImage);
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while updating the image.");
        //     }
        // }

        // [Authorize]
        // [HttpDelete("/delete-image/{imageId}")]
        // public ActionResult DeleteImage(int imageId)
        // {
        //     try
        //     {
        //         int rowsAffected = _imageDao.DeleteImage(imageId);

        //         if (rowsAffected > 0)
        //         {
        //             return Ok("Image deleted successfully.");
        //         }
        //         else
        //         {
        //             return NotFound();
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while deleting the image.");
        //     }
        // }

        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                        PORTFOLIO IMAGE CRUD CONTROLLER
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/portfolio/{portfolioId}/create-image")]
        public ActionResult CreateImageByPortfolioId(int portfolioId, Image image)
        {
            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != AdditionalImage)
            {
                return BadRequest("Invalid image type. Allowed values are 'main image' and 'additional image'.");
            }

            try
            {
                Image createdImage = _imageDao.CreateImageByPortfolioId(portfolioId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetImageByPortfolioId), new { portfolioId, imageId = createdImage.Id }, createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the portfolio image.");
            }
        }

        [HttpGet("/portfolio/{portfolioId}/main-image")]
        public ActionResult GetMainImageByPortfolioId(int portfolioId)
        {
            Image mainImage = _imageDao.GetMainImageByPortfolioId(portfolioId);

            if (mainImage == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(mainImage);
            }
        }

        [HttpGet("/portfolio/{portfolioId}/image/{imageId}")]
        public ActionResult<Image> GetImageByPortfolioId(int portfolioId, int imageId)
        {
            Image image = _imageDao.GetImageByPortfolioId(portfolioId, imageId);

            if (image == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(image);
            }
        }

        [HttpGet("/portfolio/{portfolioId}/all-images")]
        public ActionResult GetAllImagesByPortfolioId(int portfolioId)
        {
            List<Image> images = _imageDao.GetAllImagesByPortfolioId(portfolioId);

            if (images == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(images);
            }
        }

        [HttpGet("/portfolio/{portfolioId}/additional-images")]
        public ActionResult GetAdditionalImagesByPortfolioId(int portfolioId)
        {
            List<Image> additionalImages = _imageDao.GetAdditionalImagesByPortfolioId(portfolioId);

            if (additionalImages == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(additionalImages);
            }
        }

        [Authorize]
        [HttpPut("/portfolio/{portfolioId}/update-image/{imageId}")]
        public ActionResult UpdateImageByPortfolioId(int portfolioId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByPortfolioId(portfolioId, imageId, image);

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
                return StatusCode(500, "An error occurred while updating the portfolio image.");
            }
        }

        [Authorize]
        [HttpPut("/portfolio/{portfolioId}/update-main-image/{mainImageId}")]
        public ActionResult UpdateMainImageByPortfolioId(int portfolioId, int mainImageId, Image mainImage)
        {
            try
            {
                Image updatedMainImage = _imageDao.UpdateMainImageByPortfolioId(portfolioId, mainImageId, mainImage);

                if (updatedMainImage == null)
                {
                    return BadRequest("The image is null. Please provide a main image.");
                }
                else if (updatedMainImage.Type != MainImage)
                {
                    return BadRequest("The provided image is not a main image. Please provide a main image.");
                }
                else
                {
                    return Ok(updatedMainImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the portfolio main image.");
            }
        }

        [Authorize]
        [HttpDelete("/portfolio/{portfolioId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByPortfolioId(int portfolioId, int imageId)
        {
            Image image = _imageDao.GetImageByPortfolioId(portfolioId, imageId);

            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != AdditionalImage)
            {
                return BadRequest("Invalid imageType. Allowed values are 'main image' and 'additional image'.");
            }

            try
            {
                int rowsAffected = _imageDao.DeleteImageByPortfolioId(portfolioId, imageId);

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
                return StatusCode(500, "An error occurred while deleting the portfolio image.");
            }
        }

        /*   
            **********************************************************************************************
                                      WORK EXPERIENCE IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/work-experience/{experienceId}/create-image")]
        public ActionResult CreateImageByWorkExperienceId(int experienceId, Image image)
        {
            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != AdditionalImage && imageType != Logo)
            {
                return BadRequest("Invalid image type. Allowed values are 'main image', 'additional image', and 'logo'.");
            }

            try
            {
                Image createdImage = _imageDao.CreateImageByWorkExperienceId(experienceId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetImageByWorkExperienceId), new { experienceId, imageId = createdImage.Id }, createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the work experience image.");
            }
        }

        [HttpGet("/work-experience/{experienceId}/main-image")]
        public ActionResult GetMainImageByWorkExperienceId(int experienceId)
        {
            Image mainImage = _imageDao.GetMainImageOrCompanyLogoByWorkExperienceId(experienceId, MainImage);

            if (mainImage == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(mainImage);
            }
        }

        [HttpGet("/work-experience/{experienceId}/logo")]
        public ActionResult GetCompanyLogoByWorkExperienceId(int experienceId)
        {
            Image logo = _imageDao.GetMainImageOrCompanyLogoByWorkExperienceId(experienceId, Logo);

            if (logo == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(logo);
            }
        }

        [HttpGet("/work-experience/{experienceId}/image/{imageId}")]
        public ActionResult<Image> GetImageByWorkExperienceId(int experienceId, int imageId)
        {
            Image image = _imageDao.GetImageByWorkExperienceId(experienceId, imageId);

            if (image == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(image);
            }
        }

        [HttpGet("/work-experience/{experienceId}/additional-images")]
        public ActionResult GetAdditionalImagesByWorkExperienceId(int experienceId)
        {
            List<Image> additionalImages = _imageDao.GetAdditionalImagesByWorkExperienceId(experienceId);

            if (additionalImages == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(additionalImages);
            }
        }

        [Authorize]
        [HttpPut("/work-experience/{experienceId}/update-image/{imageId}")]
        public ActionResult UpdateImageByWorkExperienceId(int experienceId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByWorkExperienceId(experienceId, imageId, image);

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
                return StatusCode(500, "An error occurred while updating the work experience image.");
            }
        }

        [Authorize]
        [HttpPut("/work-experience/{experienceId}/update-main-image-or-logo")]
        public ActionResult UpdateMainImageOrCompanyLogoByWorkExperienceId(int experienceId, int imageId, Image image)
        {
            try
            {
                Image updatedMainImage = _imageDao.UpdateMainImageOrLogoByWorkExperienceId(experienceId, imageId, image);

                if (updatedMainImage == null)
                {
                    return BadRequest("The image is null. Please provide a main image or logo.");
                }
                else if (updatedMainImage.Type != MainImage && updatedMainImage.Type != Logo)
                {
                    return BadRequest("The provided image is not a main image or logo. Please provide a main image or logo.");
                }
                else
                {
                    return Ok(updatedMainImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the work experience main image or logo.");
            }
        }

        [Authorize]
        [HttpDelete("/work-experience/{experienceId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByWorkExperienceId(int experienceId, int imageId)
        {
            Image image = _imageDao.GetImageByWorkExperienceId(experienceId, imageId);

            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != AdditionalImage && imageType != Logo)
            {
                return BadRequest("Invalid imageType. Allowed values are 'main image', 'additional image', and 'logo'.");
            }

            try
            {
                int rowsAffected = _imageDao.DeleteImageByWorkExperienceId(experienceId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Work experience image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the work experience image.");
            }
        }

        /*  
            **********************************************************************************************
                                        CREDENTIAL IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/credential/{credentialId}/create-image")]
        public ActionResult CreateImageByCredentialId(int credentialId, Image image)
        {
            string imageType = image.Type.ToLower();

            try
            {
                Image createdImage = _imageDao.CreateImageByCredentialId(credentialId, image);

                if (createdImage == null)
                {
                    return BadRequest("Image cannot be null or empty.");
                }
                else
                {
                    if (imageType == MainImage)
                    {
                        return CreatedAtAction(nameof(GetMainImageByCredentialId), new { credentialId }, createdImage);
                    }
                    else if (imageType == Logo)
                    {
                        return CreatedAtAction(nameof(GetOrganizationLogoByCredentialId), new { credentialId }, createdImage);
                    }
                    else
                    {
                        return BadRequest("Invalid image type. Allowed values are 'main image' and 'logo'.");
                    }
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the credential image.");
            }
        }

        [HttpGet("/credential/{credentialId}/main-image")]
        public ActionResult GetMainImageByCredentialId(int credentialId)
        {
            Image mainImage = _imageDao.GetMainImageOrOrganizationLogoByCredentialId(credentialId, MainImage);

            if (mainImage == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(mainImage);
            }
        }

        [HttpGet("/credential/{credentialId}/logo")]
        public ActionResult GetOrganizationLogoByCredentialId(int credentialId)
        {
            Image logo = _imageDao.GetMainImageOrOrganizationLogoByCredentialId(credentialId, Logo);

            if (logo == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(logo);
            }
        }

        [Authorize]
        [HttpPut("/credential/{credentialId}/update-image/{imageId}")]
        public ActionResult UpdateMainImageOrOrganizationLogoByCredentialId(int credentialId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateMainImageOrOrganizationLogoByCredentialId(credentialId, imageId, image);

                if (updatedImage == null)
                {
                    return BadRequest("The image is null. Please provide a main image or logo.");
                }
                else if (updatedImage.Type != MainImage && updatedImage.Type != Logo)
                {
                    return BadRequest("The provided image is not a main image or logo. Please provide a main image or logo.");
                }
                else
                {
                    return Ok(updatedImage);
                }

            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the credential main image or logo.");
            }
        }

        [Authorize]
        [HttpDelete("/credential/{credentialId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByCredentialId(int credentialId, int imageId)
        {
            Image image = _imageDao.GetImageByCredentialId(credentialId, imageId);

            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != Logo)
            {
                return BadRequest("Invalid imageType. Allowed values are 'main image' and 'logo'.");
            }

            try
            {
                int rowsAffected = _imageDao.DeleteImageByCredentialId(credentialId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Credential image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the credential image.");
            }
        }

        /*  
            **********************************************************************************************
                                        EDUCATION IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/education/{educationId}/create-image")]
        public ActionResult CreateImageByEducationId(int educationId, Image image)
        {
            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != Logo && imageType != AdditionalImage)
            {
                return BadRequest("Invalid image type. Allowed values are 'main image', 'logo', and 'additional image'.");
            }

            try
            {
                Image createdImage = _imageDao.CreateImageByEducationId(educationId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetImageByEducationId), new { educationId, imageId = createdImage.Id }, createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the education image.");
            }
        }

        [HttpGet("/education/{educationId}/main-image")]
        public ActionResult GetMainImageByEducationId(int educationId)
        {
            Image mainImage = _imageDao.GetMainImageOrInstitutionLogoByEducationId(educationId, MainImage);

            if (mainImage == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(mainImage);
            }
        }

        [HttpGet("/education/{educationId}/logo")]
        public ActionResult GetInstitutionLogoByEducationId(int educationId)
        {
            Image logo = _imageDao.GetMainImageOrInstitutionLogoByEducationId(educationId, Logo);

            if (logo == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(logo);
            }
        }

        [HttpGet("/education/{educationId}/image/{imageId}")]
        public ActionResult<Image> GetImageByEducationId(int educationId, int imageId)
        {
            Image image = _imageDao.GetImageByEducationId(educationId, imageId);

            if (image == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(image);
            }
        }

        [HttpGet("/education/{educationId}/additional-images")]
        public ActionResult GetAdditionalImagesByEducationId(int educationId)
        {
            List<Image> additionalImages = _imageDao.GetAdditionalImagesByEducationId(educationId);

            if (additionalImages == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(additionalImages);
            }
        }

        [Authorize]
        [HttpPut("/education/{educationId}/update-image/{imageId}")]
        public ActionResult UpdateImageByEducationId(int educationId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByEducationId(educationId, imageId, image);

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
                return StatusCode(500, "An error occurred while updating the education image.");
            }
        }

        [Authorize]
        [HttpPut("/education/{educationId}/update-main-image-or-logo")]
        public ActionResult UpdateMainImageOrInstitutionLogoByEducationId(int educationId, int imageId, Image image)
        {
            try
            {
                Image updatedMainImage = _imageDao.UpdateMainImageOrLogoByEducationId(educationId, imageId, image);

                if (updatedMainImage == null)
                {
                    return BadRequest("The image is null. Please provide a main image or logo.");
                }
                else if (updatedMainImage.Type != MainImage && updatedMainImage.Type != Logo)
                {
                    return BadRequest("The provided image is not a main image or logo. Please provide a main image or logo.");
                }
                else
                {
                    return Ok(updatedMainImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the education main image or logo.");
            }
        }

        [Authorize]
        [HttpDelete("/education/{educationId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByEducationId(int educationId, int imageId)
        {
            Image image = _imageDao.GetImageByEducationId(educationId, imageId);

            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != Logo && imageType != AdditionalImage)
            {
                return BadRequest("Invalid imageType. Allowed values are 'main image', 'logo', and 'additional image'.");
            }

            try
            {
                int rowsAffected = _imageDao.DeleteImageByEducationId(educationId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Education image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the education image.");
            }
        }

        /*  
            **********************************************************************************************
                                OPEN SOURCE CONTRIBUTION IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/open-source-contribution/{contributionId}/create-image")]
        public ActionResult CreateImageByOpenSourceContributionId(int contributionId, Image image)
        {
            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != AdditionalImage && imageType != Logo)
            {
                return BadRequest("Invalid image type. Allowed values are 'main image', 'additional image', and 'logo'.");
            }

            try
            {
                Image createdImage = _imageDao.CreateImageByOpenSourceContributionId(contributionId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetImageByOpenSourceContributionId), new { contributionId, imageId = createdImage.Id }, createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the open source contribution image.");
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}/main-image")]
        public ActionResult GetMainImageByOpenSourceContributionId(int contributionId)
        {
            Image mainImage = _imageDao.GetMainImageOrOrganizationLogoByOpenSourceContributionId(contributionId, MainImage);

            if (mainImage == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(mainImage);
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}/logo")]
        public ActionResult GetOrganizationLogoByOpenSourceContributionId(int contributionId)
        {
            Image logo = _imageDao.GetMainImageOrOrganizationLogoByOpenSourceContributionId(contributionId, Logo);

            if (logo == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(logo);
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}/image/{imageId}")]
        public ActionResult<Image> GetImageByOpenSourceContributionId(int contributionId, int imageId)
        {
            Image image = _imageDao.GetImageByOpenSourceContributionId(contributionId, imageId);

            if (image == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(image);
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}/additional-images")]
        public ActionResult GetAdditionalImagesByOpenSourceContributionId(int contributionId)
        {
            List<Image> additionalImages = _imageDao.GetAdditionalImagesByOpenSourceContributionId(contributionId);

            if (additionalImages == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(additionalImages);
            }
        }

        [Authorize]
        [HttpPut("/open-source-contribution/{contributionId}/update-image/{imageId}")]
        public ActionResult UpdateImageByOpenSourceContributionId(int contributionId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByOpenSourceContributionId(contributionId, imageId, image);

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
                return StatusCode(500, "An error occurred while updating the open source contribution image.");
            }
        }

        [Authorize]
        [HttpPut("/open-source-contribution/{contributionId}/update-main-image-or-logo")]
        public ActionResult UpdateMainImageOrOrganizationLogoByOpenSourceContributionId(int contributionId, int imageId, Image image)
        {
            try
            {
                Image updatedMainImage = _imageDao.UpdateMainImageOrLogoByOpenSourceContributionId(contributionId, imageId, image);

                if (updatedMainImage == null)
                {
                    return BadRequest("The image is null. Please provide a main image or logo.");
                }
                else if (updatedMainImage.Type != MainImage && updatedMainImage.Type != Logo)
                {
                    return BadRequest("The provided image is not a main image or logo. Please provide a main image or logo.");
                }
                else
                {
                    return Ok(updatedMainImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the open source contribution main image or logo.");
            }
        }

        [Authorize]
        [HttpDelete("/open-source-contribution/{contributionId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByOpenSourceContributionId(int contributionId, int imageId)
        {
            Image image = _imageDao.GetImageByOpenSourceContributionId(contributionId, imageId);

            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != AdditionalImage && imageType != Logo)
            {
                return BadRequest("Invalid imageType. Allowed values are 'main image', 'additional image', and 'logo'.");
            }

            try
            {
                int rowsAffected = _imageDao.DeleteImageByOpenSourceContributionId(contributionId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Open source contribution image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the open source contribution image.");
            }
        }

        /*  
            **********************************************************************************************
                                        VOLUNTEER WORK IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/volunteer-work/{volunteerWorkId}/create-image")]
        public ActionResult CreateImageByVolunteerWorkId(int volunteerWorkId, Image image)
        {
            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != AdditionalImage && imageType != Logo)
            {
                return BadRequest("Invalid image type. Allowed values are 'main image', 'additional image', and 'logo'.");
            }

            try
            {
                Image createdImage = _imageDao.CreateImageByVolunteerWorkId(volunteerWorkId, image);

                if (createdImage == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetImageByVolunteerWorkId), new { volunteerWorkId, imageId = createdImage.Id }, createdImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the volunteer work image.");
            }
        }

        [HttpGet("/volunteer-work/{volunteerWorkId}/main-image")]
        public ActionResult GetMainImageByVolunteerWorkId(int volunteerWorkId)
        {
            Image mainImage = _imageDao.GetMainImageOrOrganizationLogoByVolunteerWorkId(volunteerWorkId, MainImage);

            if (mainImage == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(mainImage);
            }
        }

        [HttpGet("/volunteer-work/{volunteerWorkId}/logo")]
        public ActionResult GetOrganizationLogoByVolunteerWorkId(int volunteerWorkId)
        {
            Image logo = _imageDao.GetMainImageOrOrganizationLogoByVolunteerWorkId(volunteerWorkId, Logo);

            if (logo == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(logo);
            }
        }

        [HttpGet("/volunteer-work/{volunteerWorkId}/image/{imageId}")]
        public ActionResult<Image> GetImageByVolunteerWorkId(int volunteerWorkId, int imageId)
        {
            Image image = _imageDao.GetImageByVolunteerWorkId(volunteerWorkId, imageId);

            if (image == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(image);
            }
        }

        [HttpGet("/volunteer-work/{volunteerWorkId}/additional-images")]
        public ActionResult GetAdditionalImagesByVolunteerWorkId(int volunteerWorkId)
        {
            List<Image> additionalImages = _imageDao.GetAdditionalImagesByVolunteerWorkId(volunteerWorkId);

            if (additionalImages == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(additionalImages);
            }
        }

        [Authorize]
        [HttpPut("/volunteer-work/{volunteerWorkId}/update-image/{imageId}")]
        public ActionResult UpdateImageByVolunteerWorkId(int volunteerWorkId, int imageId, Image image)
        {
            try
            {
                Image updatedImage = _imageDao.UpdateImageByVolunteerWorkId(volunteerWorkId, imageId, image);

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
                return StatusCode(500, "An error occurred while updating the volunteer work image.");
            }
        }

        [Authorize]
        [HttpPut("/volunteer-work/{volunteerWorkId}/update-main-image-or-logo")]
        public ActionResult UpdateMainImageOrOrganizationLogoByVolunteerWorkId(int volunteerWorkId, int imageId, Image image)
        {
            try
            {
                Image updatedMainImage = _imageDao.UpdateMainImageOrLogoByVolunteerWorkId(volunteerWorkId, imageId, image);

                if (updatedMainImage == null)
                {
                    return BadRequest("The image is null. Please provide a main image or logo.");
                }
                else if (updatedMainImage.Type != MainImage && updatedMainImage.Type != Logo)
                {
                    return BadRequest("The provided image is not a main image or logo. Please provide a main image or logo.");
                }
                else
                {
                    return Ok(updatedMainImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the volunteer work main image or logo.");
            }
        }

        [Authorize]
        [HttpDelete("/volunteer-work/{volunteerWorkId}/delete-image/{imageId}")]
        public ActionResult DeleteImageByVolunteerWorkId(int volunteerWorkId, int imageId)
        {
            Image image = _imageDao.GetImageByVolunteerWorkId(volunteerWorkId, imageId);

            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != AdditionalImage && imageType != Logo)
            {
                return BadRequest("Invalid imageType. Allowed values are 'main image', 'additional image', and 'logo'.");
            }

            try
            {
                int rowsAffected = _imageDao.DeleteImageByVolunteerWorkId(volunteerWorkId, imageId);

                if (rowsAffected > 0)
                {
                    return Ok("Volunteer work image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the volunteer work image.");
            }
        }

        /*  
            **********************************************************************************************
                                        ACHIEVEMENT IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */
// TODO IMAGE Achievement Controllers****
        /*  
            **********************************************************************************************
                                            HOBBY IMAGE CRUD CONTROLLER
            **********************************************************************************************
        */
// TODO IMAGE Hobby Controllers****        
        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                        SIDE PROJECT IMAGE CRUD CONTROLLER
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/sideproject/{sideProjectId}/create-image")]
        public ActionResult CreateImageBySideProjectId(int sideProjectId, Image image)
        {

            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != AdditionalImage)
            {
                return BadRequest("Invalid image type. Allowed values are 'main image' and 'additional image'.");
            }

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

        [HttpGet("/sideproject/{sideProjectId}/main-image")]
        public ActionResult GetMainImageBySideProjectId(int sideProjectId)
        {
            Image mainImage = _imageDao.GetMainImageBySideProjectId(sideProjectId);

            if (mainImage == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(mainImage);
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/all-images")]
        public ActionResult GetAllImagesBySideProjectId(int sideProjectId)
        {
            List<Image> images = _imageDao.GetAllImagesBySideProjectId(sideProjectId);

            if (images == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(images);
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/additional-images")]
        public ActionResult GetAdditionalImagesBySideProjectId(int sideProjectId)
        {
            List<Image> additionalImages = _imageDao.GetAdditionalImagesBySideProjectId(sideProjectId);

            if (additionalImages == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(additionalImages);
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

        [Authorize]
        [HttpPut("/update-sideproject/{sideProjectId}/update-main-image/{mainImageId}")]
        public ActionResult UpdateMainImageBySideProjectId(int sideProjectId, int mainImageId, Image mainImage)
        {
            try
            {
                Image updatedMainImage = _imageDao.UpdateMainImageBySideProjectId(sideProjectId, mainImageId, mainImage);
                
                if (updatedMainImage == null)
                {
                    return BadRequest("The image is null. Please provide a main image.");
                }
                else if (updatedMainImage.Type != MainImage)
                {
                    return BadRequest("The provided image is not a main image. Please provide a main image.");
                }
                else
                {
                    return Ok(updatedMainImage);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the side project main image.");
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/{sideProjectId}/delete-image/{imageId}")]
        public ActionResult DeleteImageBySideProjectId(int sideProjectId, int imageId)
        {
            Image image = _imageDao.GetImageBySideProjectId(sideProjectId, imageId);

            string imageType = image.Type.ToLower();

            if (imageType != MainImage && imageType != AdditionalImage)
            {
                return BadRequest("Invalid imageType. Allowed values are 'main image' and 'additional image'.");
            }

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
                return StatusCode(500, "An error occurred while deleting the blog post image.");
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

        [HttpGet("/website/{websiteId}/image/{imageId}")]
        public ActionResult<Image> GetImageByWebsiteId(int websiteId, int imageId)
        {
            Image logo = _imageDao.GetImageByWebsiteId(websiteId, imageId);

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
