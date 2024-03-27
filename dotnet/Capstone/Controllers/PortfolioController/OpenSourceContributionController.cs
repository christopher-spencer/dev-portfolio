using System.Collections.Generic;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OpenSourceContributionController : ControllerBase
    {
        private readonly IOpenSourceContributionDao _openSourceContributionDao;

        public OpenSourceContributionController(IOpenSourceContributionDao openSourceContributionDao)
        {
            _openSourceContributionDao = openSourceContributionDao;
        }

        /*  
            **********************************************************************************************
                                    OPEN SOURCE CONTRIBUTION CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/create-open-source-contribution")]
        public ActionResult CreateOpenSourceContribution(OpenSourceContribution contribution)
        {
            try
            {
                OpenSourceContribution createdContribution = _openSourceContributionDao.CreateOpenSourceContribution(contribution);

                if (createdContribution == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetOpenSourceContribution), new { contributionId = createdContribution.Id }, createdContribution);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the open source contribution.");
            }
        }

        [HttpGet("/open-source-contributions")]
        public ActionResult<List<OpenSourceContribution>> GetOpenSourceContributions()
        {
            List<OpenSourceContribution> contributions = _openSourceContributionDao.GetOpenSourceContributions();

            if (contributions == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(contributions);
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}")]
        public ActionResult<OpenSourceContribution> GetOpenSourceContribution(int contributionId)
        {
            OpenSourceContribution contribution = _openSourceContributionDao.GetOpenSourceContribution(contributionId);

            if (contribution == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(contribution);
            }
        }

        [Authorize]
        [HttpPut("/update-open-source-contribution/{contributionId}")]
        public ActionResult UpdateOpenSourceContribution(int contributionId, OpenSourceContribution contribution)
        {
            try
            {
                OpenSourceContribution updatedContribution = _openSourceContributionDao.UpdateOpenSourceContribution(contributionId, contribution);

                if (updatedContribution == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedContribution);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the open source contribution.");
            }
        }

        [Authorize]
        [HttpDelete("/delete-open-source-contribution/{contributionId}")]
        public ActionResult DeleteOpenSourceContribution(int contributionId)
        {
            try
            {
                int rowsAffected = _openSourceContributionDao.DeleteOpenSourceContribution(contributionId);

                if (rowsAffected > 0)
                {
                    return Ok("Open Source Contribution deleted successfully.");
                }
                else
                {
                    return NoContent();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the open source contribution.");
            }
        }

        /*  
            **********************************************************************************************
                                PORTFOLIO OPEN SOURCE CONTRIBUTION CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/portfolio/{portfolioId}/create-open-source-contribution")]
        public ActionResult CreateOpenSourceContributionByPortfolioId(int portfolioId, OpenSourceContribution contribution)
        {
            try
            {
                OpenSourceContribution createdContribution = _openSourceContributionDao.CreateOpenSourceContributionByPortfolioId(portfolioId, contribution);

                if (createdContribution == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetOpenSourceContributionByPortfolioId), new { portfolioId = portfolioId, contributionId = createdContribution.Id }, createdContribution);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the open source contribution.");
            }
        }

        [HttpGet("/portfolio/{portfolioId}/open-source-contributions")]
        public ActionResult<List<OpenSourceContribution>> GetOpenSourceContributionsByPortfolioId(int portfolioId)
        {
            List<OpenSourceContribution> contributions = _openSourceContributionDao.GetOpenSourceContributionsByPortfolioId(portfolioId);

            if (contributions == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(contributions);
            }
        }

        [HttpGet("/portfolio/{portfolioId}/open-source-contribution/{contributionId}")]
        public ActionResult<OpenSourceContribution> GetOpenSourceContributionByPortfolioId(int portfolioId, int contributionId)
        {
            OpenSourceContribution contribution = _openSourceContributionDao.GetOpenSourceContributionByPortfolioId(portfolioId, contributionId);

            if (contribution == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(contribution);
            }
        }

        [Authorize]
        [HttpPut("/portfolio/{portfolioId}/update-open-source-contribution/{contributionId}")]
        public ActionResult UpdateOpenSourceContributionByPortfolioId(int portfolioId, int contributionId, OpenSourceContribution contribution)
        {
            try
            {
                OpenSourceContribution updatedContribution = _openSourceContributionDao.UpdateOpenSourceContributionByPortfolioId(portfolioId, contributionId, contribution);

                if (updatedContribution == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedContribution);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the open source contribution.");
            }
        }

        [Authorize]
        [HttpDelete("/portfolio/{portfolioId}/delete-open-source-contribution/{contributionId}")]
        public ActionResult DeleteOpenSourceContributionByPortfolioId(int portfolioId, int contributionId)
        {
            try
            {
                int rowsAffected = _openSourceContributionDao.DeleteOpenSourceContributionByPortfolioId(portfolioId, contributionId);

                if (rowsAffected > 0)
                {
                    return Ok("Open Source Contribution deleted successfully.");
                }
                else
                {
                    return NoContent();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the open source contribution.");
            }
        }

    }
}