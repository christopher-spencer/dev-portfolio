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
    public class EducationController : ControllerBase
    {
        private readonly IEducationDao _educationDao;

        public EducationController(IEducationDao educationDao)
        {
            _educationDao = educationDao;
        }

        /*  
            **********************************************************************************************
                                            EDUCATION CRUD CONTROLLER
            **********************************************************************************************
        */

        // [Authorize]
        // [HttpPost("/create-education")]
        // public ActionResult CreateEducation(Education education)
        // {
        //     try
        //     {
        //         Education createdEducation = _educationDao.CreateEducation(education);

        //         if (createdEducation == null)
        //         {
        //             return BadRequest();
        //         }
        //         else
        //         {
        //             return CreatedAtAction(nameof(GetEducation), new { educationId = createdEducation.Id }, createdEducation);
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while creating the education.");
        //     }
        // }

        [HttpGet("/educations")]
        public ActionResult<List<Education>> GetEducations()
        {
            List<Education> educations = _educationDao.GetEducations();

            if (educations == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(educations);
            }
        }

        [HttpGet("/education/{educationId}")]
        public ActionResult<Education> GetEducation(int educationId)
        {
            Education education = _educationDao.GetEducation(educationId);

            if (education == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(education);
            }
        }

        // [HttpPut("/update-education/{educationId}")]
        // public ActionResult UpdateEducation(int educationId, Education education)
        // {
        //     try
        //     {
        //         Education updatedEducation = _educationDao.UpdateEducation(educationId, education);

        //         if (updatedEducation == null)
        //         {
        //             return BadRequest();
        //         }
        //         else
        //         {
        //             return Ok(updatedEducation);
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while updating the education.");
        //     }
        // }

        // [HttpDelete("/delete-education/{educationId}")]
        // public ActionResult DeleteEducation(int educationId)
        // {
        //     try
        //     {
        //         int result = _educationDao.DeleteEducation(educationId);

        //         if (result == 0)
        //         {
        //             return BadRequest();
        //         }
        //         else
        //         {
        //             return NoContent();
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while deleting the education.");
        //     }
        // }

        /*  
            **********************************************************************************************
                                        PORTFOLIO EDUCATION CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/portfolio/{portfolioId}/create-education")]
        public ActionResult CreateEducationByPortfolioId(int portfolioId, Education education)
        {
            try
            {
                Education createdEducation = _educationDao.CreateEducationByPortfolioId(portfolioId, education);

                if (createdEducation == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetEducationByPortfolioId), new { portfolioId = portfolioId, educationId = createdEducation.Id }, createdEducation);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the education.");
            }
        }

        [HttpGet("/portfolio/{portfolioId}/educations")]
        public ActionResult<List<Education>> GetEducationsByPortfolioId(int portfolioId)
        {
            List<Education> educations = _educationDao.GetEducationsByPortfolioId(portfolioId);

            if (educations == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(educations);
            }
        }

        [HttpGet("/portfolio/{portfolioId}/education/{educationId}")]
        public ActionResult<Education> GetEducationByPortfolioId(int portfolioId, int educationId)
        {
            Education education = _educationDao.GetEducationByPortfolioId(portfolioId, educationId);

            if (education == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(education);
            }
        }

        [HttpPut("/portfolio/{portfolioId}/update-education/{educationId}")]
        public ActionResult UpdateEducationByPortfolioId(int portfolioId, int educationId, Education education)
        {
            try
            {
                Education updatedEducation = _educationDao.UpdateEducationByPortfolioId(portfolioId, educationId, education);

                if (updatedEducation == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedEducation);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the education.");
            }
        }

        [HttpDelete("/portfolio/{portfolioId}/delete-education/{educationId}")]
        public ActionResult DeleteEducationByPortfolioId(int portfolioId, int educationId)
        {
            try
            {
                int rowsAffected = _educationDao.DeleteEducationByPortfolioId(portfolioId, educationId);

                if (rowsAffected > 0)
                {
                    return Ok("Education deleted successfully.");
                }
                else
                {
                    return NoContent();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the education.");
            }
        }
    }
}