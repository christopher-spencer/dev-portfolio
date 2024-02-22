using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IWebsiteDao
    {
        Website CreateWebsiteLink(Website websiteLink);
        Website GetWebsiteByProjectIdAndWebsiteId(int projectId, int websiteId);
        Website GetWebsiteLinkById(int websiteLinkId);
        List<Website> GetAllWebsiteLinks();
        Website UpdateWebsiteLink(Website websiteLink);
        int DeleteWebsiteLinkById(int websiteLinkId);
    }
}