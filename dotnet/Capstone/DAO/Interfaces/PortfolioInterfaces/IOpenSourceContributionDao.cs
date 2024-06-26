using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IOpenSourceContributionDao
    {
        /*  
            **********************************************************************************************
                                            OPEN SOURCE CONTRIBUTION CRUD
            **********************************************************************************************
        */

        List<OpenSourceContribution> GetOpenSourceContributions();
        OpenSourceContribution GetOpenSourceContribution(int contributionId);

        /*  
            **********************************************************************************************
                                        PORTFOLIO OPEN SOURCE CONTRIBUTION CRUD
            **********************************************************************************************
        */
        OpenSourceContribution CreateOpenSourceContributionByPortfolioId(int portfolioId, OpenSourceContribution contribution);
        List<OpenSourceContribution> GetOpenSourceContributionsByPortfolioId(int portfolioId);
        OpenSourceContribution GetOpenSourceContributionByPortfolioId(int portfolioId, int contributionId);
        OpenSourceContribution UpdateOpenSourceContributionByPortfolioId(int portfolioId, int contributionId, OpenSourceContribution contribution);
        int DeleteOpenSourceContributionByPortfolioId(int portfolioId, int contributionId);
    }
}