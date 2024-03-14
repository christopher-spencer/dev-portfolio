using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IContributorDao
    {
        /*  
            **********************************************************************************************
                                                CONTRIBUTOR CRUD
            **********************************************************************************************
        */ 
        Contributor CreateContributor(Contributor contributor);
        Contributor GetContributor(int contributorId);
        List<Contributor> GetContributors();
        Contributor UpdateContributor(int contributorId, Contributor contributor);
        int DeleteContributor(int contributorId);

        /*  
            **********************************************************************************************
                                          SIDE PROJECT CONTRIBUTOR CRUD
            **********************************************************************************************
        */         
        Contributor CreateContributorBySideProjectId(int projectId, Contributor contributor);
        List<Contributor> GetContributorsBySideProjectId(int projectId);
        Contributor GetContributorBySideProjectId(int projectId, int contributorId);
        Contributor UpdateContributorBySideProjectId(int projectId, int contributorId, Contributor contributor);
        int DeleteContributorBySideProjectId(int projectId, int contributorId);

    }
}