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
                    return CreatedAtAction(nameof(GetGoalById), new { goalId = createdGoal.Id }, createdGoal);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while creating the goal.");
            }
        }

        [HttpGet("/goal/{goalId}")]
        public ActionResult<Goal> GetGoalById(int goalId)
        {
            Goal goal = _goalDao.GetGoalById(goalId);

            if (goal == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(goal);
            }
        }

        [HttpGet("/get-all-goals")]
        public ActionResult<List<Goal>> GetAllGoals()
        {
            List<Goal> goals = _goalDao.GetAllGoals();

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
                Goal updatedGoal = _goalDao.UpdateGoal(goal);

                if (updatedGoal == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedGoal);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while updating the goal.");
            }
        }

        [Authorize]
        [HttpDelete("/delete-goal/{goalId}")]
        public ActionResult DeleteGoalById(int goalId)
        {
            try
            {
                int rowsAffected = _goalDao.DeleteGoalById(goalId);

                if (rowsAffected > 0)
                {
                    return Ok("Goal deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException ex)
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
        [HttpPost("/sideproject/{projectId}/create-goal")]
        public ActionResult CreateGoalBySideProjectId(int projectId, Goal goal)
        {
            try
            {
                Goal createdGoal = _goalDao.CreateGoalBySideProjectId(projectId, goal);

                if (createdGoal == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetGoalBySideProjectId), new { projectId = projectId, goalId = createdGoal.Id }, createdGoal);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while creating the side project goal.");
            }
        }

        [HttpGet("/sideproject/{projectId}/goals")]
        public ActionResult GetGoalsBySideProjectId(int projectId)
        {
            List<Goal> goals = _goalDao.GetGoalsBySideProjectId(projectId);

            if (goals == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(goals);
            }
        }

        [HttpGet("/sideproject/{projectId}/goal/{goalId}")]
        public ActionResult<Goal> GetGoalBySideProjectId(int projectId, int goalId)
        {
            Goal goal = _goalDao.GetGoalBySideProjectId(projectId, goalId);

            if (goal == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(goal);
            }
        }

        //FIXME add goalId?
        [Authorize]
        [HttpPut("/update-sideproject/{projectId}/update-goal")]
        public ActionResult UpdateGoalBySideProjectId(int projectId, Goal goal)
        {
            try
            {
                Goal updatedGoal = _goalDao.UpdateGoalBySideProjectId(projectId, goal);

                if (updatedGoal == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedGoal);
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while updating the side project goal.");
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/{projectId}/delete-goal/{goalId}")]
        public ActionResult DeleteGoalBySideProjectId(int projectId, int goalId)
        {
            try
            {
                int rowsAffected = _goalDao.DeleteGoalBySideProjectId(projectId, goalId);

                if (rowsAffected > 0)
                {
                    return Ok("Side project goal deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while deleting the side project goal.");
            }
        }
    }
}
