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
// TODO WORK EXPERIENCE Controllers****
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
// TODO create PORTFOLIO Work Experience Controllers      
        WorkExperience CreateWorkExperienceByPortfolioId(int portfolioId, WorkExperience experience);
        List<WorkExperience> GetWorkExperiencesByPortfolioId(int portfolioId);
        WorkExperience GetWorkExperienceByPortfolioId(int portfolioId, int experienceId);
        WorkExperience UpdateWorkExperienceByPortfolioId(int portfolioId, int experienceId, WorkExperience experience);
        int DeleteWorkExperienceByPortfolioId(int portfolioId, int experienceId);
    }
}