using Capstone.Models;
using Npgsql;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IWebsiteDao
    {
        Website CreateWebsiteByProjectId(int projectId, Website website);
        Website CreateWebsiteLink(Website websiteLink);

        Website GetWebsiteByProjectId(int projectId);
        Website GetWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId);
        
        Website GetWebsiteById(int websiteLinkId);
        List<Website> GetAllWebsites();
        Website UpdateWebsiteByProjectId(int projectId, Website updatedWebsite);
        Website UpdateWebsiteByWebsiteId(int websiteId, Website website);
        Website UpdateWebsite(Website websiteLink);
        int DeleteWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId);
        int DeleteWebsiteById(int websiteLinkId);

    }
}