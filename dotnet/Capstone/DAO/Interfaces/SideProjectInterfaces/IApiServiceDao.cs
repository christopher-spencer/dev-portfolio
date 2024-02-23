using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IApiServiceDao
    {
        ApiService CreateAPIOrServiceByProjectId(int projectId, ApiService apiService);
        ApiService CreateAPIOrService(ApiService apiService);
        List<ApiService> GetAPIsAndServicesByProjectId(int projectId);
        ApiService GetAPIOrServiceByProjectId(int projectId, int apiServiceId);
        ApiService GetAPIOrServiceById(int apiServiceId);
        List<ApiService> GetAllAPIsAndServices();
        ApiService UpdateAPIOrServiceByProjectId(int projectId, ApiService updatedApiService);
        ApiService UpdateAPIOrService(ApiService apiService);
        int DeleteAPIOrService(int apiServiceId);
    }
}
