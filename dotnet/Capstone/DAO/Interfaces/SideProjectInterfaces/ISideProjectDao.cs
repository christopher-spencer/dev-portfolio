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
    }
}