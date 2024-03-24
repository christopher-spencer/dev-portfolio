using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class AchievementPostgresDao : IAchievementDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;

        public AchievementPostgresDao(string dbConnectionString, IImageDao imageDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
        }

        /*  
            **********************************************************************************************
                                            ACHIEVEMENT CRUD
            **********************************************************************************************
        */
    }
}