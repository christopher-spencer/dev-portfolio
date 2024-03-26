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
    public class SkillController : ControllerBase
    {
        private readonly ISkillDao _skillDao;

        public SkillController(ISkillDao skillDao)
        {
            _skillDao = skillDao;
        }

        /*  
            **********************************************************************************************
                                                SKILL CRUD CONTROLLER
            **********************************************************************************************
        */

        // [Authorize]
        // [HttpPost("/create-skill")]
        // public ActionResult CreateSkill(Skill skill)
        // {
        //     try
        //     {
        //         Skill createdSkill = _skillDao.CreateSkill(skill);

        //         if (createdSkill == null)
        //         {
        //             return BadRequest();
        //         }
        //         else
        //         {
        //             return CreatedAtAction(nameof(GetSkill), new { skillId = createdSkill.Id }, createdSkill);
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while creating the skill.");
        //     }
        // }

        [HttpGet("/skill/{skillId}")]
        public ActionResult<Skill> GetSkill(int skillId)
        {
            Skill skill = _skillDao.GetSkill(skillId);

            if (skill == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(skill);
            }
        }

        [HttpGet("/skills")]
        public ActionResult<List<Skill>> GetSkills()
        {
            List<Skill> skills = _skillDao.GetSkills();

            if (skills == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(skills);
            }
        }

        // [Authorize]
        // [HttpPut("/update-skill/{skillId}")]
        // public ActionResult UpdateSkill(int skillId, Skill skill)
        // {
        //     try
        //     {
        //         Skill updatedSkill = _skillDao.UpdateSkill(skillId, skill);

        //         if (updatedSkill == null)
        //         {
        //             return BadRequest();
        //         }
        //         else
        //         {
        //             return Ok(updatedSkill);
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while updating the skill.");
        //     }
        // }

        // [Authorize]
        // [HttpDelete("/delete-skill/{skillId}")]
        // public ActionResult DeleteSkill(int skillId)
        // {
        //     try
        //     {
        //         int rowsAffected = _skillDao.DeleteSkill(skillId);

        //         if (rowsAffected > 0)
        //         {
        //             return Ok("Skill deleted successfully.");
        //         }
        //         else
        //         {
        //             return NotFound();
        //         }
        //     }
        //     catch (DaoException)
        //     {
        //         return StatusCode(500, "An error occurred while deleting the skill.");
        //     }
        // }

        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                            PORTFOLIO SKILL CRUD CONTROLLER
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/portfolio/{portfolioId}/create-skill")]
        public ActionResult CreateSkillByPortfolioId(int portfolioId, Skill skill)
        {
            try
            {
                Skill createdSkill = _skillDao.CreateSkillByPortfolioId(portfolioId, skill);

                if (createdSkill == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetSkillByPortfolioId), new { portfolioId, skillId = createdSkill.Id }, createdSkill);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the portfolio skill.");
            }
        }

        [HttpGet("/portfolio/{portfolioId}/skills")]
        public ActionResult GetSkillsByPortfolioId(int portfolioId)
        {
            List<Skill> skills = _skillDao.GetSkillsByPortfolioId(portfolioId);

            if (skills == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(skills);
            }
        }

        [HttpGet("/portfolio/{portfolioId}/skill/{skillId}")]
        public ActionResult<Skill> GetSkillByPortfolioId(int portfolioId, int skillId)
        {
            Skill skill = _skillDao.GetSkillByPortfolioId(portfolioId, skillId);

            if (skill == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(skill);
            }
        }

        [Authorize]
        [HttpPut("/portfolio/{portfolioId}/update-skill/{skillId}")]
        public ActionResult UpdateSkillByPortfolioId(int portfolioId, int skillId, Skill skill)
        {
            try
            {
                Skill updatedSkill = _skillDao.UpdateSkillByPortfolioId(portfolioId, skillId, skill);

                if (updatedSkill == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedSkill);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the portfolio skill.");
            }
        }

        [Authorize]
        [HttpDelete("/portfolio/{portfolioId}/delete-skill/{skillId}")]
        public ActionResult DeleteSkillByPortfolioId(int portfolioId, int skillId)
        {
            try
            {
                int rowsAffected = _skillDao.DeleteSkillByPortfolioId(portfolioId, skillId);

                if (rowsAffected > 0)
                {
                    return Ok("Portfolio skill deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the portfolio skill.");
            }
        }

        /*  
            **********************************************************************************************
                                            SIDE PROJECT SKILL CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/sideproject/{sideProjectId}/create-skill")]
        public ActionResult CreateSkillBySideProjectId(int sideProjectId, Skill skill)
        {
            try
            {
                Skill createdSkill = _skillDao.CreateSkillBySideProjectId(sideProjectId, skill);

                if (createdSkill == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetSkillBySideProjectId), new { sideProjectId, skillId = createdSkill.Id }, createdSkill);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the side project skill.");
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/skills")]
        public ActionResult GetSkillsBySideProjectId(int sideProjectId)
        {
            List<Skill> skills = _skillDao.GetSkillsBySideProjectId(sideProjectId);

            if (skills == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(skills);
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/skill/{skillId}")]
        public ActionResult<Skill> GetSkillBySideProjectId(int sideProjectId, int skillId)
        {
            Skill skill = _skillDao.GetSkillBySideProjectId(sideProjectId, skillId);

            if (skill == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(skill);
            }
        }
// FIXME update paths like this with multiple "updates" or "deletes" to be more specific*******
        [Authorize]
        [HttpPut("/update-sideproject/{sideProjectId}/update-skill/{skillId}")]
        public ActionResult UpdateSkillBySideProjectId(int sideProjectId, int skillId, Skill skill)
        {
            try
            {
                Skill updatedSkill = _skillDao.UpdateSkillBySideProjectId(sideProjectId, skillId, skill);

                if (updatedSkill == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedSkill);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the side project skill.");
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/{sideProjectId}/delete-skill/{skillId}")]
        public ActionResult DeleteSkillBySideProjectId(int sideProjectId, int skillId)
        {
            try
            {
                int rowsAffected = _skillDao.DeleteSkillBySideProjectId(sideProjectId, skillId);

                if (rowsAffected > 0)
                {
                    return Ok("Side project skill deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the side project skill.");
            }
        }

        /*  
            **********************************************************************************************
                                        WORK EXPERIENCE SKILL CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/work-experience/{experienceId}/create-skill")]
        public ActionResult CreateSkillByWorkExperienceId(int experienceId, Skill skill)
        {
            try
            {
                Skill createdSkill = _skillDao.CreateSkillByWorkExperienceId(experienceId, skill);

                if (createdSkill == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetSkillByWorkExperienceId), new { experienceId, skillId = createdSkill.Id }, createdSkill);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the work experience skill.");
            }
        }

        [HttpGet("/work-experience/{experienceId}/skills")]
        public ActionResult GetSkillsByWorkExperienceId(int experienceId)
        {
            List<Skill> skills = _skillDao.GetSkillsByWorkExperienceId(experienceId);

            if (skills == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(skills);
            }
        }

        [HttpGet("/work-experience/{experienceId}/skill/{skillId}")]
        public ActionResult<Skill> GetSkillByWorkExperienceId(int experienceId, int skillId)
        {
            Skill skill = _skillDao.GetSkillByWorkExperienceId(experienceId, skillId);

            if (skill == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(skill);
            }
        }

        [Authorize]
        [HttpPut("/work-experience/{experienceId}/update-skill/{skillId}")]
        public ActionResult UpdateSkillByWorkExperienceId(int experienceId, int skillId, Skill skill)
        {
            try
            {
                Skill updatedSkill = _skillDao.UpdateSkillByWorkExperienceId(experienceId, skillId, skill);

                if (updatedSkill == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedSkill);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the work experience skill.");
            }
        }

        [Authorize]
        [HttpDelete("/work-experience/{experienceId}/delete-skill/{skillId}")]
        public ActionResult DeleteSkillByWorkExperienceId(int experienceId, int skillId)
        {
            try
            {
                int rowsAffected = _skillDao.DeleteSkillByWorkExperienceId(experienceId, skillId);

                if (rowsAffected > 0)
                {
                    return Ok("Work experience skill deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the work experience skill.");
            }
        }

        /*  
            **********************************************************************************************
                                            CREDENTIAL SKILL CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/credential/{credentialId}/create-skill")]
        public ActionResult CreateSkillByCredentialId(int credentialId, Skill skill)
        {
            try
            {
                Skill createdSkill = _skillDao.CreateSkillByCredentialId(credentialId, skill);

                if (createdSkill == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetSkillByCredentialId), new { credentialId, skillId = createdSkill.Id }, createdSkill);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the credential skill.");
            }
        }

        [HttpGet("/credential/{credentialId}/skills")]
        public ActionResult GetSkillsByCredentialId(int credentialId)
        {
            List<Skill> skills = _skillDao.GetSkillsByCredentialId(credentialId);

            if (skills == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(skills);
            }
        }

        [HttpGet("/credential/{credentialId}/skill/{skillId}")]
        public ActionResult<Skill> GetSkillByCredentialId(int credentialId, int skillId)
        {
            Skill skill = _skillDao.GetSkillByCredentialId(credentialId, skillId);

            if (skill == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(skill);
            }
        }

        [Authorize]
        [HttpPut("/credential/{credentialId}/update-skill/{skillId}")]
        public ActionResult UpdateSkillByCredentialId(int credentialId, int skillId, Skill skill)
        {
            try
            {
                Skill updatedSkill = _skillDao.UpdateSkillByCredentialId(credentialId, skillId, skill);

                if (updatedSkill == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedSkill);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the credential skill.");
            }
        }

        [Authorize]
        [HttpDelete("/credential/{credentialId}/delete-skill/{skillId}")]
        public ActionResult DeleteSkillByCredentialId(int credentialId, int skillId)
        {
            try
            {
                int rowsAffected = _skillDao.DeleteSkillByCredentialId(credentialId, skillId);

                if (rowsAffected > 0)
                {
                    return Ok("Credential skill deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the credential skill.");
            }
        }

        /*  
            **********************************************************************************************
                                     OPEN SOURCE CONTRIBUTION SKILL CRUD CONTROLLER
            **********************************************************************************************
        */

        /*  
            **********************************************************************************************
                                            VOLUNTEER WORK SKILL CRUD CONTROLLER
            **********************************************************************************************
        */
    }
}
