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
    public class ApiServiceController : ControllerBase
    {
        private readonly IApiServiceDao _apiServiceDao;

        public ApiServiceController(IApiServiceDao apiServiceDao)
        {
            _apiServiceDao = apiServiceDao;
        }

        /*  
            **********************************************************************************************
                                        APIS AND SERVICES CRUD CONTROLLER
            **********************************************************************************************
        */ 

        [Authorize]
        [HttpPost("/create-api-or-service")]
        public ActionResult CreateAPIOrService(ApiService apiService)
        {
            try
            {
                ApiService createdApiService = _apiServiceDao.CreateAPIOrService(apiService);

                if (createdApiService == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetAPIOrServiceById), new { apiServiceId = createdApiService.Id }, createdApiService);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the API/service.");
            }
        }

        [HttpGet("/api-or-service/{apiServiceId}")]
        public ActionResult<ApiService> GetAPIOrServiceById(int apiServiceId)
        {
            ApiService apiService = _apiServiceDao.GetAPIOrServiceById(apiServiceId);

            if (apiService == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(apiService);
            }
        }

        [HttpGet("/get-all-apis-and-services")]
        public ActionResult<List<ApiService>> GetAllAPIsAndServices()
        {
            List<ApiService> apiServices = _apiServiceDao.GetAllAPIsAndServices();

            if (apiServices == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(apiServices);
            }
        }

        [Authorize]
        [HttpPut("/update-api-or-service/{apiServiceId}")]
        public ActionResult UpdateAPIOrService(int apiServiceId, ApiService apiService)
        {
            try
            {
                ApiService updatedApiService = _apiServiceDao.UpdateAPIOrService(apiServiceId, apiService);

                if (updatedApiService == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedApiService);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the API/service.");
            }
        }

        [Authorize]
        [HttpDelete("/delete-api-or-service/{apiServiceId}")]
        public ActionResult DeleteAPIOrService(int apiServiceId)
        {
            try
            {
                int rowsAffected = _apiServiceDao.DeleteAPIOrService(apiServiceId);

                if (rowsAffected > 0)
                {
                    return Ok("API/service deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the API/service.");
            }
        }

        /*  
            **********************************************************************************************
                                SIDE PROJECT APIS AND SERVICES CRUD CONTROLLER
            **********************************************************************************************
        */ 

        [Authorize]
        [HttpPost("/sideproject/{projectId}/create-api-or-service")]
        public ActionResult CreateAPIOrServiceBySideProjectId(int projectId, ApiService apiService)
        {
            try
            {
                ApiService createdApiService = _apiServiceDao.CreateAPIOrServiceBySideProjectId(projectId, apiService);

                if (createdApiService == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetAPIOrServiceBySideProjectId), new { projectId = projectId, apiServiceId = createdApiService.Id }, createdApiService);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the side project API/service.");
            }
        }

        [HttpGet("/sideproject/{projectId}/apis-and-services")]
        public ActionResult GetAPIsAndServicesBySideProjectId(int projectId)
        {
            List<ApiService> apiServices = _apiServiceDao.GetAPIsAndServicesBySideProjectId(projectId);

            if (apiServices == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(apiServices);
            }
        }

        [HttpGet("/sideproject/{projectId}/api-or-service/{apiServiceId}")]
        public ActionResult<ApiService> GetAPIOrServiceBySideProjectId(int projectId, int apiServiceId)
        {
            ApiService apiService = _apiServiceDao.GetAPIOrServiceBySideProjectId(projectId, apiServiceId);

            if (apiService == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(apiService);
            }
        }
// TODO NEEDS apiServiceId as seen in URL
        [Authorize]
        [HttpPut("/update-sideproject/{projectId}/update-api-or-service/{apiServiceId}")]
        public ActionResult UpdateAPIOrServiceBySideProjectId(int projectId, ApiService apiService)
        {
            try
            {
                ApiService updatedApiService = _apiServiceDao.UpdateAPIOrServiceBySideProjectId(projectId, apiService);

                if (updatedApiService == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedApiService);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the side project API/service.");
            }
        }

        [Authorize]
        [HttpDelete("/sideproject/{projectId}/delete-api-or-service/{apiServiceId}")]
        public ActionResult DeleteAPIOrServiceBySideProjectId(int projectId, int apiServiceId)
        {
            try
            {
                int rowsAffected = _apiServiceDao.DeleteAPIOrServiceBySideProjectId(projectId, apiServiceId);

                if (rowsAffected > 0)
                {
                    return Ok("Side project API/service deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the side project API/service.");
            }
        }
    }
}
