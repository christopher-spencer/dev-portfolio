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

        /*  
            **********************************************************************************************
                                            SIDEPROJECT CRUD CONTROLLER
            **********************************************************************************************
        */
        [Authorize]
        [HttpPost("/create-sideproject")]
        public ActionResult CreateSideProject(SideProject sideProject)
        {
            try
            {
                SideProject createdSideProject = _sideProjectDao.CreateSideProject(sideProject);

                if (createdSideProject == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetSideProject), new { sideProjectId = createdSideProject.Id }, createdSideProject);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the side project.");
            }
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
        public ActionResult<SideProject> GetSideProject(int sideProjectId)
        {
            SideProject sideProject = _sideProjectDao.GetSideProject(sideProjectId);

            if (sideProject == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(sideProject);
            }
        }

        [Authorize]
        [HttpPut("/update-sideproject/{sideProjectId}")]
        public ActionResult UpdateSideProject(SideProject sideProject, int sideProjectId)
        {
            try
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
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the side project.");
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/delete/{sideProjectId}")]
        public ActionResult<int> DeleteSideProject(int sideProjectId)
        {
            try
            {
                int rowsAffected = _sideProjectDao.DeleteSideProject(sideProjectId);

                if (rowsAffected > 0)
                {
                    return Ok("Side Project deleted successfully.");
                }
                else
                {
                    return NoContent();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the side project.");
            }
        }

        /*  
            **********************************************************************************************
                                        PORTFOLIO SIDEPROJECT CRUD CONTROLLER
            **********************************************************************************************
        */

        
    }
}
