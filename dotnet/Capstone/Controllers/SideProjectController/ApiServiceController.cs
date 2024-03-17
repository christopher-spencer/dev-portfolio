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
// TODO TEST ALL POSTMAN CRUD

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
                    return CreatedAtAction(nameof(GetAPIOrService), new { apiServiceId = createdApiService.Id }, createdApiService);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the API/service.");
            }
        }

        [HttpGet("/api-or-service/{apiServiceId}")]
        public ActionResult<ApiService> GetAPIOrService(int apiServiceId)
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

        [HttpGet("/apis-and-services")]
        public ActionResult<List<ApiService>> GetAPIsAndServices()
        {
            List<ApiService> apiServices = _apiServiceDao.GetAPIsAndServices();

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
        [HttpPost("/sideproject/{sideProjectId}/create-api-or-service")]
        public ActionResult CreateAPIOrServiceBySideProjectId(int sideProjectId, ApiService apiService)
        {
            try
            {
                ApiService createdApiService = _apiServiceDao.CreateAPIOrServiceBySideProjectId(sideProjectId, apiService);

                if (createdApiService == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetAPIOrServiceBySideProjectId), new { sideProjectId, apiServiceId = createdApiService.Id }, createdApiService);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the side project API/service.");
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/apis-and-services")]
        public ActionResult GetAPIsAndServicesBySideProjectId(int sideProjectId)
        {
            List<ApiService> apiServices = _apiServiceDao.GetAPIsAndServicesBySideProjectId(sideProjectId);

            if (apiServices == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(apiServices);
            }
        }

        [HttpGet("/sideproject/{sideProjectId}/api-or-service/{apiServiceId}")]
        public ActionResult<ApiService> GetAPIOrServiceBySideProjectId(int sideProjectId, int apiServiceId)
        {
            ApiService apiService = _apiServiceDao.GetAPIOrServiceBySideProjectId(sideProjectId, apiServiceId);

            if (apiService == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(apiService);
            }
        }

        [Authorize]
        [HttpPut("/update-sideproject/{sideProjectId}/update-api-or-service/{apiServiceId}")]
        public ActionResult UpdateAPIOrServiceBySideProjectId(int sideProjectId, int apiServiceId, ApiService apiService)
        {
            try
            {
                ApiService updatedApiService = _apiServiceDao.UpdateAPIOrServiceBySideProjectId(sideProjectId, apiServiceId, apiService);

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
        [HttpDelete("/sideproject/{sideProjectId}/delete-api-or-service/{apiServiceId}")]
        public ActionResult DeleteAPIOrServiceBySideProjectId(int sideProjectId, int apiServiceId)
        {
            try
            {
                int rowsAffected = _apiServiceDao.DeleteAPIOrServiceBySideProjectId(sideProjectId, apiServiceId);

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
