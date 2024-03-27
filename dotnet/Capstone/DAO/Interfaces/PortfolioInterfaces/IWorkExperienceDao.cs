using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IWorkExperienceDao
    {
        /*  
            **********************************************************************************************
                                              WORK EXPERIENCE CRUD
            **********************************************************************************************
        */
        WorkExperience CreateWorkExperience(WorkExperience experience);
        List<WorkExperience> GetWorkExperiences();
        WorkExperience GetWorkExperience(int experienceId);
        WorkExperience UpdateWorkExperience(int experienceId, WorkExperience experience);
        int DeleteWorkExperience(int experienceId);

        /*  
            **********************************************************************************************
                                           PORTFOLIO WORK EXPERIENCE CRUD
            **********************************************************************************************
        */
        WorkExperience CreateWorkExperienceByPortfolioId(int portfolioId, WorkExperience experience);
        List<WorkExperience> GetWorkExperiencesByPortfolioId(int portfolioId);
        WorkExperience GetWorkExperienceByPortfolioId(int portfolioId, int experienceId);
        WorkExperience UpdateWorkExperienceByPortfolioId(int portfolioId, int experienceId, WorkExperience experience);
        int DeleteWorkExperienceByPortfolioId(int portfolioId, int experienceId);
    }
}