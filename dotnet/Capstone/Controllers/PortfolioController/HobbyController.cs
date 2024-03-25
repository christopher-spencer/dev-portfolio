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
    public class HobbyController : ControllerBase
    {
        private readonly IHobbyDao _hobbyDao;

        public HobbyController(IHobbyDao hobbyDao)
        {
            _hobbyDao = hobbyDao;
        }

        /*  
            **********************************************************************************************
                                                HOBBY CRUD CONTROLLER
            **********************************************************************************************
        */

        [HttpGet("/hobby/{hobbyId}")]
        public ActionResult<Hobby> GetHobby(int hobbyId)
        {
            Hobby hobby = _hobbyDao.GetHobby(hobbyId);

            if (hobby == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(hobby);
            }
        }

        [HttpGet("/hobbies")]
        public ActionResult<List<Hobby>> GetHobbies()
        {
            List<Hobby> hobbies = _hobbyDao.GetHobbies();

            if (hobbies == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(hobbies);
            }
        }

        /*  
            **********************************************************************************************
                                          PORTFOLIO HOBBY CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/portfolio/{portfolioId}/create-hobby")]
        public ActionResult<Hobby> CreateHobbyByPortfolioId(int portfolioId, Hobby hobby)
        {
            try
            {
                Hobby createdHobby = _hobbyDao.CreateHobbyByPortfolioId(portfolioId, hobby);

                if (createdHobby == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetHobbyByPortfolioId), new { portfolioId = portfolioId, hobbyId = createdHobby.Id }, createdHobby);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the hobby.");
            }
        }

        [HttpGet("/portfolio/{portfolioId}/hobbies")]
        public ActionResult<List<Hobby>> GetHobbiesByPortfolioId(int portfolioId)
        {
            List<Hobby> hobbies = _hobbyDao.GetHobbiesByPortfolioId(portfolioId);

            if (hobbies == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(hobbies);
            }
        }

        [HttpGet("/portfolio/{portfolioId}/hobby/{hobbyId}")]
        public ActionResult<Hobby> GetHobbyByPortfolioId(int portfolioId, int hobbyId)
        {
            Hobby hobby = _hobbyDao.GetHobbyByPortfolioId(portfolioId, hobbyId);

            if (hobby == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(hobby);
            }
        }

        [Authorize]
        [HttpPut("/portfolio/{portfolioId}/update-hobby/{hobbyId}")]
        public ActionResult<Hobby> UpdateHobbyByPortfolioId(int portfolioId, int hobbyId, Hobby hobby)
        {
            try
            {
                Hobby updatedHobby = _hobbyDao.UpdateHobbyByPortfolioId(portfolioId, hobbyId, hobby);

                if (updatedHobby == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedHobby);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the hobby.");
            }
        }

        [Authorize]
        [HttpDelete("/portfolio/{portfolioId}/delete-hobby/{hobbyId}")]
        public ActionResult DeleteHobbyByPortfolioId(int portfolioId, int hobbyId)
        {
            try
            {
                int rowsAffected = _hobbyDao.DeleteHobbyByPortfolioId(portfolioId, hobbyId);

                if (rowsAffected > 0)
                {
                    return Ok("Portfolio hobby deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the hobby from portfolio.");
            }
        }
    }
}