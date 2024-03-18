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
        Website CreateWebsite(Website websiteLink);
        Website GetWebsite(int websiteLinkId);
        List<Website> GetWebsites();
        Website UpdateWebsite(Website website, int websiteId);
        int DeleteWebsite(int websiteLinkId);

        /*  
            **********************************************************************************************
                                            SIDE PROJECT WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteBySideProjectId(int projectId, Website website, string websiteType);
        Website GetWebsiteBySideProjectId(int projectId, int websiteId);
        Website UpdateWebsiteBySideProjectId(int projectId, int websiteId, Website website);
        int DeleteWebsiteBySideProjectId(int projectId, int websiteId, string websiteType);

        /*  
            **********************************************************************************************
                                            CONTRIBUTOR WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByContributorId(int controllerId, Website website, string websiteType);
        Website GetWebsiteByContributorId(int contributorId, int websiteId);
        Website UpdateWebsiteByContributorId(int contributorId, int websiteId, Website website);
        int DeleteWebsiteByContributorId(int contributorId, int websiteId);

        /*  
            **********************************************************************************************
                                         API AND SERVICE WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByApiServiceId(int apiServiceId, Website website);
        Website GetWebsiteByApiServiceId(int apiServiceId);
        Website UpdateWebsiteByApiServiceId(int apiServiceId, int websiteId, Website website);
        int DeleteWebsiteByApiServiceId(int apiServiceId, int websiteId);

        /*  
            **********************************************************************************************
                                      DEPENDENCY AND LIBRARY WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByDependencyLibraryId(int dependencyLibraryId, Website website);
        Website GetWebsiteByDependencyLibraryId(int dependencyLibraryId, int websiteId);
        Website UpdateWebsiteByDependencyLibraryId(int dependencyLibraryId, int websiteId, Website website);
        int DeleteWebsiteByDependencyLibraryId(int dependencyLibraryId, int websiteId);


    }
}