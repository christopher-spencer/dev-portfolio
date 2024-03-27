using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface ISideProjectDao
    {

        /*  
            **********************************************************************************************
                                                SIDE PROJECT CRUD
            **********************************************************************************************
        */
        SideProject CreateSideProject(SideProject sideProject);
        List<SideProject> GetSideProjects();
        SideProject GetSideProject(int sideProjectId);
        SideProject UpdateSideProject(SideProject sideProject, int sideProjectId);
        int DeleteSideProject(int sideProjectId);

        /*  
            **********************************************************************************************
                                            PORTFOLIO SIDE PROJECT CRUD
            **********************************************************************************************
        */
        SideProject CreateSideProjectByPortfolioId(int portfolioId, SideProject sideProject);
        List<SideProject> GetSideProjectsByPortfolioId(int portfolioId);
        SideProject GetSideProjectByPortfolioId(int portfolioId, int sideProjectId);
        SideProject UpdateSideProjectByPortfolioId(int portfolioId, int sideProjectId, SideProject sideProject);
        int DeleteSideProjectByPortfolioId(int portfolioId, int sideProjectId);
    }
}