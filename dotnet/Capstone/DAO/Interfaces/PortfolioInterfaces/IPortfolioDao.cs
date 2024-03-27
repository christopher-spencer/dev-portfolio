using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IPortfolioDao
    {

        /*  
            **********************************************************************************************
                                            PORTFOLIO CRUD
            **********************************************************************************************
        */

        Portfolio CreatePortfolio(Portfolio portfolio);
        List<Portfolio> GetPortfolios();
        Portfolio GetPortfolio(int portfolioId);
        Portfolio UpdatePortfolio(Portfolio portfolio, int portfolioId);
        int DeletePortfolio(int portfolioId);

    }
}