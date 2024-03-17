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
    public class GoalController : ControllerBase
    {
        private readonly IGoalDao _goalDao;

        public GoalController(IGoalDao goalDao)
        {
            _goalDao = goalDao;
        }

        /*  
            **********************************************************************************************
                                                    GOAL CRUD CONTROLLER
            **********************************************************************************************
        */
// TODO TEST ALL POSTMAN CRUD

        [Authorize]
        [HttpPost("/create-goal")]
        public ActionResult CreateGoal(Goal goal)
        {
            try
            {
                Goal createdGoal = _goalDao.CreateGoal(goal);

                if (createdGoal == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetGoal), new { goalId = createdGoal.Id }, createdGoal);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the goal.");
            }
        }

        [HttpGet("/goal/{goalId}")]
        public ActionResult<Goal> GetGoal(int goalId)
        {
            Goal goal = _goalDao.GetGoal(goalId);

            if (goal == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(goal);
            }
        }

        [HttpGet("/goals")]
        public ActionResult<List<Goal>> GetGoals()
        {
            List<Goal> goals = _goalDao.GetGoals();

            if (goals == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(goals);
            }
        }

        [Authorize]
        [HttpPut("/update-goal/{goalId}")]
        public ActionResult UpdateGoal(int goalId, Goal goal)
        {
            try
            {
                Goal updatedGoal = _goalDao.UpdateGoal(goalId, goal);

                if (updatedGoal == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedGoal);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the goal.");
            }
        }

        [Authorize]
        [HttpDelete("/delete-goal/{goalId}")]
        public ActionResult DeleteGoal(int goalId)
        {
            try
            {
                int rowsAffected = _goalDao.DeleteGoal(goalId);

                if (rowsAffected > 0)
                {
                    return Ok("Goal deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the goal.");
            }
        }

        /*  
            **********************************************************************************************
                                            SIDE PROJECT GOAL CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/sideproject/{sideProjectId}/create-goal")]
        public ActionResult CreateGoalBySideProjectId(int sideProjectId, Goal goal)
        {
            try
            {
                Goal createdGoal = _goalDao.CreateGoalBySideProjectId(sideProjectId, goal);

                if (createdGoal == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetGoalBySideProjectId), new { sideProjectId, goalId = createdGoal.Id }, createdGoal);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the side project goal.");
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/goals")]
        public ActionResult GetGoalsBySideProjectId(int sideProjectId)
        {
            List<Goal> goals = _goalDao.GetGoalsBySideProjectId(sideProjectId);

            if (goals == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(goals);
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/goal/{goalId}")]
        public ActionResult<Goal> GetGoalBySideProjectId(int sideProjectId, int goalId)
        {
            Goal goal = _goalDao.GetGoalBySideProjectId(sideProjectId, goalId);

            if (goal == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(goal);
            }
        }

        [Authorize]
        [HttpPut("/update-sideproject/{sideProjectId}/update-goal/{goalId}")]
        public ActionResult UpdateGoalBySideProjectId(int sideProjectId, int goalId, Goal goal)
        {
            try
            {
                Goal updatedGoal = _goalDao.UpdateGoalBySideProjectId(sideProjectId, goalId, goal);

                if (updatedGoal == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedGoal);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the side project goal.");
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/{sideProjectId}/delete-goal/{goalId}")]
        public ActionResult DeleteGoalBySideProjectId(int sideProjectId, int goalId)
        {
            try
            {
                int rowsAffected = _goalDao.DeleteGoalBySideProjectId(sideProjectId, goalId);

                if (rowsAffected > 0)
                {
                    return Ok("Side project goal deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the side project goal.");
            }
        }
    }
}
