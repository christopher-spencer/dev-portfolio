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
        Website GetWebsite(int websiteLinkId);
        List<Website> GetWebsites();

        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                            PORTFOLIO WEBSITE CRUD
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */
        Website CreateWebsiteByPortfolioId(int portfolioId, Website website);
        Website GetWebsiteByPortfolioId(int portfolioId, int websiteId);
        Website GetGitHubByPortfolioId(int portfolioId);
        Website GetLinkedInByPortfolioId(int portfolioId);
        Website UpdateWebsiteByPortfolioId(int portfolioId, int websiteId, Website website);
        int DeleteWebsiteByPortfolioId(int portfolioId, int websiteId);

        /*  
            **********************************************************************************************
                                            WORK EXPERIENCE WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByWorkExperienceId(int experienceId, Website website);
        Website GetWebsiteByWorkExperienceId(int experienceId);
        Website UpdateWebsiteByWorkExperienceId(int experienceId, int websiteId, Website website);
        int DeleteWebsiteByWorkExperienceId(int experienceId, int websiteId);

        /*  
            **********************************************************************************************
                                            CREDENTIAL WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByCredentialId(int credentialId, Website website);
        Website GetWebsiteByCredentialId(int credentialId, int websiteId);
        Website GetOrganizationWebsiteByCredentialId(int credentialId);
        Website GetCredentialWebsiteByCredentialId(int credentialId);
        Website UpdateWebsiteByCredentialId(int credentialId, int websiteId, Website website);
        int DeleteWebsiteByCredentialId(int credentialId, int websiteId);

        /*  
            **********************************************************************************************
                                            EDUCATION WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByEducationId(int educationId, Website website);
        Website GetWebsiteByEducationId(int educationId);
        Website UpdateWebsiteByEducationId(int educationId, int websiteId, Website website);
        int DeleteWebsiteByEducationId(int educationId, int websiteId);

        /*  
            **********************************************************************************************
                                        OPEN SOURCE CONTRIBUTION WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByOpenSourceContributionId(int contributionId, Website website);
        Website GetWebsiteByOpenSourceContributionId(int contributionId, int websiteId);
        Website GetMainWebsiteOrGitHubByOpenSourceContributionId(int contributionId, string websiteType);
        List<Website> GetAllWebsitesByOpenSourceContributionId(int contributionId);
        List<Website> GetPullRequestLinksByOpenSourceContributionId(int contributionId);
        Website UpdateWebsiteByOpenSourceContributionId(int contributionId, int websiteId, Website website);
        Website UpdateMainWebsiteOrGitHubByOpenSourceContributionId(int contributionId, int websiteId, Website website);
        int DeleteWebsiteByOpenSourceContributionId(int contributionId, int websiteId);

        /*  
            **********************************************************************************************
                                            VOLUNTEER WORK WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByVolunteerWorkId(int volunteerWorkId, Website website);
        Website GetWebsiteByVolunteerWorkId(int volunteerWorkId);
        Website UpdateWebsiteByVolunteerWorkId(int volunteerWorkId, int websiteId, Website website);
        int DeleteWebsiteByVolunteerWorkId(int volunteerWorkId, int websiteId);

        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                            SIDE PROJECT WEBSITE CRUD
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */
        Website CreateWebsiteBySideProjectId(int projectId, Website website);
        Website GetWebsiteBySideProjectId(int projectId, int websiteId);
        Website GetMainWebsiteBySideProjectId(int sideProjectId);
        Website GetGitHubBySideProjectId(int sideProjectId);
        Website UpdateWebsiteBySideProjectId(int projectId, int websiteId, Website website);
        int DeleteWebsiteBySideProjectId(int projectId, int websiteId);

        /*  
            **********************************************************************************************
                                            CONTRIBUTOR WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByContributorId(int controllerId, Website website);
        Website GetWebsiteByContributorId(int contributorId, int websiteId);
        Website GetPortfolioLinkByContributorId(int contributorId);
        Website GetGitHubByContributorId(int contributorId);
        Website GetLinkedInByContributorId(int contributorId);
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