using System;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class PortfolioPostgresDao : IPortfolioDao
    {
        private readonly string connectionString;
        private readonly ISideProjectDao _sideProjectDao;
        private readonly IWebsiteDao _websiteDao;
        private readonly IImageDao _imageDao;
        private readonly ISkillDao _skillDao;

        public PortfolioPostgresDao(string dbConnectionString, ISideProjectDao sideProjectDao,
            IWebsiteDao websiteDao, IImageDao imageDao, ISkillDao skillDao) {
            connectionString = dbConnectionString;
            this._sideProjectDao = sideProjectDao;
            this._websiteDao = websiteDao;
            this._imageDao = imageDao;
            this._skillDao = skillDao;
        }
// TODO will need to model after SP and BP Daos
// TODO use SIDEPROJECT POSTGRES DAO CRUD AS TEMPLATE FOR IMPROVING CRUD METHODS
        /*  
            **********************************************************************************************
                                            PORTFOLIO CRUD
            **********************************************************************************************
        */

        /*  
            **********************************************************************************************
                                        PORTFOLIO HELPER METHODS
            **********************************************************************************************
        */

        /*  
            **********************************************************************************************
                                            PORTFOLIO MAP ROW
            **********************************************************************************************
        */

        private Portfolio MapRowToPortfolio(NpgsqlDataReader reader)
        {
            Portfolio portfolio = new Portfolio
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Location = Convert.ToString(reader["location"]),
                ProfessionalSummary = Convert.ToString(reader["professional_summary"]),
                Email = Convert.ToString(reader["email"])
            };

            return portfolio;
        }
    }
}