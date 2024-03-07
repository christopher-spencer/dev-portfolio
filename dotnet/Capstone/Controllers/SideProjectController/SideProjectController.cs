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
    public class SideProjectController : ControllerBase
    {
        private readonly ISideProjectDao _sideProjectDao;

        public SideProjectController(ISideProjectDao sideProjectDao)
        {
            _sideProjectDao = sideProjectDao;
        }

        [HttpGet("/sideprojects")]
        public ActionResult<List<SideProject>> GetSideProjects()
        {
            List<SideProject> sideProjects = _sideProjectDao.GetSideProjects();

            if (sideProjects == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(sideProjects);
            }
        }

        [HttpGet("/sideproject/{sideProjectId}")]
        public ActionResult<SideProject> GetSideProjectById(int sideProjectId)
        {
            SideProject sideProject = _sideProjectDao.GetSideProjectById(sideProjectId);

            if (sideProject == null) 
            {
                return NotFound();
            }
            else 
            {
                return Ok(sideProject);
            }
        }

        [Authorize]
        [HttpPost("/create-sideproject")]
        public ActionResult CreateSideProject(SideProject sideProject)
        {
            SideProject createdSideProject = _sideProjectDao.CreateSideProject(sideProject);

            if (createdSideProject == null) 
            {
                return BadRequest();
            }
            else
            {
                return CreatedAtAction(nameof(GetSideProjectById), new { sideProjectId = createdSideProject.Id }, createdSideProject);
            }
        }

        [Authorize]
        [HttpPut("/update-sideproject/{sideProjectId}")]
        public ActionResult UpdateSideProject(SideProject sideProject, int sideProjectId)
        {
            SideProject updatedSideProject = _sideProjectDao.UpdateSideProject(sideProject, sideProjectId);

            if (updatedSideProject == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(updatedSideProject);
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/delete/{sideProjectId}")]
        public ActionResult<int> DeleteSideProject(int sideProjectId)
        {
            try
            {
                int rowsAffected = _sideProjectDao.DeleteSideProjectBySideProjectId(sideProjectId);

                if (rowsAffected > 0)
                {
                    return Ok("Side Project deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while deleting the side project.");
            }
        }
    }
}
