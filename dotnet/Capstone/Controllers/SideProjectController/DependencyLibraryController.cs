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
    public class DependencyLibraryController : ControllerBase
    {
        private readonly IDependencyLibraryDao _dependencyLibraryDao;

        public DependencyLibraryController(IDependencyLibraryDao dependencyLibraryDao)
        {
            _dependencyLibraryDao = dependencyLibraryDao;
        }

        /*  
            **********************************************************************************************
                                        DEPENDENCIES AND LIBRARIES CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/create-dependency-or-library")]
        public ActionResult CreateDependencyOrLibrary(DependencyLibrary dependencyLibrary)
        {
            try
            {
                DependencyLibrary createdDependencyLibrary = _dependencyLibraryDao.CreateDependencyOrLibrary(dependencyLibrary);

                if (createdDependencyLibrary == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetDependencyOrLibraryById), new { dependencyLibraryId = createdDependencyLibrary.Id }, createdDependencyLibrary);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the dependency or library.");
            }
        }

        [HttpGet("/dependency-or-library/{dependencyLibraryId}")]
        public ActionResult<DependencyLibrary> GetDependencyOrLibraryById(int dependencyLibraryId)
        {
            DependencyLibrary dependencyLibrary = _dependencyLibraryDao.GetDependencyOrLibraryById(dependencyLibraryId);

            if (dependencyLibrary == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(dependencyLibrary);
            }
        }

        [HttpGet("/get-all-dependencies-and-libraries")]
        public ActionResult<List<DependencyLibrary>> GetAllDependenciesAndLibraries()
        {
            List<DependencyLibrary> dependenciesAndLibraries = _dependencyLibraryDao.GetDependenciesAndLibraries();

            if (dependenciesAndLibraries == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(dependenciesAndLibraries);
            }
        }

        [Authorize]
        [HttpPut("update-dependency-or-library/{dependencyLibraryId}")]
        public ActionResult UpdateDependencyOrLibrary(int dependencyLibraryId, DependencyLibrary dependencyLibrary)
        {
            try
            {
                DependencyLibrary updatedDependencyLibrary = _dependencyLibraryDao.UpdateDependencyOrLibrary(dependencyLibraryId, dependencyLibrary);

                if (updatedDependencyLibrary == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedDependencyLibrary);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the dependency or library.");
            }
        }

        [Authorize]
        [HttpDelete("/delete-dependency-or-library/{dependencyLibraryId}")]
        public ActionResult DeleteDependencyOrLibraryById(int dependencyLibraryId)
        {
            try
            {
                int rowsAffected = _dependencyLibraryDao.DeleteDependencyOrLibrary(dependencyLibraryId);

                if (rowsAffected > 0)
                {
                    return Ok("Dependency or library deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the dependency or library.");
            }
        }

        /*  
            **********************************************************************************************
                                    SIDE PROJECT DEPENDENCIES AND LIBRARIES CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/sideproject/{projectId}/create-dependency-or-library")]
        public ActionResult CreateDependencyOrLibraryBySideProjectId(int projectId, DependencyLibrary dependencyLibrary)
        {
            try
            {
                DependencyLibrary createdDependencyLibrary = _dependencyLibraryDao.CreateDependencyOrLibraryBySideProjectId(projectId, dependencyLibrary);

                if (createdDependencyLibrary == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetDependencyOrLibraryBySideProjectId), new { projectId = projectId, dependencyLibraryId = createdDependencyLibrary.Id }, createdDependencyLibrary);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the side project dependency or library.");
            }
        }

        [HttpGet("/sideproject/{projectId}/dependencies-and-libraries")]
        public ActionResult GetDependenciesAndLibrariesBySideProjectId(int projectId)
        {
            List<DependencyLibrary> dependenciesAndLibraries = _dependencyLibraryDao.GetDependenciesAndLibrariesBySideProjectId(projectId);

            if (dependenciesAndLibraries == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(dependenciesAndLibraries);
            }
        }

        [HttpGet("/sideproject/{projectId}/dependency-or-library/{dependencyLibraryId}")]
        public ActionResult<DependencyLibrary> GetDependencyOrLibraryBySideProjectId(int projectId, int dependencyLibraryId)
        {
            DependencyLibrary dependencyLibrary = _dependencyLibraryDao.GetDependencyOrLibraryBySideProjectId(projectId, dependencyLibraryId);

            if (dependencyLibrary == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(dependencyLibrary);
            }
        }

        [Authorize]
        [HttpPut("/update-sideproject/{projectId}/update-dependency-or-library/{dependencyLibraryId}")]
        public ActionResult UpdateDependencyOrLibraryBySideProjectId(int projectId, int dependencyLibraryId, DependencyLibrary dependencyLibrary)
        {
            try
            {
                DependencyLibrary updatedDependencyLibrary = _dependencyLibraryDao.UpdateDependencyOrLibraryBySideProjectId(projectId, dependencyLibraryId, dependencyLibrary);

                if (updatedDependencyLibrary == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedDependencyLibrary);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the side project dependency or library.");
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/{projectId}/delete-dependency-or-library/{dependencyLibraryId}")]
        public ActionResult DeleteDependencyOrLibraryBySideProjectId(int projectId, int dependencyLibraryId)
        {
            try
            {
                int rowsAffected = _dependencyLibraryDao.DeleteDependencyOrLibraryBySideProjectId(projectId, dependencyLibraryId);

                if (rowsAffected > 0)
                {
                    return Ok("Side project dependency or library deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the side project dependency or library.");
            }
        }
    }
}
