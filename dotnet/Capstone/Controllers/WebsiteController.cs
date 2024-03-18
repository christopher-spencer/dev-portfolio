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

        /*  
            **********************************************************************************************
                                                WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */
// TODO TEST ALL POSTMAN CRUD

        [Authorize]
        [HttpPost("/create-website")]
        public ActionResult CreateWebsite(Website website)
        {
            try
            {
                Website createdWebsite = _websiteDao.CreateWebsite(website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsite), new { websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the website.");
            }
        }

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

        [Authorize]
        [HttpPut("/update-website/{websiteId}")]
        public ActionResult UpdateWebsite(Website website, int websiteId)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsite(website, websiteId);

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
                return StatusCode(500, "An error occurred while updating the website.");
            }
        }

        [Authorize]
        [HttpDelete("/delete-website/{websiteId}")]
        public ActionResult DeleteWebsite(int websiteId)
        {
            try
            {
                int rowsAffected = _websiteDao.DeleteWebsite(websiteId);

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
                                            SIDE PROJECT WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/sideproject/{sideProjectId}/create-website/{websiteType}")]
        public ActionResult CreateWebsiteBySideProjectId(int sideProjectId, Website website, string websiteType)
        {
            websiteType = websiteType.ToLower();
      //TODO change to "main website" or something
            if (websiteType != "website" && websiteType != "github")
            {
                return BadRequest("Invalid websiteType. Allowed values are 'website' and 'github'.");
            }

            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteBySideProjectId(sideProjectId, website, websiteType);

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
        [HttpDelete("/sideproject/{sideProjectId}/delete-website/{websiteType}/{websiteId}")]
        public ActionResult DeleteWebsiteBySideProjectId(int sideProjectId, int websiteId, string websiteType)
        {
            websiteType = websiteType.ToLower();
//TODO change to "main website" or something
            if (websiteType != "website" && websiteType != "github")
            {
                return BadRequest("Invalid websiteType. Allowed values are 'website' and 'github'.");
            }

            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteBySideProjectId(sideProjectId, websiteId, websiteType);

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
        [HttpPost("/contributor/{contributorId}/create-website/{websiteType}")]
        public ActionResult CreateWebsiteByContributorId(int contributorId, Website website, string websiteType)
        {
            websiteType = websiteType.ToLower();

            if (websiteType != "linkedin" && websiteType != "github" && websiteType != "portfoliolink")
            {
                return BadRequest("Invalid websiteType. Allowed values are 'linkedin' and 'github' and 'portfoliolink'.");
            }

            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByContributorId(contributorId, website, websiteType);

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
        [HttpDelete("/contributor/{contributorId}/delete-website/{websiteType}/{websiteId}")]
        public ActionResult DeleteWebsiteByContributorId(int contributorId, int websiteId, string websiteType)
        {
            websiteType = websiteType.ToLower();

            if (websiteType != "linkedin" && websiteType != "github" && websiteType != "portfoliolink")
            {
                return BadRequest("Invalid websiteType. Allowed values are 'linkedin' and 'github' and 'portfoliolink'.");
            }

            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteByContributorId(contributorId, websiteId, websiteType);

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
//FIXME in Postman: "An error occurred while deleting the API/service website."
// COMMENT IT WORKED WHEN DELETING WHERE THERE WAS NO IMAGE/ASSOCIATED IMAGE ID, SO ISSUE W/ OUR DELETES PROB DEALS WITH THOSE ASSOCIATED OBJECTS
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
//FIXME in Postman: "An error occurred while deleting the dependency/library website."
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
