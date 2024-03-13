using Capstone.Models;
using Npgsql;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IWebsiteDao
    {

        /*  
            **********************************************************************************************
                                                    WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteLink(Website websiteLink);
        Website GetWebsiteById(int websiteLinkId);
        List<Website> GetAllWebsites();
        Website UpdateWebsite(Website website, int websiteId);
        int DeleteWebsiteById(int websiteLinkId);

        /*  
            **********************************************************************************************
                                            SIDE PROJECT WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByProjectId(int projectId, Website website);
        Website GetWebsiteByProjectId(int projectId);
        Website GetWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId);    
        Website UpdateWebsiteByProjectId(int projectId, int websiteId, Website website);
        int DeleteWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId);

        /*  
            **********************************************************************************************
                                            CONTRIBUTOR WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByContributorId(int controllerId, Website website);
        Website GetWebsiteByContributorId(int contributorId);
        Website UpdateWebsiteByContributorId(int contributorId, int websiteId, Website website);
        int DeleteWebsiteByContributorId(int contributorId, int websiteId);

        /*  
            **********************************************************************************************
                                         API AND SERVICE WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByApiServiceId(int apiServiceId, Website website);
        Website GetWebsiteByApiServiceId(int apiServiceId);
        Website UpdateWebsiteByApiServiceId(int apiServiceId, Website website);
        int DeleteWebsiteByApiServiceId(int apiServiceId, int websiteId);

        /*  
            **********************************************************************************************
                                      DEPENDENCY AND LIBRARY WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByDependencyLibraryId(int dependencyLibraryId, Website website);
        Website GetWebsiteByDependencyLibraryId(int dependencyLibraryId);
        Website UpdateWebsiteByDependencyLibraryId(int dependencyLibraryId, Website website);
        int DeleteWebsiteByDependencyLibraryId(int dependencyLibraryId, int websiteId);


    }
}