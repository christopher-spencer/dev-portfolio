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
    public class WebsiteController : ControllerBase
    {
        private readonly IWebsiteDao _websiteDao;

        public WebsiteController(IWebsiteDao websiteDao) 
        {
            _websiteDao = websiteDao;
        }

        /*  
            **********************************************************************************************
                                                WEBSITE CRUD CONTROLLER
            **********************************************************************************************
        */


    }
}