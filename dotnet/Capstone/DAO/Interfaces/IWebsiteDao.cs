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
                                            PORTFOLIO WEBSITE CRUD
            **********************************************************************************************
        */
        // TODO Portfolio Website Controllers****

        /*  
            **********************************************************************************************
                                            SIDE PROJECT WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteBySideProjectId(int projectId, Website website);
        Website GetWebsiteBySideProjectId(int projectId, int websiteId);
        Website UpdateWebsiteBySideProjectId(int projectId, int websiteId, Website website);
        int DeleteWebsiteBySideProjectId(int projectId, int websiteId);

        /*  
            **********************************************************************************************
                                            CONTRIBUTOR WEBSITE CRUD
            **********************************************************************************************
        */
        Website CreateWebsiteByContributorId(int controllerId, Website website);
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

        /*  
            **********************************************************************************************
                                            EXPERIENCE WEBSITE CRUD
            **********************************************************************************************
        */
// TODO WEBSITE Experience Controllers****
        Website CreateWebsiteByExperienceId(int experienceId, Website website);
        Website GetWebsiteByExperienceId(int experienceId);
        Website UpdateWebsiteByExperienceId(int experienceId, int websiteId, Website website);
        int DeleteWebsiteByExperienceId(int experienceId, int websiteId);

        /*  
            **********************************************************************************************
                                            CREDENTIAL WEBSITE CRUD
            **********************************************************************************************
        */
// TODO WEBSITE Credential Controllers****
        Website CreateWebsiteByCredentialId(int credentialId, Website website);
        Website GetWebsiteByCredentialId(int credentialId, int websiteId);
        Website UpdateWebsiteByCredentialId(int credentialId, int websiteId, Website website);
        int DeleteWebsiteByCredentialId(int credentialId, int websiteId);
        /*  
            **********************************************************************************************
                                            EDUCATION WEBSITE CRUD
            **********************************************************************************************
        */
// TODO WEBSITE Education Controllers****
        Website CreateWebsiteByEducationId(int educationId, Website website);
        Website GetWebsiteByEducationId(int educationId);
        Website UpdateWebsiteByEducationId(int educationId, int websiteId, Website website);
        int DeleteWebsiteByEducationId(int educationId, int websiteId);

        /*  
            **********************************************************************************************
                                        OPEN SOURCE CONTRIBUTION WEBSITE CRUD
            **********************************************************************************************
        */
// TODO WEBSITE OpenSourceContribution Controllers****
        Website CreateWebsiteByOpenSourceContributionId(int contributionId, Website website);
        Website GetWebsiteByOpenSourceContributionId(int contributionId, int websiteId);
        Website UpdateWebsiteByOpenSourceContributionId(int contributionId, int websiteId, Website website);
        int DeleteWebsiteByOpenSourceContributionId(int contributionId, int websiteId);
        /*  
            **********************************************************************************************
                                            VOLUNTEER WORK WEBSITE CRUD
            **********************************************************************************************
        */
// TODO WEBSITE VolunteerWork Controllers****
        Website CreateWebsiteByVolunteerWorkId(int volunteerWorkId, Website website);
        Website GetWebsiteByVolunteerWorkId(int volunteerWorkId);
        Website UpdateWebsiteByVolunteerWorkId(int volunteerWorkId, int websiteId, Website website);
        int DeleteWebsiteByVolunteerWorkId(int volunteerWorkId, int websiteId);

    }
}