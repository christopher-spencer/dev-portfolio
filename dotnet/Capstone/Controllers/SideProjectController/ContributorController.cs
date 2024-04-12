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
    public class ContributorController : ControllerBase
    {
        private readonly IContributorDao _contributorDao;

        public ContributorController(IContributorDao contributorDao)
        {
            _contributorDao = contributorDao;
        }

        /*  
            **********************************************************************************************
                                                CONTRIBUTOR CRUD CONTROLLER
            **********************************************************************************************
        */

        [HttpGet("/contributor/{contributorId}")]
        public ActionResult<Contributor> GetContributor(int contributorId)
        {
            Contributor contributor = _contributorDao.GetContributor(contributorId);

            if (contributor == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(contributor);
            }
        }

        [HttpGet("/contributors")]
        public ActionResult<List<Contributor>> GetContributors()
        {
            List<Contributor> contributors = _contributorDao.GetContributors();

            if (contributors == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(contributors);
            }
        }

        /*  
            **********************************************************************************************
                                            SIDE PROJECT CONTRIBUTOR CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/sideproject/{sideProjectId}/create-contributor")]
        public ActionResult CreateContributorBySideProjectId(int sideProjectId, Contributor contributor)
        {
            try
            {
                Contributor createdContributor = _contributorDao.CreateContributorBySideProjectId(sideProjectId, contributor);

                if (createdContributor == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetContributorBySideProjectId), new { sideProjectId, contributorId = createdContributor.Id }, createdContributor);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the side project contributor.");
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/contributors")]
        public ActionResult GetContributorsBySideProjectId(int sideProjectId)
        {
            List<Contributor> contributors = _contributorDao.GetContributorsBySideProjectId(sideProjectId);

            if (contributors == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(contributors);
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/contributor/{contributorId}")]
        public ActionResult<Contributor> GetContributorBySideProjectId(int sideProjectId, int contributorId)
        {
            Contributor contributor = _contributorDao.GetContributorBySideProjectId(sideProjectId, contributorId);

            if (contributor == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(contributor);
            }
        }

        [Authorize]
        [HttpPut("/update-sideproject/{sideProjectId}/update-contributor/{contributorId}")]
        public ActionResult UpdateContributorBySideProjectId(int sideProjectId, int contributorId, Contributor contributor)
        {
            try
            {
                Contributor updatedContributor = _contributorDao.UpdateContributorBySideProjectId(sideProjectId, contributorId, contributor);

                if (updatedContributor == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedContributor);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the side project contributor.");
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/{sideProjectId}/delete-contributor/{contributorId}")]
        public ActionResult DeleteContributorBySideProjectId(int sideProjectId, int contributorId)
        {
            try
            {
                int rowsAffected = _contributorDao.DeleteContributorBySideProjectId(sideProjectId, contributorId);

                if (rowsAffected > 0)
                {
                    return Ok("Side project contributor deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the side project contributor.");
            }
        }
    }
}
