using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class OpenSourceContribution
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string OrganizationName { get; set; }
        public Image OrganizationLogoUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectDescription { get; set; }
        public string ContributionDetails { get; set; }
        public string WebsiteUrl { get; set; }
        public string RepoUrl { get; set; }
        public List<Skill> TechSkillsUtilized { get; set; }
        public List<string> PullRequestsLinks { get; set; }
        public List<string> ReviewCommentsAndFeedbackReceived { get; set; }
        public Image MainImageUrl { get; set; }
        public List<Image> AdditionalImagesUrl { get; set; }

    }
}