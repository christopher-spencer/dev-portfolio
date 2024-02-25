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
        Website UpdateWebsiteByWebsiteId(int websiteId, Website website);
        Website UpdateWebsite(Website websiteLink);
        int DeleteWebsiteById(int websiteLinkId);


        /*  
            **********************************************************************************************
                                            SIDE PROJECT WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByProjectId(int projectId, Website website);

        Website GetWebsiteByProjectId(int projectId);
        Website GetWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId);    
        Website UpdateWebsiteByProjectId(int projectId, Website updatedWebsite);
        int DeleteWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId);

        
        /*  
            **********************************************************************************************
                                           EVENTUAL BLOG POST WEBSITE CRUD
            **********************************************************************************************
        */

    }
}