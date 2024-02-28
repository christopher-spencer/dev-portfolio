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
        Contributor GetContributorById(int contributorId);
        List<Contributor> GetAllContributors();
        Contributor UpdateContributor(Contributor contributor);
        int DeleteContributorById(int contributorId);

        /*  
            **********************************************************************************************
                                          SIDE PROJECT CONTRIBUTOR CRUD
            **********************************************************************************************
        */         
        Contributor CreateContributorBySideProjectId(int projectId, Contributor contributor);
        List<Contributor> GetContributorsBySideProjectId(int projectId);
        Contributor GetContributorBySideProjectId(int projectId, int contributorId);
        Contributor UpdateContributorBySideProjectId(int projectId, Contributor updatedContributor);
        int DeleteContributorBySideProjectId(int projectId, int contributorId);

    }
}