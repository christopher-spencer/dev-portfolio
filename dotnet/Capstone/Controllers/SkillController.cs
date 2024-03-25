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

        [Authorize]
        [HttpPost("/create-skill")]
        public ActionResult CreateSkill(Skill skill)
        {
            try
            {
                Skill createdSkill = _skillDao.CreateSkill(skill);

                if (createdSkill == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetSkill), new { skillId = createdSkill.Id }, createdSkill);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the skill.");
            }
        }

        [HttpGet("/skill/{skillId}")]
        public ActionResult<Skill> GetSkill(int skillId)
        {
            Skill skill = _skillDao.GetSkill(skillId);

            if (skill == null)
            {
                return NotFound();
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

        [Authorize]
        [HttpPut("/update-skill/{skillId}")]
        public ActionResult UpdateSkill(int skillId, Skill skill)
        {
            try
            {
                Skill updatedSkill = _skillDao.UpdateSkill(skillId, skill);

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
                return StatusCode(500, "An error occurred while updating the skill.");
            }
        }

        [Authorize]
        [HttpDelete("/delete-skill/{skillId}")]
        public ActionResult DeleteSkill(int skillId)
        {
            try
            {
                int rowsAffected = _skillDao.DeleteSkill(skillId);

                if (rowsAffected > 0)
                {
                    return Ok("Skill deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the skill.");
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
    }
}
