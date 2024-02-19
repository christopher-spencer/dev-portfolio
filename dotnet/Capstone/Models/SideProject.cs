using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class SideProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MainImageUrl { get; set; }
        public string Description { get; set; }
        public List<string> GoalsAndObjectives { get; set; }
        public List<string> AdditionalImagesUrl { get; set; }
        public string VideoWalkthroughUrl { get; set; }
        public List<TechSkill> ToolsUsed { get; set; }
        public string WebsiteLink { get; set; }
        public string GitHubRepoLink { get; set; }
        public List<string> Contributors { get; set; }
        public List<string> ExternalAPIsAndServicesUsed { get; set; }
        public List<string> DependenciesOrLibrariesUsed { get; set; }
        public string ProjectStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }

    }
}