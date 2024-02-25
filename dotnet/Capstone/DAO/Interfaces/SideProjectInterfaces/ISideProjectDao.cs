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
        List<SideProject> GetSideProjects();
        SideProject GetSideProjectById(int sideProjectId);
        SideProject CreateSideProject(SideProject sideProject);
        SideProject UpdateSideProject(SideProject sideProject, int sideProjectId);
        int DeleteSideProjectBySideProjectId(int sideProjectId);
    }
}