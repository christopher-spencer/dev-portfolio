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

        [Authorize]
        [HttpPost("/create-contributor")]
        public ActionResult CreateContributor(Contributor contributor)
        {
            try
            {
                Contributor createdContributor = _contributorDao.CreateContributor(contributor);

                if (createdContributor == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetContributorById), new { contributorId = createdContributor.Id }, createdContributor);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the contributor.");
            }
        }

        [HttpGet("/contributor/{contributorId}")]
        public ActionResult<Contributor> GetContributorById(int contributorId)
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

        [HttpGet("get-all-contributors")]
        public ActionResult<List<Contributor>> GetAllContributors()
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

        [Authorize]
        [HttpPut("/update-contributor/{contributorId}")]
        public ActionResult UpdateContributor(int contributorId, Contributor contributor)
        {
            try
            {
                Contributor updatedContributor = _contributorDao.UpdateContributor(contributorId, contributor);

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
                return StatusCode(500, "An error occurred while updating the contributor.");
            }
        }

        [Authorize]
        [HttpDelete("/delete-contributor/{contributorId}")]
        public ActionResult DeleteContributorById(int contributorId)
        {
            try
            {
                int rowsAffected = _contributorDao.DeleteContributor(contributorId);

                if (rowsAffected > 0)
                {
                    return Ok("Contributor deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the contributor.");
            }
        }

        /*  
            **********************************************************************************************
                                            SIDE PROJECT CONTRIBUTOR CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/sideproject/{projectId}/create-contributor")]
        public ActionResult CreateContributorBySideProjectId(int projectId, Contributor contributor)
        {
            try
            {
                Contributor createdContributor = _contributorDao.CreateContributorBySideProjectId(projectId, contributor);

                if (createdContributor == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetContributorBySideProjectId), new { projectId = projectId, contributorId = createdContributor.Id }, createdContributor);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the side project contributor.");
            }
        }

        [HttpGet("/sideproject/{projectId}/contributors")]
        public ActionResult GetContributorsBySideProjectId(int projectId)
        {
            List<Contributor> contributors = _contributorDao.GetContributorsBySideProjectId(projectId);

            if (contributors == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(contributors);
            }
        }

        [HttpGet("/sideproject/{projectId}/contributor/{contributorId}")]
        public ActionResult<Contributor> GetContributorBySideProjectId(int projectId, int contributorId)
        {
            Contributor contributor = _contributorDao.GetContributorBySideProjectId(projectId, contributorId);

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
        [HttpPut("/update-sideproject/{projectId}/update-contributor/{contributorId}")]
        public ActionResult UpdateContributorBySideProjectId(int projectId, int contributorId, Contributor contributor)
        {
            try
            {
                Contributor updatedContributor = _contributorDao.UpdateContributorBySideProjectId(projectId, contributorId, contributor);

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
        [HttpDelete("/sideproject/{projectId}/delete-contributor/{contributorId}")]
        public ActionResult DeleteContributorBySideProjectId(int projectId, int contributorId)
        {
            try
            {
                int rowsAffected = _contributorDao.DeleteContributorBySideProjectId(projectId, contributorId);

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
