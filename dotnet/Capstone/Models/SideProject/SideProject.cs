using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class SideProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Image MainImageUrl { get; set; }
        public string Description { get; set; }
        public List<Goal> GoalsAndObjectives { get; set; }
        public List<Image> AdditionalImagesUrl { get; set; }
        public string VideoWalkthroughUrl { get; set; }
        public List<Skill> ToolsUsed { get; set; }
        public Website Website { get; set; }
        public Website GitHubRepoLink { get; set; }
        public List<Contributor> Contributors { get; set; }
        public List<ApiService> ExternalAPIsAndServicesUsed { get; set; }
        public List<DependencyLibrary> DependenciesOrLibrariesUsed { get; set; }
        public string ProjectStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }

    }
}