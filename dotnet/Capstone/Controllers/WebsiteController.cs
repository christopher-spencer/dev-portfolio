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
    public class WebsiteController : ControllerBase
    {
        private readonly IWebsiteDao _websiteDao;

        public WebsiteController(IWebsiteDao websiteDao)
        {
            _websiteDao = websiteDao;
        }

        const string MainWebsite = "main website";
        const string SecondaryWebsite = "secondary website";
        const string GitHub = "github";
        const string PortfolioLink = "portfolio link";
        const string LinkedIn = "linkedin";
        const string PullRequestLink = "pull request link";


        /*  
            **********************************************************************************************
                                                WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */

        // [Authorize]
        // [HttpPost("/create-website")]
        // public ActionResult CreateWebsite(Website website)
        // {
        //     try
        //     {
        //         Website createdWebsite = _websiteDao.CreateWebsite(website);

        //         if (createdWebsite == null)
        //         {
        //             return BadRequest();
        //         }
        //         else
        //         {
        //             return CreatedAtAction(nameof(GetWebsite), new { websiteId = createdWebsite.Id }, createdWebsite);
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while creating the website.");
        //     }
        // }

        [HttpGet("/website/{websiteId}")]
        public ActionResult<Website> GetWebsite(int websiteId)
        {
            Website website = _websiteDao.GetWebsite(websiteId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        [HttpGet("/websites")]
        public ActionResult<List<Website>> GetWebsites()
        {
            List<Website> websites = _websiteDao.GetWebsites();

            if (websites == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(websites);
            }
        }

        // [Authorize]
        // [HttpPut("/update-website/{websiteId}")]
        // public ActionResult UpdateWebsite(Website website, int websiteId)
        // {
        //     try
        //     {
        //         Website updatedWebsite = _websiteDao.UpdateWebsite(website, websiteId);

        //         if (updatedWebsite == null)
        //         {
        //             return BadRequest();
        //         }
        //         else
        //         {
        //             return Ok(updatedWebsite);
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while updating the website.");
        //     }
        // }

        // [Authorize]
        // [HttpDelete("/delete-website/{websiteId}")]
        // public ActionResult DeleteWebsite(int websiteId)
        // {
        //     try
        //     {
        //         int rowsAffected = _websiteDao.DeleteWebsite(websiteId);

        //         if (rowsAffected > 0)
        //         {
        //             return Ok("Website deleted successfully.");
        //         }
        //         else
        //         {
        //             return NotFound();
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while deleting the website.");
        //     }
        // }

        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                        PORTFOLIO WEBSITE CRUD CONTROLLER
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/portfolio/{portfolioId}/create-website")]
        public ActionResult CreateWebsiteByPortfolioId(int portfolioId, Website website)
        {
            string websiteType = website.Type.ToLower();

            if (websiteType != GitHub && websiteType != LinkedIn)
            {
                return BadRequest("Invalid websiteType. Allowed values are 'github' and 'linkedin'.");
            }

            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByPortfolioId(portfolioId, website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteByPortfolioId), new { portfolioId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the portfolio website.");
            }
        }

        [HttpGet("/portfolio/{portfolioId}/website/{websiteId}")]
        public ActionResult<Website> GetWebsiteByPortfolioId(int portfolioId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteByPortfolioId(portfolioId, websiteId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        [Authorize]
        [HttpPut("/portfolio/{portfolioId}/update-website/{websiteId}")]
        public ActionResult UpdateWebsiteByPortfolioId(int portfolioId, int websiteId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteByPortfolioId(portfolioId, websiteId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the portfolio website.");
            }
        }

        [Authorize]
        [HttpDelete("/portfolio/{portfolioId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteByPortfolioId(int portfolioId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteByPortfolioId(portfolioId, websiteId);

            string websiteType = website.Type.ToLower();

            if (websiteType != GitHub && websiteType != LinkedIn)
            {
                return BadRequest("Invalid websiteType. Allowed values are 'github' and 'linkedin'.");
            }

            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteByPortfolioId(portfolioId, websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("Portfolio website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the portfolio website.");
            }
        }

        /*   
            **********************************************************************************************
                                        WORK EXPERIENCE WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/work-experience/{experienceId}/create-website")]
        public ActionResult CreateWebsiteByWorkExperienceId(int experienceId, Website website)
        {
            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByWorkExperienceId(experienceId, website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteByWorkExperienceId), new { experienceId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the work experience website.");
            }
        }

        [HttpGet("/work-experience/{experienceId}/website")]
        public ActionResult<Website> GetWebsiteByWorkExperienceId(int experienceId)
        {
            Website website = _websiteDao.GetWebsiteByWorkExperienceId(experienceId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        [Authorize]
        [HttpPut("/work-experience/{experienceId}/update-website/{websiteId}")]
        public ActionResult UpdateWebsiteByWorkExperienceId(int experienceId, int websiteId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteByWorkExperienceId(experienceId, websiteId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the work experience website.");
            }
        }

        [Authorize]
        [HttpDelete("/work-experience/{experienceId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteByWorkExperienceId(int experienceId, int websiteId)
        {
            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteByWorkExperienceId(experienceId, websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("Work experience website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the work experience website.");
            }
        }

        /*  
            **********************************************************************************************
                                        CREDENTIAL WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/credential/{credentialId}/create-website")]
        public ActionResult CreateWebsiteByCredentialId(int credentialId, Website website)
        {
            string websiteType = website.Type.ToLower();

            if (websiteType != MainWebsite && websiteType != SecondaryWebsite)
            {
                return BadRequest("Invalid websiteType. Allowed values are 'main website' and 'secondary website'.");
            }

            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByCredentialId(credentialId, website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteByCredentialId), new { credentialId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the credential website.");
            }
        }

        [HttpGet("/credential/{credentialId}/website/{websiteId}")]
        public ActionResult<Website> GetWebsiteByCredentialId(int credentialId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteByCredentialId(credentialId, websiteId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        [Authorize]
        [HttpPut("/credential/{credentialId}/update-website/{websiteId}")]
        public ActionResult UpdateWebsiteByCredentialId(int credentialId, int websiteId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteByCredentialId(credentialId, websiteId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the credential website.");
            }
        }

        [Authorize]
        [HttpDelete("/credential/{credentialId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteByCredentialId(int credentialId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteByCredentialId(credentialId, websiteId);

            string websiteType = website.Type.ToLower();

            if (websiteType != MainWebsite && websiteType != SecondaryWebsite)
            {
                return BadRequest("Invalid websiteType. Allowed values are 'main website' and 'secondary website'.");
            }

            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteByCredentialId(credentialId, websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("Credential website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the credential website.");
            }
        }

        /*  
            **********************************************************************************************
                                        EDUCATION WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/education/{educationId}/create-website")]
        public ActionResult CreateWebsiteByEducationId(int educationId, Website website)
        {
            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByEducationId(educationId, website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteByEducationId), new { educationId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the education website.");
            }
        }

        [HttpGet("/education/{educationId}/website")]
        public ActionResult<Website> GetWebsiteByEducationId(int educationId)
        {
            Website website = _websiteDao.GetWebsiteByEducationId(educationId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        [Authorize]
        [HttpPut("/education/{educationId}/update-website/{websiteId}")]
        public ActionResult UpdateWebsiteByEducationId(int educationId, int websiteId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteByEducationId(educationId, websiteId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the education website.");
            }
        }

        [Authorize]
        [HttpDelete("/education/{educationId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteByEducationId(int educationId, int websiteId)
        {
            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteByEducationId(educationId, websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("Education website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the education website.");
            }
        }

        /*  
            **********************************************************************************************
                                OPEN SOURCE CONTRIBUTION WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */
// TODO WEBSITE OpenSourceContribution Controllers****
        [Authorize]
        [HttpPost("/open-source-contribution/{contributionId}/create-website")]
        public ActionResult CreateWebsiteByOpenSourceContributionId(int contributionId, Website website)
        {
            string websiteType = website.Type.ToLower();

            if (websiteType != MainWebsite && websiteType != GitHub && websiteType != PullRequestLink)
            {
                return BadRequest("Invalid websiteType. Allowed values are 'main website' and 'github' and 'pull request link'.");
            }

            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByOpenSourceContributionId(contributionId, website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteByOpenSourceContributionId), new { contributionId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the open source contribution website.");
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}/website/{websiteId}")]
        public ActionResult<Website> GetWebsiteByOpenSourceContributionId(int contributionId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteByOpenSourceContributionId(contributionId, websiteId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        /*  
            **********************************************************************************************
                                        VOLUNTEER WORK WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */
// TODO WEBSITE VolunteerWork Controllers****
        /*  
            **********************************************************************************************
                                        ACHIEVEMENT WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */
// TODO WEBSITE Achievement Controllers****
        /*  
            **********************************************************************************************
                                            HOBBY WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */
// TODO WEBSITE Hobby Controllers****       

        /*  
            **********************************************************************************************
                                            SIDE PROJECT WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/sideproject/{sideProjectId}/create-website")]
        public ActionResult CreateWebsiteBySideProjectId(int sideProjectId, Website website)
        {
            string websiteType = website.Type.ToLower();

      //TODO change to "main website" or something
            if (websiteType != MainWebsite && websiteType != GitHub)
            {
                return BadRequest("Invalid websiteType. Allowed values are 'main-website' and 'github'.");
            }

            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteBySideProjectId(sideProjectId, website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteBySideProjectId), new { sideProjectId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the side project website.");
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/website/{websiteId}")]
        public ActionResult<Website> GetWebsiteBySideProjectId(int sideProjectId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteBySideProjectId(sideProjectId, websiteId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        [Authorize]
        [HttpPut("/update-sideproject/{sideProjectId}/update-website/{websiteId}")]
        public ActionResult UpdateWebsiteBySideProjectId(int sideProjectId, int websiteId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteBySideProjectId(sideProjectId, websiteId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the side project website.");
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/{sideProjectId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteBySideProjectId(int sideProjectId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteBySideProjectId(sideProjectId, websiteId);
            
            string websiteType = website.Type.ToLower();

            if (websiteType != MainWebsite && websiteType != GitHub)
            {
                return BadRequest("Invalid websiteType. Allowed values are 'main-website' and 'github'.");
            }

            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteBySideProjectId(sideProjectId, websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("Website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the website.");
            }
        }

        /*  
            **********************************************************************************************
                                            CONTRIBUTOR WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */
        
        [Authorize]
        [HttpPost("/contributor/{contributorId}/create-website")]
        public ActionResult CreateWebsiteByContributorId(int contributorId, Website website)
        {
            string websiteType = website.Type.ToLower();

            if (websiteType != LinkedIn && websiteType != GitHub && websiteType != PortfolioLink)
            {
                return BadRequest("Invalid websiteType. Allowed values are 'linkedin' and 'github' and 'portfolio-link'.");
            }

            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByContributorId(contributorId, website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteByContributorId), new { contributorId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the contributor website.");
            }
        }

        [HttpGet("/contributor/{contributorId}/website/{websiteId}")]
        public ActionResult<Website> GetWebsiteByContributorId(int contributorId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteByContributorId(contributorId, websiteId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        [Authorize]
        [HttpPut("/contributor/{contributorId}/update-website/{websiteId}")]
        public ActionResult UpdateWebsiteByContributorId(int contributorId, int websiteId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteByContributorId(contributorId, websiteId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the contributor website.");
            }
        }

        [Authorize]
        [HttpDelete("/contributor/{contributorId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteByContributorId(int contributorId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteByContributorId(contributorId, websiteId);

            string websiteType = website.Type.ToLower();

            if (websiteType != LinkedIn && websiteType != GitHub && websiteType != PortfolioLink)
            {
                return BadRequest("Invalid websiteType. Allowed values are 'linkedin' and 'github' and 'portfolio-link'.");
            }

            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteByContributorId(contributorId, websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("Contributor website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the contributor website.");
            }
        }
        /*  
            **********************************************************************************************
                                         API AND SERVICE WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/api-service/{apiServiceId}/create-website")]
        public ActionResult CreateWebsiteByApiServiceId(int apiServiceId, Website website)
        {
            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByApiServiceId(apiServiceId, website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteByApiServiceId), new { apiServiceId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the API/service website.");
            }
        }

        [HttpGet("/api-service/{apiServiceId}/website/{websiteId}")]
        public ActionResult<Website> GetWebsiteByApiServiceId(int apiServiceId)
        {
            Website website = _websiteDao.GetWebsiteByApiServiceId(apiServiceId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        [Authorize]
        [HttpPut("/api-service/{apiServiceId}/update-website/{websiteId}")]
        public ActionResult UpdateWebsiteByApiServiceId(int apiServiceId, int websiteId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteByApiServiceId(apiServiceId, websiteId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the API/service website.");
            }
        }

        [Authorize]
        [HttpDelete("/api-service/{apiServiceId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteByApiServiceId(int apiServiceId, int websiteId)
        {
            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteByApiServiceId(apiServiceId, websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("API/service website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the API/service website.");
            }
        }
        /*  
            **********************************************************************************************
                                      DEPENDENCY AND LIBRARY WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/dependency-library/{dependencyLibraryId}/create-website")]
        public ActionResult CreateWebsiteByDependencyLibraryId(int dependencyLibraryId, Website website)
        {
            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByDependencyLibraryId(dependencyLibraryId, website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteByDependencyLibraryId), new { dependencyLibraryId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the dependency/library website.");
            }
        }

        [HttpGet("/dependency-library/{dependencyLibraryId}/website/{websiteId}")]
        public ActionResult<Website> GetWebsiteByDependencyLibraryId(int dependencyLibraryId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteByDependencyLibraryId(dependencyLibraryId, websiteId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        [Authorize]
        [HttpPut("/dependency-library/{dependencyLibraryId}/update-website/{websiteId}")]
        public ActionResult UpdateWebsiteByDependencyLibraryId(int dependencyLibraryId, int websiteId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteByDependencyLibraryId(dependencyLibraryId, websiteId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the dependency/library website.");
            }
        }

        [Authorize]
        [HttpDelete("/dependency-library/{dependencyLibraryId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteByDependencyLibraryId(int dependencyLibraryId, int websiteId)
        {
            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteByDependencyLibraryId(dependencyLibraryId, websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("Dependency/library website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the dependency/library website.");
            }
        }
    }
}
