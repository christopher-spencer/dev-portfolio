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
    public class WorkExperienceController : ControllerBase
    {
        private readonly IWorkExperienceDao _workExperienceDao;

        public WorkExperienceController(IWorkExperienceDao workExperienceDao)
        {
            _workExperienceDao = workExperienceDao;
        }

        /*  
            **********************************************************************************************
                                        WORK EXPERIENCE CRUD CONTROLLER
            **********************************************************************************************
        */

        // [Authorize]
        // [HttpPost("/create-work-experience")]
        // public ActionResult CreateWorkExperience(WorkExperience experience)
        // {
        //     try
        //     {
        //         WorkExperience createdWorkExperience = _workExperienceDao.CreateWorkExperience(experience);

        //         if (createdWorkExperience == null)
        //         {
        //             return BadRequest();
        //         }
        //         else
        //         {
        //             return CreatedAtAction(nameof(GetWorkExperience), new { experienceId = createdWorkExperience.Id }, createdWorkExperience);
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while creating the work experience.");
        //     }
        // }

        [HttpGet("/work-experiences")]
        public ActionResult<List<WorkExperience>> GetWorkExperiences()
        {
            List<WorkExperience> workExperiences = _workExperienceDao.GetWorkExperiences();

            if (workExperiences == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(workExperiences);
            }
        }

        [HttpGet("/work-experience/{experienceId}")]
        public ActionResult<WorkExperience> GetWorkExperience(int experienceId)
        {
            WorkExperience workExperience = _workExperienceDao.GetWorkExperience(experienceId);

            if (workExperience == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(workExperience);
            }
        }

        // [Authorize]
        // [HttpPut("/update-work-experience/{experienceId}")]
        // public ActionResult UpdateWorkExperience(int experienceId, WorkExperience experience)
        // {
        //     try
        //     {
        //         WorkExperience updatedWorkExperience = _workExperienceDao.UpdateWorkExperience(experienceId, experience);

        //         if (updatedWorkExperience == null)
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             return Ok(updatedWorkExperience);
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while updating the work experience.");
        //     }
        // }

        // [Authorize]
        // [HttpDelete("/work-experience/delete/{experienceId}")]
        // public ActionResult DeleteWorkExperience(int experienceId)
        // {
        //     try
        //     {
        //         int result = _workExperienceDao.DeleteWorkExperience(experienceId);

        //         if (result == 0)
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             return NoContent();
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while deleting the work experience.");
        //     }
        // }

        /*  
            **********************************************************************************************
                                    PORTFOLIO WORK EXPERIENCE CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/portfolio/{portfolioId}/create-work-experience")]
        public ActionResult CreateWorkExperienceByPortfolioId(int portfolioId, WorkExperience experience)
        {
            try
            {
                WorkExperience createdWorkExperience = _workExperienceDao.CreateWorkExperienceByPortfolioId(portfolioId, experience);

                if (createdWorkExperience == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetWorkExperienceByPortfolioId), new { portfolioId = portfolioId, experienceId = createdWorkExperience.Id }, createdWorkExperience);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the work experience.");
            }
        }

        [HttpGet("/portfolio/{portfolioId}/work-experiences")]
        public ActionResult<List<WorkExperience>> GetWorkExperiencesByPortfolioId(int portfolioId)
        {
            List<WorkExperience> workExperiences = _workExperienceDao.GetWorkExperiencesByPortfolioId(portfolioId);

            if (workExperiences == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(workExperiences);
            }
        }

        [HttpGet("/portfolio/{portfolioId}/work-experience/{experienceId}")]
        public ActionResult<WorkExperience> GetWorkExperienceByPortfolioId(int portfolioId, int experienceId)
        {
            WorkExperience workExperience = _workExperienceDao.GetWorkExperienceByPortfolioId(portfolioId, experienceId);

            if (workExperience == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(workExperience);
            }
        }

        [Authorize]
        [HttpPut("/portfolio/{portfolioId}/update-work-experience/{experienceId}")]
        public ActionResult UpdateWorkExperienceByPortfolioId(int portfolioId, int experienceId, WorkExperience experience)
        {
            try
            {
                WorkExperience updatedWorkExperience = _workExperienceDao.UpdateWorkExperienceByPortfolioId(portfolioId, experienceId, experience);

                if (updatedWorkExperience == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(updatedWorkExperience);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the work experience.");
            }
        }

        [Authorize]
        [HttpDelete("/portfolio/{portfolioId}/work-experience/delete/{experienceId}")]
        public ActionResult DeleteWorkExperienceByPortfolioId(int portfolioId, int experienceId)
        {
            try
            {
                int result = _workExperienceDao.DeleteWorkExperienceByPortfolioId(portfolioId, experienceId);

                if (result == 0)
                {
                    return NotFound();
                }
                else
                {
                    return NoContent();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the work experience.");
            }
        }
        
    }
}