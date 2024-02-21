using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IDependencyLibraryDao
    {
        DependencyLibrary CreateDependencyLibrary(DependencyLibrary dependencyLibrary);
        List<DependencyLibrary> GetDependenciesAndLibrariesByProjectId(int projectId);
        DependencyLibrary GetDependencyLibraryById(int dependencyLibraryId);
        List<DependencyLibrary> GetAllDependencyLibraries();
        DependencyLibrary UpdateDependencyLibrary(DependencyLibrary dependencyLibrary);
        int DeleteDependencyLibraryById(int dependencyLibraryId);
    }
}
