using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Image PortfolioImage { get; set; }
        public string Location { get; set; }
        public string ProfessionalSummary { get; set; }
        public List<Hobby> Hobbies { get; set; }
        public string Email { get; set; }
        public Website GitHubURL { get; set; }
        public Website LinkedInURL { get; set; }
        public List<Skill> TechSkills { get; set; }
        public List<SideProject> SideProjects { get; set; }
        public List<Experience> BackgroundExperiences { get; set; }
        public List<Education> EducationHistory { get; set; }
        public List<Credential> CertificationsAndCredentials { get; set; }
        public List<VolunteerWork> VolunteerWorks { get; set; }
        public List<OpenSourceContribution> OpenSourceContributions { get; set; }
    }
}