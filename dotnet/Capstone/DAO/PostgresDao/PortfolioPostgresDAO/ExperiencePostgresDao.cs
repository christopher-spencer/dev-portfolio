using System;
using System.Collections.Generic;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class ExperiencePostgresDao: IExperienceDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;
        private readonly ISkillDao _skillDao;
        private readonly IWebsiteDao _websiteDao;
        private readonly IAchievementDao _achievementDao;

        public ExperiencePostgresDao(string dbConnectionString, IImageDao imageDao, ISkillDao skillDao, IWebsiteDao websiteDao, IAchievementDao achievementDao)
        {
            connectionString = dbConnectionString;
            _imageDao = imageDao;
            _skillDao = skillDao;
            _websiteDao = websiteDao;
            _achievementDao = achievementDao;
        }

        /*  
            **********************************************************************************************
                                              EXPERIENCE CRUD
            **********************************************************************************************
        */
    }
}