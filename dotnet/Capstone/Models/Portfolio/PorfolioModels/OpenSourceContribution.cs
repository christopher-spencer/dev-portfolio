using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class OpenSourceContribution
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string OrganizationName { get; set; }
        public int OrganizationLogoId { get; set; }
        public Image OrganizationLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectDescription { get; set; }
        public string ContributionDetails { get; set; }
        public int OrganizationWebsiteId { get; set; }
        public Website OrganizationWebsite { get; set; }
        public int OrganizationGitHubId { get; set; }
        public Website OrganizationGitHubRepo { get; set; }
        public List<Skill> TechSkillsUtilized { get; set; }
        public List<Website> PullRequestsLinks { get; set; }
        public List<Achievement> ReviewCommentsAndFeedbackReceived { get; set; }
        public int MainImageId { get; set; }
        public Image MainImage { get; set; }
        public List<Image> AdditionalImages { get; set; }

    }
}