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

        [Authorize]
        [HttpPost("/create-website")]
        public ActionResult CreateWebsite(Website website)
        {
            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteLink(website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteById), new { websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while creating the website.");
            }
        }

        [HttpGet("/website/{websiteId}")]
        public ActionResult<Website> GetWebsiteById(int websiteId)
        {
            Website website = _websiteDao.GetWebsiteById(websiteId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        [HttpGet("/get-all-websites")]
        public ActionResult<List<Website>> GetAllWebsites()
        {
            List<Website> websites = _websiteDao.GetAllWebsites();

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
        [HttpPut("/update-website/{websiteId}/")]
        public ActionResult UpdateWebsite(Website website, int websiteId)
        {
            try 
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteByWebsiteId(websiteId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while updating the website.");
            }
        }

        [Authorize]
        [HttpDelete("/website/delete/{websiteId}")]
        public ActionResult DeleteWebsiteById(int websiteId)
        {
            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteById(websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("Website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException ex)
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
        [HttpPost("/sideproject/{projectId}/create-website")]
        public ActionResult CreateWebsiteByProjectId(int projectId, Website website)
        {
            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByProjectId(projectId, website);
            
                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteByProjectId), new { projectId = projectId, websiteId = createdWebsite.Id}, createdWebsite);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while creating the side project website.");
            }
        }

        [HttpGet("/sideproject/{projectId}/website")]
        public ActionResult<Website> GetWebsiteByProjectId(int projectId)
        {
            Website website = _websiteDao.GetWebsiteByProjectId(projectId);

            if (website == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(website);
            }
        }

        [HttpGet("/sideproject/{projectId}/website/{websiteId}")]
        public ActionResult<Website> GetWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteByProjectIdAndWebsiteId(projectId, websiteId);

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
        [HttpPut("/update-sideproject/{projectId}/update-website")]
        public ActionResult UpdateWebsiteByProjectId(int projectId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteByProjectId(projectId, website);
                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while updating the side project website.");
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/{projectId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId)
        {
            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteByProjectIdAndWebsiteId(projectId, websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("Website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException ex)
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
            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByContributorId(contributorId, website);
            
                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteByContributorId), new { contributorId = contributorId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while creating the contributor website.");
            }
        }

        [HttpGet("/contributor/{contributorId}/websites")]
        public ActionResult GetWebsitesByContributorId(int contributorId)
        {
            List<Website> websites = _websiteDao.GetWebsiteByContributorId(contributorId);

            if (websites == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(websites);               
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
                Website updatedWebsite = _websiteDao.UpdateWebsiteByContributorId(contributorId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while updating the contributor website.");
            }
        }

        [Authorize]
        [HttpDelete("/contributor/{contributorId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteByContributorId(int contributorId, int websiteId)
        {
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
            catch (DaoException ex)
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
                    return CreatedAtAction(nameof(GetWebsiteByApiServiceId), new { apiServiceId = apiServiceId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while creating the API/service website.");
            }
        }

        [HttpGet("/api-service/{apiServiceId}/websites")]
        public ActionResult GetWebsitesByApiServiceId(int apiServiceId)
        {
            List<Website> websites = _websiteDao.GetWebsiteByApiServiceId(apiServiceId);

            if (websites == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(websites);               
            }
        }

        [HttpGet("/api-service/{apiServiceId}/website/{websiteId}")]
        public ActionResult<Website> GetWebsiteByApiServiceId(int apiServiceId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteByApiServiceId(apiServiceId, websiteId);
            
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
                Website updatedWebsite = _websiteDao.UpdateWebsiteByApiServiceId(apiServiceId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException ex)
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
            catch (DaoException ex)
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
                    return CreatedAtAction(nameof(GetWebsiteByDependencyLibraryId), new { dependencyLibraryId = dependencyLibraryId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while creating the dependency/library website.");
            }
        }

        [HttpGet("/dependency-library/{dependencyLibraryId}/websites")]
        public ActionResult GetWebsitesByDependencyLibraryId(int dependencyLibraryId)
        {
            List<Website> websites = _websiteDao.GetWebsiteByDependencyLibraryId(dependencyLibraryId);

            if (websites == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(websites);               
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
                Website updatedWebsite = _websiteDao.UpdateWebsiteByDependencyLibraryId(dependencyLibraryId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException ex)
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
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while deleting the dependency/library website.");
            }
        }    }
}