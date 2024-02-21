using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IDependencyLibraryDao
    {
        DependencyLibrary CreateDependencyLibrary(DependencyLibrary dependencyLibrary);
        DependencyLibrary GetDependencyLibraryById(int dependencyLibraryId);
        List<DependencyLibrary> GetAllDependencyLibraries();
        DependencyLibrary UpdateDependencyLibrary(DependencyLibrary dependencyLibrary);
        int DeleteDependencyLibraryById(int dependencyLibraryId);
    }
}
