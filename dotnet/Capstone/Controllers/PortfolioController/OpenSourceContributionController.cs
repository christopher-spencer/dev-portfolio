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
    public class OpenSourceContributionController : ControllerBase
    {
        private readonly IOpenSourceContributionDao _openSourceContributionDao;

        public OpenSourceContributionController(IOpenSourceContributionDao openSourceContributionDao)
        {
            _openSourceContributionDao = openSourceContributionDao;
        }

        /*  
            **********************************************************************************************
                                    OPEN SOURCE CONTRIBUTION CRUD CONTROLLER
            **********************************************************************************************
        */

        /*  
            **********************************************************************************************
                                PORTFOLIO OPEN SOURCE CONTRIBUTION CRUD CONTROLLER
            **********************************************************************************************
        */

    }
}