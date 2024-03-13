using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IDependencyLibraryDao
    {
        /*  
            **********************************************************************************************
                                        DEPENDENCIES AND LIBRARIES CRUD
            **********************************************************************************************
        */    
        DependencyLibrary CreateDependencyOrLibrary(DependencyLibrary dependencyLibrary);
        DependencyLibrary GetDependencyOrLibraryById(int dependencyLibraryId);
        List<DependencyLibrary> GetAllDependenciesAndLibraries();
        DependencyLibrary UpdateDependencyOrLibrary(int dependencyLibraryId, DependencyLibrary dependencyLibrary);
        int DeleteDependencyOrLibraryById(int dependencyLibraryId);

        /*  
            **********************************************************************************************
                                    SIDE PROJECT DEPENDENCIES AND LIBRARIES CRUD
            **********************************************************************************************
        */ 
        DependencyLibrary CreateDependencyOrLibraryBySideProjectId(int projectId, DependencyLibrary dependencyLibrary);
        List<DependencyLibrary> GetDependenciesAndLibrariesBySideProjectId(int projectId);
        DependencyLibrary GetDependencyOrLibraryBySideProjectId(int projectId, int dependencyLibraryId);
        DependencyLibrary UpdateDependencyOrLibraryBySideProjectId(int projectId, DependencyLibrary dependencyLibrary);
        int DeleteDependencyOrLibraryBySideProjectId(int projectId, int dependencyLibraryId);

    }
}
