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
                    return CreatedAtAction(nameof(GetSkillById), new { skillId = createdSkill.Id }, createdSkill);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the skill.");
            }
        }

        [HttpGet("/skill/{skillId}")]
        public ActionResult<Skill> GetSkillById(int skillId)
        {
            Skill skill = _skillDao.GetSkillById(skillId);

            if (skill == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(skill);
            }
        }

        [HttpGet("/get-all-skills")]
        public ActionResult<List<Skill>> GetAllSkills()
        {
            List<Skill> skills = _skillDao.GetAllSkills();

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
        [HttpPost("/sideproject/{projectId}/create-skill")]
        public ActionResult CreateSkillBySideProjectId(int projectId, Skill skill)
        {
            try
            {
                Skill createdSkill = _skillDao.CreateSkillBySideProjectId(projectId, skill);

                if (createdSkill == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetSkillBySideProjectId), new { projectId = projectId, skillId = createdSkill.Id }, createdSkill);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the side project skill.");
            }
        }

        [HttpGet("/sideproject/{projectId}/skills")]
        public ActionResult GetSkillsBySideProjectId(int projectId)
        {
            List<Skill> skills = _skillDao.GetSkillsBySideProjectId(projectId);

            if (skills == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(skills);
            }
        }

        [HttpGet("/sideproject/{projectId}/skill/{skillId}")]
        public ActionResult<Skill> GetSkillBySideProjectId(int projectId, int skillId)
        {
            Skill skill = _skillDao.GetSkillBySideProjectId(projectId, skillId);

            if (skill == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(skill);
            }
        }

        [Authorize]
        [HttpPut("/update-sideproject/{projectId}/update-skill/{skillId}")]
        public ActionResult UpdateSkillBySideProjectId(int projectId, int skillId, Skill skill)
        {
            try
            {
                Skill updatedSkill = _skillDao.UpdateSkillBySideProjectId(projectId, skill);

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
        [HttpDelete("/sideproject/{projectId}/delete-skill/{skillId}")]
        public ActionResult DeleteSkillBySideProjectId(int projectId, int skillId)
        {
            try
            {
                int rowsAffected = _skillDao.DeleteSkillBySideProjectId(projectId, skillId);

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
