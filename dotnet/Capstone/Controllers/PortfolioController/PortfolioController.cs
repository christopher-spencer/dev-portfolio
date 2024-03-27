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
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioDao _portfolioDao;

        public PortfolioController(IPortfolioDao portfolioDao)
        {
            _portfolioDao = portfolioDao;
        }

        /*  
            **********************************************************************************************
                                            PORTFOLIO CRUD CONTROLLER
            **********************************************************************************************
        */

        [Authorize]
        [HttpPost("/create-portfolio")]
        public ActionResult CreatePortfolio(Portfolio portfolio)
        {
            try
            {
                Portfolio createdPortfolio = _portfolioDao.CreatePortfolio(portfolio);

                if (createdPortfolio == null)
                {
                    return BadRequest();
                }
                else
                {
                    return CreatedAtAction(nameof(GetPortfolio), new { portfolioId = createdPortfolio.Id }, createdPortfolio);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while creating the portfolio.");
            }
        }

        [HttpGet("/portfolios")]
        public ActionResult<List<Portfolio>> GetPortfolios()
        {
            List<Portfolio> portfolios = _portfolioDao.GetPortfolios();

            if (portfolios == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(portfolios);
            }
        }

        [HttpGet("/portfolio/{portfolioId}")]
        public ActionResult<Portfolio> GetPortfolio(int portfolioId)
        {
            Portfolio portfolio = _portfolioDao.GetPortfolio(portfolioId);

            if (portfolio == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(portfolio);
            }
        }

        [Authorize]
        [HttpPut("/update-portfolio/{portfolioId}")]
        public ActionResult UpdatePortfolio(Portfolio portfolio, int portfolioId)
        {
            try
            {
                Portfolio updatedPortfolio = _portfolioDao.UpdatePortfolio(portfolio, portfolioId);

                if (updatedPortfolio == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedPortfolio);
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while updating the portfolio.");
            }
        }

        [Authorize]
        [HttpDelete("/portfolio/delete/{portfolioId}")]
        public ActionResult<int> DeletePortfolio(int portfolioId)
        {
            try
            {
                int rowsAffected = _portfolioDao.DeletePortfolio(portfolioId);

                if (rowsAffected > 0)
                {
                    return Ok("Portfolio deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, "An error occurred while deleting the portfolio.");
            }
        }
    }
}