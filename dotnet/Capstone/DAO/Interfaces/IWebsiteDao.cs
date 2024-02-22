using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IWebsiteDao
    {
        Website CreateWebsiteByProjectId(int projectId, Website website);
        Website CreateWebsiteLink(Website websiteLink);
        Website GetWebsiteByProjectId(int projectId);
        Website GetWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId);
        Website GetWebsiteLinkById(int websiteLinkId);
        List<Website> GetAllWebsiteLinks();
        Website UpdateWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId, Website website);
        Website UpdateWebsiteLink(Website websiteLink);
        int DeleteWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId);
        int DeleteWebsiteLinkById(int websiteLinkId);
    }
}