using System.Collections.Generic;
using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{

// TODO fix controller based on BlogPostController
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
            try
            {
                var sideProjects = _sideProjectDao.GetSideProjects();
                return Ok(sideProjects);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
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
        [HttpPut("/update-sideproject/{id}")]
        public ActionResult<SideProject> UpdateSideProject(int id, [FromBody] SideProject sideProject)
        {
            try
            {
                var updatedSideProject = _sideProjectDao.UpdateSideProject(sideProject, id);

                if (updatedSideProject == null)
                {
                    return NotFound();
                }
                return Ok(updatedSideProject);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/delete/{id}")]
        public ActionResult<int> DeleteSideProject(int id)
        {
            try
            {
                var numberOfRowsAffected = _sideProjectDao.DeleteSideProjectBySideProjectId(id);
                if (numberOfRowsAffected == 0)
                {
                    return NotFound();
                }
                return Ok(numberOfRowsAffected);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
