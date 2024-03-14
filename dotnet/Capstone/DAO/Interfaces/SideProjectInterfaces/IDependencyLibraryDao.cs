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
        DependencyLibrary GetDependencyOrLibrary(int dependencyLibraryId);
        List<DependencyLibrary> GetDependenciesAndLibraries();
        DependencyLibrary UpdateDependencyOrLibrary(int dependencyLibraryId, DependencyLibrary dependencyLibrary);
        int DeleteDependencyOrLibrary(int dependencyLibraryId);

        /*  
            **********************************************************************************************
                                    SIDE PROJECT DEPENDENCIES AND LIBRARIES CRUD
            **********************************************************************************************
        */ 
        DependencyLibrary CreateDependencyOrLibraryBySideProjectId(int projectId, DependencyLibrary dependencyLibrary);
        List<DependencyLibrary> GetDependenciesAndLibrariesBySideProjectId(int projectId);
        DependencyLibrary GetDependencyOrLibraryBySideProjectId(int projectId, int dependencyLibraryId);
        DependencyLibrary UpdateDependencyOrLibraryBySideProjectId(int projectId, int dependencyLibraryId, DependencyLibrary dependencyLibrary);
        int DeleteDependencyOrLibraryBySideProjectId(int projectId, int dependencyLibraryId);

    }
}
