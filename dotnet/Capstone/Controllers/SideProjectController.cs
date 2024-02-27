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

        [HttpGet("/sideprojects/{id}")]
        public ActionResult<SideProject> GetSideProjectById(int id)
        {
            try
            {
                var sideProject = _sideProjectDao.GetSideProjectById(id);
                if (sideProject == null)
                {
                    return NotFound();
                }
                return Ok(sideProject);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("/create-sideproject")]
        public ActionResult<SideProject> CreateSideProject([FromBody] SideProject sideProject)
        {
            try
            {
                var newSideProject = _sideProjectDao.CreateSideProject(sideProject);
                return CreatedAtAction(nameof(GetSideProjectById), new { id = newSideProject.Id }, newSideProject);
            }
            catch (DaoException ex)
            {
                return StatusCode(500, ex.Message);
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
