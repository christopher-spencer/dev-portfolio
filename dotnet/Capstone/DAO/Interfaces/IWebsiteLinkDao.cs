using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IWebsiteLinkDao
    {
        WebsiteLink CreateWebsiteLink(WebsiteLink websiteLink);
        WebsiteLink GetWebsiteLinkById(int websiteLinkId);
        List<WebsiteLink> GetAllWebsiteLinks();
        WebsiteLink UpdateWebsiteLink(WebsiteLink websiteLink);
        int DeleteWebsiteLinkById(int websiteLinkId);
    }
}