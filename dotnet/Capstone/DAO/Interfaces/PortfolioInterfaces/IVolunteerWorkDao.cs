using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IVolunteerWorkDao
    {
        /*  
            **********************************************************************************************
                                            VOLUNTEER WORK CRUD
            **********************************************************************************************
        */

        List<VolunteerWork> GetVolunteerWorks();
        VolunteerWork GetVolunteerWork(int volunteerWorkId);

        /*  
            **********************************************************************************************
                                            PORTFOLIO VOLUNTEER WORK CRUD
            **********************************************************************************************
        */
        VolunteerWork CreateVolunteerWorkByPortfolioId(int portfolioId, VolunteerWork volunteerWork);
        List<VolunteerWork> GetVolunteerWorksByPortfolioId(int portfolioId);
        VolunteerWork GetVolunteerWorkByPortfolioId(int portfolioId, int volunteerWorkId);
        VolunteerWork UpdateVolunteerWorkByPortfolioId(int portfolioId, int volunteerWorkId, VolunteerWork volunteerWork);
        int DeleteVolunteerWorkByPortfolioId(int portfolioId, int volunteerWorkId);
    }
}