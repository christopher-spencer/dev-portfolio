using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IEducationDao
    {
        /*  
            **********************************************************************************************
                                                EDUCATION CRUD
            **********************************************************************************************
        */

        List<Education> GetEducations();
        Education GetEducation(int educationId);

        /*  
            **********************************************************************************************
                                            PORTFOLIO EDUCATION CRUD
            **********************************************************************************************
        */
        Education CreateEducationByPortfolioId(int portfolioId, Education education);
        List<Education> GetEducationsByPortfolioId(int portfolioId);
        Education GetEducationByPortfolioId(int portfolioId, int educationId);
        Education UpdateEducationByPortfolioId(int portfolioId, int educationId, Education education);
        int DeleteEducationByPortfolioId(int portfolioId, int educationId);
    }
}