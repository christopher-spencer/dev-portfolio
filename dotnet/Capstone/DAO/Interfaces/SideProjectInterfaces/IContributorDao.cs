using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IContributorDao
    {
        Contributor CreateContributor(Contributor contributor);
        List<Contributor> GetContributorsByProjectId(int projectId);
        Contributor GetContributorById(int contributorId);
        List<Contributor> GetAllContributors();
        Contributor UpdateContributor(Contributor contributor);
        int DeleteContributorById(int contributorId);

    }
}