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
    public class VolunteerWorkController : ControllerBase
    {
        private readonly IVolunteerWorkDao _volunteerWorkDao;

        public VolunteerWorkController(IVolunteerWorkDao volunteerWorkDao)
        {
            _volunteerWorkDao = volunteerWorkDao;
        }

        /*  
            **********************************************************************************************
                                        VOLUNTEER WORK CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/create-volunteer-work")]
        public ActionResult CreateVolunteerWork(VolunteerWork volunteerWork)
        {
            try
            {
                VolunteerWork createdVolunteerWork = _volunteerWorkDao.CreateVolunteerWork(volunteerWork);

                if (createdVolunteerWork == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetVolunteerWork), new { volunteerWorkId = createdVolunteerWork.Id }, createdVolunteerWork);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the volunteer work.");
            }
        }

        [HttpGet("/volunteer-works")]
        public ActionResult<List<VolunteerWork>> GetVolunteerWorks()
        {
            List<VolunteerWork> volunteerWorks = _volunteerWorkDao.GetVolunteerWorks();

            if (volunteerWorks == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(volunteerWorks);
            }
        }

        [HttpGet("/volunteer-work/{volunteerWorkId}")]
        public ActionResult<VolunteerWork> GetVolunteerWork(int volunteerWorkId)
        {
            VolunteerWork volunteerWork = _volunteerWorkDao.GetVolunteerWork(volunteerWorkId);

            if (volunteerWork == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(volunteerWork);
            }
        }

        [Authorize]
        [HttpPut("/update-volunteer-work/{volunteerWorkId}")]
        public ActionResult UpdateVolunteerWork(int volunteerWorkId, VolunteerWork volunteerWork)
        {
            try
            {
                VolunteerWork updatedVolunteerWork = _volunteerWorkDao.UpdateVolunteerWork(volunteerWorkId, volunteerWork);

                if (updatedVolunteerWork == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedVolunteerWork);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the volunteer work.");
            }
        }

        [Authorize]
        [HttpDelete("/delete-volunteer-work/{volunteerWorkId}")]
        public ActionResult DeleteVolunteerWork(int volunteerWorkId)
        {
            try
            {
                int result = _volunteerWorkDao.DeleteVolunteerWork(volunteerWorkId);

                if (result == 0)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the volunteer work.");
            }
        }

        /*  
            **********************************************************************************************
                                    PORTFOLIO VOLUNTEER WORK CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/portfolio/{portfolioId}/create-volunteer-work")]
        public ActionResult CreateVolunteerWorkByPortfolioId(int portfolioId, VolunteerWork volunteerWork)
        {
            try
            {
                VolunteerWork createdVolunteerWork = _volunteerWorkDao.CreateVolunteerWorkByPortfolioId(portfolioId, volunteerWork);

                if (createdVolunteerWork == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetVolunteerWorkByPortfolioId), new { portfolioId = portfolioId, volunteerWorkId = createdVolunteerWork.Id }, createdVolunteerWork);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the volunteer work.");
            }
        }

        [HttpGet("/portfolio/{portfolioId}/volunteer-works")]
        public ActionResult<List<VolunteerWork>> GetVolunteerWorksByPortfolioId(int portfolioId)
        {
            List<VolunteerWork> volunteerWorks = _volunteerWorkDao.GetVolunteerWorksByPortfolioId(portfolioId);

            if (volunteerWorks == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(volunteerWorks);
            }
        }

        [HttpGet("/portfolio/{portfolioId}/volunteer-work/{volunteerWorkId}")]
        public ActionResult<VolunteerWork> GetVolunteerWorkByPortfolioId(int portfolioId, int volunteerWorkId)
        {
            VolunteerWork volunteerWork = _volunteerWorkDao.GetVolunteerWorkByPortfolioId(portfolioId, volunteerWorkId);

            if (volunteerWork == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(volunteerWork);
            }
        }

        [Authorize]
        [HttpPut("/portfolio/{portfolioId}/update-volunteer-work/{volunteerWorkId}")]
        public ActionResult UpdateVolunteerWorkByPortfolioId(int portfolioId, int volunteerWorkId, VolunteerWork volunteerWork)
        {
            try
            {
                VolunteerWork updatedVolunteerWork = _volunteerWorkDao.UpdateVolunteerWorkByPortfolioId(portfolioId, volunteerWorkId, volunteerWork);

                if (updatedVolunteerWork == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedVolunteerWork);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the volunteer work.");
            }
        }

        [Authorize]
        [HttpDelete("/portfolio/{portfolioId}/delete-volunteer-work/{volunteerWorkId}")]
        public ActionResult DeleteVolunteerWorkByPortfolioId(int portfolioId, int volunteerWorkId)
        {
            try
            {
                int result = _volunteerWorkDao.DeleteVolunteerWorkByPortfolioId(portfolioId, volunteerWorkId);

                if (result == 0)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the volunteer work.");
            }
        }

    }
}