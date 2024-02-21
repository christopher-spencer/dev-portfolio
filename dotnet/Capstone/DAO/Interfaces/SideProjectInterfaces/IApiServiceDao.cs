using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IApiServiceDao
    {
        ApiService CreateApiService(ApiService apiService);
        List<ApiService> GetAPIsAndServicesByProjectId(int projectId);
        ApiService GetApiServiceById(int apiServiceId);
        List<ApiService> GetAllApiServices();
        ApiService UpdateApiService(ApiService apiService);
        int DeleteApiServiceById(int apiServiceId);
    }
}
