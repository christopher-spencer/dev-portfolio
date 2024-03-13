using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IApiServiceDao
    {
        /*  
            **********************************************************************************************
                                        APIS AND SERVICES CRUD
            **********************************************************************************************
        */ 
        ApiService CreateAPIOrService(ApiService apiService);
        ApiService GetAPIOrServiceById(int apiServiceId);
        List<ApiService> GetAllAPIsAndServices();
        ApiService UpdateAPIOrService(int apiServiceId, ApiService apiService);
        int DeleteAPIOrService(int apiServiceId);

        /*  
            **********************************************************************************************
                                     SIDE PROJECT APIS AND SERVICES CRUD
            **********************************************************************************************
        */ 
        ApiService CreateAPIOrServiceBySideProjectId(int projectId, ApiService apiService);
        List<ApiService> GetAPIsAndServicesBySideProjectId(int projectId);
        ApiService GetAPIOrServiceBySideProjectId(int projectId, int apiServiceId);
        ApiService UpdateAPIOrServiceBySideProjectId(int projectId, ApiService apiService);
        int DeleteAPIOrServiceBySideProjectId(int projectId, int apiServiceId);
    }
}
