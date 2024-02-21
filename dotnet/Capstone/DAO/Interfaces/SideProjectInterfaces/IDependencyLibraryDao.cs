using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IDependencyLibraryDao
    {
        DependencyLibrary CreateDependencyOrLibrary(DependencyLibrary dependencyLibrary);
        List<DependencyLibrary> GetDependenciesAndLibrariesByProjectId(int projectId);
        DependencyLibrary GetDependencyOrLibraryById(int dependencyLibraryId);
        List<DependencyLibrary> GetAllDependenciesAndLibraries();
        DependencyLibrary UpdateDependencyOrLibrary(DependencyLibrary dependencyLibrary);
        int DeleteDependencyOrLibraryById(int dependencyLibraryId);
    }
}
