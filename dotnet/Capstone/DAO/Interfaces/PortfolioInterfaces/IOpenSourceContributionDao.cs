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
// TODO OPEN SOURCE CONTRIBUTION Controllers****
        OpenSourceContribution CreateOpenSourceContribution(OpenSourceContribution contribution);
        List<OpenSourceContribution> GetOpenSourceContributions();
        OpenSourceContribution GetOpenSourceContribution(int contributionId);
        OpenSourceContribution UpdateOpenSourceContribution(int contributionId, OpenSourceContribution contribution);
        int DeleteOpenSourceContribution(int contributionId);

        /*  
            **********************************************************************************************
                                        PORTFOLIO OPEN SOURCE CONTRIBUTION CRUD
            **********************************************************************************************
        */
// TODO create PORTFOLIO Open Source Contribution Controllers
        OpenSourceContribution CreateOpenSourceContributionByPortfolioId(int portfolioId, OpenSourceContribution contribution);
        List<OpenSourceContribution> GetOpenSourceContributionsByPortfolioId(int portfolioId);
        OpenSourceContribution GetOpenSourceContributionByPortfolioId(int portfolioId, int contributionId);
        OpenSourceContribution UpdateOpenSourceContributionByPortfolioId(int portfolioId, int contributionId, OpenSourceContribution contribution);
        int DeleteOpenSourceContributionByPortfolioId(int portfolioId, int contributionId);
    }
}