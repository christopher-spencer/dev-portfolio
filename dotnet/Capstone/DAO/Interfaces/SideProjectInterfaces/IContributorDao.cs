using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IContributorDao
    {
        Contributor CreateContributorByProjectId(int projectId, Contributor contributor);
        Contributor CreateContributor(Contributor contributor);
        List<Contributor> GetContributorsByProjectId(int projectId);
        Contributor GetContributorByProjectId(int projectId, int contributorId);
        Contributor GetContributorById(int contributorId);
        List<Contributor> GetAllContributors();
        Contributor UpdateContributorByProjectId(int projectId, Contributor updatedContributor);
        Contributor UpdateContributor(Contributor contributor);
        int DeleteContributorByProjectId(int projectId, int contributorId);
        int DeleteContributorById(int contributorId);

    }
}