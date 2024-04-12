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

        ApiService GetAPIOrServiceById(int apiServiceId);
        List<ApiService> GetAPIsAndServices();

        /*  
            **********************************************************************************************
                                     SIDE PROJECT APIS AND SERVICES CRUD
            **********************************************************************************************
        */ 
        ApiService CreateAPIOrServiceBySideProjectId(int projectId, ApiService apiService);
        List<ApiService> GetAPIsAndServicesBySideProjectId(int projectId);
        ApiService GetAPIOrServiceBySideProjectId(int projectId, int apiServiceId);
        ApiService UpdateAPIOrServiceBySideProjectId(int projectId, int apiServiceId, ApiService apiService);
        int DeleteAPIOrServiceBySideProjectId(int projectId, int apiServiceId);
    }
}
