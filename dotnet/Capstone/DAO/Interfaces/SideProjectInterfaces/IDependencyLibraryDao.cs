using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IDependencyLibraryDao
    {
        DependencyLibrary CreateDependencyOrLibraryByProjectId(int projectId, DependencyLibrary dependencyLibrary);
        DependencyLibrary CreateDependencyOrLibrary(DependencyLibrary dependencyLibrary);
        List<DependencyLibrary> GetDependenciesAndLibrariesByProjectId(int projectId);
        DependencyLibrary GetDependencyOrLibraryByProjectId(int projectId, int dependencyLibraryId);
        DependencyLibrary GetDependencyOrLibraryById(int dependencyLibraryId);
        List<DependencyLibrary> GetAllDependenciesAndLibraries();
        DependencyLibrary UpdateDependencyOrLibraryByProjectId(int projectId, DependencyLibrary updatedDependencyLibrary);
        DependencyLibrary UpdateDependencyOrLibrary(DependencyLibrary dependencyLibrary);
        int DeleteDependencyOrLibraryByProjectId(int projectId, int dependencyLibraryId);
        int DeleteDependencyOrLibraryById(int dependencyLibraryId);
    }
}
