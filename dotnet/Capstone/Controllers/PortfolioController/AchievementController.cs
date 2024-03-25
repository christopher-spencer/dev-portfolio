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
    public class AchievementController : ControllerBase
    {
        private readonly IAchievementDao _achievementDao;

        public AchievementController(IAchievementDao achievementDao)
        {
            _achievementDao = achievementDao;
        }

        /*  
            **********************************************************************************************
                                              ACHIEVEMENT CONTROLLER
            **********************************************************************************************
        */

       [HttpPost("/achievement/{achievementId}")]
        public ActionResult GetAchievement(int achievementId)
        {
            try
            {
                Achievement createdAchievement = _achievementDao.GetAchievement(achievementId);

                if (createdAchievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetAchievement), new { achievementId = createdAchievement.Id }, createdAchievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while retrieving the achievement.");
            }
        }

        [HttpGet("/achievements")]
        public ActionResult<List<Achievement>> GetAchievements()
        {
            List<Achievement> achievements = _achievementDao.GetAchievements();

            if (achievements == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(achievements);
            }
        }

        /*  
            **********************************************************************************************
                                       WORK EXPERIENCE ACHIEVEMENT CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/work-experience/{experienceId}/create-achievement")]
        public ActionResult CreateAchievementByWorkExperienceId(int experienceId, Achievement achievement)
        {
            try
            {
                Achievement createdAchievement = _achievementDao.CreateAchievementByWorkExperienceId(experienceId, achievement);

                if (createdAchievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetAchievementByWorkExperienceId), new { experienceId = experienceId, achievementId = createdAchievement.Id }, createdAchievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the achievement.");
            }
        }

        [HttpGet("/work-experience/{experienceId}/achievements")]
        public ActionResult<List<Achievement>> GetAchievementsByWorkExperienceId(int experienceId)
        {
            List<Achievement> achievements = _achievementDao.GetAchievementsByWorkExperienceId(experienceId);

            if (achievements == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(achievements);
            }
        }

        [HttpGet("/work-experience/{experienceId}/achievement/{achievementId}")]
        public ActionResult GetAchievementByWorkExperienceId(int experienceId, int achievementId)
        {
            try
            {
                Achievement achievement = _achievementDao.GetAchievementByWorkExperienceId(experienceId, achievementId);

                if (achievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(achievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while retrieving the achievement.");
            }
        }

        [Authorize]
        [HttpPut("/work-experience/{experienceId}/update-achievement/{achievementId}")] 
        public ActionResult UpdateAchievementByWorkExperienceId(int experienceId, int achievementId, Achievement achievement)
        {
            try
            {
                Achievement updatedAchievement = _achievementDao.UpdateAchievementByWorkExperienceId(experienceId, achievementId, achievement);

                if (updatedAchievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedAchievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the achievement.");
            }
        }

        [Authorize]
        [HttpDelete("/work-experience/{experienceId}/delete-achievement/{achievementId}")]
        public ActionResult DeleteAchievementByWorkExperienceId(int experienceId, int achievementId)
        {
            try
            {
                int rowsAffected = _achievementDao.DeleteAchievementByWorkExperienceId(experienceId, achievementId);

                if (rowsAffected > 0)
                {
                    return Ok("Work experience achievement deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the work experience achievement.");
            }
        }

        /*  
            **********************************************************************************************
                                          EDUCATION ACHIEVEMENT CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/education/{educationId}/create-achievement")]
        public ActionResult CreateAchievementByEducationId(int educationId, Achievement achievement)
        {
            try
            {
                Achievement createdAchievement = _achievementDao.CreateAchievementByEducationId(educationId, achievement);

                if (createdAchievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetAchievementByEducationId), new { educationId = educationId, achievementId = createdAchievement.Id }, createdAchievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the achievement.");
            }
        }

        [HttpGet("/education/{educationId}/achievements")]
        public ActionResult<List<Achievement>> GetAchievementsByEducationId(int educationId)
        {
            List<Achievement> achievements = _achievementDao.GetAchievementsByEducationId(educationId);

            if (achievements == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(achievements);
            }
        }

        [HttpGet("/education/{educationId}/achievement/{achievementId}")]
        public ActionResult GetAchievementByEducationId(int educationId, int achievementId)
        {
            try
            {
                Achievement achievement = _achievementDao.GetAchievementByEducationId(educationId, achievementId);

                if (achievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(achievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while retrieving the achievement.");
            }
        }

        [Authorize]
        [HttpPut("/education/{educationId}/update-achievement/{achievementId}")]
        public ActionResult UpdateAchievementByEducationId(int educationId, int achievementId, Achievement achievement)
        {
            try
            {
                Achievement updatedAchievement = _achievementDao.UpdateAchievementByEducationId(educationId, achievementId, achievement);

                if (updatedAchievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedAchievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the achievement.");
            }
        }

        [Authorize]
        [HttpDelete("/education/{educationId}/delete-achievement/{achievementId}")]
        public ActionResult DeleteAchievementByEducationId(int educationId, int achievementId)
        {
            try
            {
                int rowsAffected = _achievementDao.DeleteAchievementByEducationId(educationId, achievementId);

                if (rowsAffected > 0)
                {
                    return Ok("Education achievement deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the education achievement.");
            }
        }

        /*  
            **********************************************************************************************
                                    OPEN SOURCE CONTRIBUTION ACHIEVEMENT CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/open-source-contribution/{contributionId}/create-achievement")]
        public ActionResult CreateAchievementByOpenSourceContributionId(int contributionId, Achievement achievement)
        {
            try
            {
                Achievement createdAchievement = _achievementDao.CreateAchievementByOpenSourceContributionId(contributionId, achievement);

                if (createdAchievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetAchievementByOpenSourceContributionId), new { contributionId = contributionId, achievementId = createdAchievement.Id }, createdAchievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the achievement.");
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}/achievements")]
        public ActionResult<List<Achievement>> GetAchievementsByOpenSourceContributionId(int contributionId)
        {
            List<Achievement> achievements = _achievementDao.GetAchievementsByOpenSourceContributionId(contributionId);

            if (achievements == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(achievements);
            }
        }

        [HttpGet("/open-source-contribution/{contributionId}/achievement/{achievementId}")]
        public ActionResult GetAchievementByOpenSourceContributionId(int contributionId, int achievementId)
        {
            try
            {
                Achievement achievement = _achievementDao.GetAchievementByOpenSourceContributionId(contributionId, achievementId);

                if (achievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(achievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while retrieving the achievement.");
            }
        }

        [Authorize]
        [HttpPut("/open-source-contribution/{contributionId}/update-achievement/{achievementId}")]
        public ActionResult UpdateAchievementByOpenSourceContributionId(int contributionId, int achievementId, Achievement achievement)
        {
            try
            {
                Achievement updatedAchievement = _achievementDao.UpdateAchievementByOpenSourceContributionId(contributionId, achievementId, achievement);

                if (updatedAchievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedAchievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the achievement.");
            }
        }

        [Authorize]
        [HttpDelete("/open-source-contribution/{contributionId}/delete-achievement/{achievementId}")]
        public ActionResult DeleteAchievementByOpenSourceContributionId(int contributionId, int achievementId)
        {
            try
            {
                int rowsAffected = _achievementDao.DeleteAchievementByOpenSourceContributionId(contributionId, achievementId);

                if (rowsAffected > 0)
                {
                    return Ok("Open source contribution achievement deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the open source contribution achievement.");
            }
        }

        /*  
            **********************************************************************************************
                                        VOLUNTEER WORK ACHIEVEMENT CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/volunteer-work/{volunteerId}/create-achievement")]
        public ActionResult CreateAchievementByVolunteerWorkId(int volunteerId, Achievement achievement)
        {
            try
            {
                Achievement createdAchievement = _achievementDao.CreateAchievementByVolunteerWorkId(volunteerId, achievement);

                if (createdAchievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetAchievementByVolunteerWorkId), new { volunteerId = volunteerId, achievementId = createdAchievement.Id }, createdAchievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the achievement.");
            }
        }

        [HttpGet("/volunteer-work/{volunteerId}/achievements")]
        public ActionResult<List<Achievement>> GetAchievementsByVolunteerWorkId(int volunteerId)
        {
            List<Achievement> achievements = _achievementDao.GetAchievementsByVolunteerWorkId(volunteerId);

            if (achievements == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(achievements);
            }
        }

        [HttpGet("/volunteer-work/{volunteerId}/achievement/{achievementId}")]
        public ActionResult GetAchievementByVolunteerWorkId(int volunteerId, int achievementId)
        {
            try
            {
                Achievement achievement = _achievementDao.GetAchievementByVolunteerWorkId(volunteerId, achievementId);

                if (achievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(achievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while retrieving the achievement.");
            }
        }

        [Authorize]
        [HttpPut("/volunteer-work/{volunteerId}/update-achievement/{achievementId}")]
        public ActionResult UpdateAchievementByVolunteerWorkId(int volunteerId, int achievementId, Achievement achievement)
        {
            try
            {
                Achievement updatedAchievement = _achievementDao.UpdateAchievementByVolunteerWorkId(volunteerId, achievementId, achievement);

                if (updatedAchievement == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedAchievement);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the achievement.");
            }
        }

        [Authorize]
        [HttpDelete("/volunteer-work/{volunteerId}/delete-achievement/{achievementId}")]
        public ActionResult DeleteAchievementByVolunteerWorkId(int volunteerId, int achievementId)
        {
            try
            {
                int rowsAffected = _achievementDao.DeleteAchievementByVolunteerWorkId(volunteerId, achievementId);

                if (rowsAffected > 0)
                {
                    return Ok("Volunteer work achievement deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the volunteer work achievement.");
            }
        }
    }
}