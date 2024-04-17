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

// TODO add controller to postman and integration test
        [HttpGet("/portfolio/{portfolioId}/github")]
        public ActionResult<Website> GetGitHubByPortfolioId(int portfolioId)
        {
            Website gitHub = _websiteDao.GetGitHubByPortfolioId(portfolioId);

            if (gitHub == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(gitHub);
            }
        }

// TODO add controller to postman and integration test
        [HttpGet("/portfolio/{portfolioId}/linkedin")]
        public ActionResult<Website> GetLinkedInByPortfolioId(int portfolioId)
        {
            Website linkedIn = _websiteDao.GetLinkedInByPortfolioId(portfolioId);

            if (linkedIn == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(linkedIn);
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

// TODO add controller to postman and integration test
        [HttpGet("/credential/{credentialId}/organization-website")]
        public ActionResult<Website> GetOrganizationWebsiteByCredentialId(int credentialId)
        {
            Website organizationWebsite = _websiteDao.GetOrganizationWebsiteByCredentialId(credentialId);

            if (organizationWebsite == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(organizationWebsite);
            }
        }

// TODO add controller to postman and integration test
        [HttpGet("/credential/{credentialId}/credential-website")]
        public ActionResult<Website> GetCredentialWebsiteByCredentialId(int credentialId)
        {
            Website credentialWebsite = _websiteDao.GetCredentialWebsiteByCredentialId(credentialId);

            if (credentialWebsite == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(credentialWebsite);
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
                return BadRequest();
            }
            else
            {
                return Ok(website);
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}/main-website")]
        public ActionResult<Website> GetMainWebsiteByOpenSourceContributionId(int contributionId)
        {
            Website mainWebsite = _websiteDao.GetMainWebsiteOrGitHubByOpenSourceContributionId(contributionId, MainWebsite);

            if (mainWebsite == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(mainWebsite);
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}/github")]
        public ActionResult<Website> GetGitHubByOpenSourceContributionId(int contributionId)
        {
            Website gitHub = _websiteDao.GetMainWebsiteOrGitHubByOpenSourceContributionId(contributionId, GitHub);

            if (gitHub == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(gitHub);
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}/websites")]
        public ActionResult<List<Website>> GetAllWebsitesByOpenSourceContributionId(int contributionId)
        {
            List<Website> allWebsites = _websiteDao.GetAllWebsitesByOpenSourceContributionId(contributionId);

            if (allWebsites == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(allWebsites);
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}/pull-request-links")]
        public ActionResult<List<Website>> GetPullRequestLinksByOpenSourceContributionId(int contributionId)
        {
            List<Website> pullRequestLinks = _websiteDao.GetPullRequestLinksByOpenSourceContributionId(contributionId);

            if (pullRequestLinks == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(pullRequestLinks);
            }
        }

        [Authorize]
        [HttpPut("/open-source-contribution/{contributionId}/update-website/{websiteId}")]
        public ActionResult UpdateWebsiteByOpenSourceContributionId(int contributionId, int websiteId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteByOpenSourceContributionId(contributionId, websiteId, website);

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
                return StatusCode(500, "An error occurred while updating the open source contribution website.");
            }
        }

        [Authorize]
        [HttpPut("/open-source-contribution/{contributionId}/update-main-website-or-github/{websiteId}")]
        public ActionResult UpdateMainWebsiteByOpenSourceContributionId(int contributionId, int websiteId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateMainWebsiteOrGitHubByOpenSourceContributionId(contributionId, websiteId, website);

                if (updatedWebsite == null)
                {
                    return BadRequest("The website is null. Please provide a website.");
                }
                else if (updatedWebsite.Type.ToLower() != MainWebsite && updatedWebsite.Type.ToLower() != GitHub)
                {
                    return BadRequest("Invalid websiteType. Allowed values are 'main website' and 'github'.");
                }
                else
                {
                    return Ok(updatedWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the open source contribution main website.");
            }
        }

        [Authorize]
        [HttpDelete("/open-source-contribution/{contributionId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteByOpenSourceContributionId(int contributionId, int websiteId)
        {
            Website website = _websiteDao.GetWebsiteByOpenSourceContributionId(contributionId, websiteId);

            string websiteType = website.Type.ToLower();

            if (websiteType != MainWebsite && websiteType != GitHub && websiteType != PullRequestLink)
            {
                return BadRequest("Invalid websiteType. Allowed values are 'main website' and 'github' and 'pull request link'.");
            }

            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteByOpenSourceContributionId(contributionId, websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("Open source contribution website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the open source contribution website.");
            }
        }

        /*  
            **********************************************************************************************
                                        VOLUNTEER WORK WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/volunteer-work/{volunteerWorkId}/create-website")]
        public ActionResult CreateWebsiteByVolunteerWorkId(int volunteerWorkId, Website website)
        {
            try
            {
                Website createdWebsite = _websiteDao.CreateWebsiteByVolunteerWorkId(volunteerWorkId, website);

                if (createdWebsite == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWebsiteByVolunteerWorkId), new { volunteerWorkId, websiteId = createdWebsite.Id }, createdWebsite);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the volunteer work website.");
            }
        }

        [HttpGet("/volunteer-work/{volunteerWorkId}/website")]
        public ActionResult<Website> GetWebsiteByVolunteerWorkId(int volunteerWorkId)
        {
            Website website = _websiteDao.GetWebsiteByVolunteerWorkId(volunteerWorkId);

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
        [HttpPut("/volunteer-work/{volunteerWorkId}/update-website/{websiteId}")]
        public ActionResult UpdateWebsiteByVolunteerWorkId(int volunteerWorkId, int websiteId, Website website)
        {
            try
            {
                Website updatedWebsite = _websiteDao.UpdateWebsiteByVolunteerWorkId(volunteerWorkId, websiteId, website);

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
                return StatusCode(500, "An error occurred while updating the volunteer work website.");
            }
        }

        [Authorize]
        [HttpDelete("/volunteer-work/{volunteerWorkId}/delete-website/{websiteId}")]
        public ActionResult DeleteWebsiteByVolunteerWorkId(int volunteerWorkId, int websiteId)
        {
            try
            {
                int rowsAffected = _websiteDao.DeleteWebsiteByVolunteerWorkId(volunteerWorkId, websiteId);

                if (rowsAffected > 0)
                {
                    return Ok("Volunteer work website deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the volunteer work website.");
            }
        }

        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                            SIDE PROJECT WEBSITE CRUD CONTROLLER
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/sideproject/{sideProjectId}/create-website")]
        public ActionResult CreateWebsiteBySideProjectId(int sideProjectId, Website website)
        {
            string websiteType = website.Type.ToLower();

            if (websiteType != MainWebsite && websiteType != GitHub)
            {
                return BadRequest("Invalid websiteType. Allowed values are 'main website' and 'github'.");
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

// TODO add controller to postman and integration test
        [HttpGet("/sideproject/{sideProjectId}/main-website")]
        public ActionResult<Website> GetMainWebsiteBySideProjectId(int sideProjectId)
        {
            Website mainWebsite = _websiteDao.GetMainWebsiteBySideProjectId(sideProjectId);

            if (mainWebsite == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(mainWebsite);
            }
        }

// TODO add controller to postman and integration test
        [HttpGet("/sideproject/{sideProjectId}/github")]
        public ActionResult<Website> GetGitHubBySideProjectId(int sideProjectId)
        {
            Website gitHub = _websiteDao.GetGitHubBySideProjectId(sideProjectId);

            if (gitHub == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(gitHub);
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
                return BadRequest("Invalid websiteType. Allowed values are 'main website' and 'github'.");
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
                return BadRequest("Invalid websiteType. Allowed values are 'linkedin' and 'github' and 'portfolio link'.");
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

// TODO add controller to postman and integration test
        [HttpGet("/contributor/{contributorId}/portfolio-link")]
        public ActionResult<Website> GetPortfolioLinkByContributorId(int contributorId)
        {
            Website portfolioLink = _websiteDao.GetPortfolioLinkByContributorId(contributorId);

            if (portfolioLink == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(portfolioLink);
            }
        }    

// TODO add controller to postman and integration test
        [HttpGet("/contributor/{contributorId}/github")]
        public ActionResult<Website> GetGitHubByContributorId(int contributorId)
        {
            Website gitHub = _websiteDao.GetGitHubByContributorId(contributorId);

            if (gitHub == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(gitHub);
            }
        }  

// TODO add controller to postman and integration test
        [HttpGet("/contributor/{contributorId}/linkedin")]
        public ActionResult<Website> GetLinkedInByContributorId(int contributorId)
        {
            Website linkedIn = _websiteDao.GetLinkedInByContributorId(contributorId);

            if (linkedIn == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(linkedIn);
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
                return BadRequest("Invalid websiteType. Allowed values are 'linkedin' and 'github' and 'portfolio link'.");
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
