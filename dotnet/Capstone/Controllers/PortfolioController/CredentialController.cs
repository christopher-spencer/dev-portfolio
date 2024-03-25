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
    public class CredentialController : ControllerBase
    {
        private readonly ICredentialDao _credentialDao;

        public CredentialController(ICredentialDao credentialDao)
        {
            _credentialDao = credentialDao;
        }

        /*  
            **********************************************************************************************
                                            CREDENTIAL CRUD CONTROLLER
            **********************************************************************************************
        */

        [HttpGet("/credentials")]
        public ActionResult<List<Credential>> GetCredentials()
        {
            List<Credential> credentials = _credentialDao.GetCredentials();

            if (credentials == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(credentials);
            }
        }

        [HttpGet("/credential/{credentialId}")]
        public ActionResult<Credential> GetCredential(int credentialId)
        {
            Credential credential = _credentialDao.GetCredential(credentialId);

            if (credential == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(credential);
            }
        }

        /*  
            **********************************************************************************************
                                        PORTFOLIO CREDENTIAL CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/portfolio/{portfolioId}/create-credential")]
        public ActionResult<Credential> CreateCredentialByPortfolioId(int portfolioId, Credential credential)
        {
            try
            {
                Credential createdCredential = _credentialDao.CreateCredentialByPortfolioId(portfolioId, credential);

                if (createdCredential == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetCredentialByPortfolioId), new { portfolioId = portfolioId, credentialId = createdCredential.Id }, createdCredential);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the credential.");
            }
        }

        [HttpGet("/portfolio/{portfolioId}/credentials")]
        public ActionResult<List<Credential>> GetCredentialsByPortfolioId(int portfolioId)
        {
            List<Credential> credentials = _credentialDao.GetCredentialsByPortfolioId(portfolioId);

            if (credentials == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(credentials);
            }
        }

        [HttpGet("/portfolio/{portfolioId}/credential/{credentialId}")]
        public ActionResult<Credential> GetCredentialByPortfolioId(int portfolioId, int credentialId)
        {
            Credential credential = _credentialDao.GetCredentialByPortfolioId(portfolioId, credentialId);

            if (credential == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(credential);
            }
        }

        [Authorize]
        [HttpPut("/portfolio/{portfolioId}/update-credential/{credentialId}")]
        public ActionResult<Credential> UpdateCredentialByPortfolioId(int portfolioId, int credentialId, Credential credential)
        {
            try
            {
                Credential updatedCredential = _credentialDao.UpdateCredentialByPortfolioId(portfolioId, credentialId, credential);

                if (updatedCredential == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedCredential);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the credential.");
            }
        }

        [Authorize]
        [HttpDelete("/portfolio/{portfolioId}/delete-credential/{credentialId}")]
        public ActionResult DeleteCredentialByPortfolioId(int portfolioId, int credentialId)
        {
            try
            {
                int result = _credentialDao.DeleteCredentialByPortfolioId(portfolioId, credentialId);

                if (result == 0)
                {
                    return BadRequest();
                }
                else
                {
                    return NoContent();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the credential.");
            }
        }
    }
}