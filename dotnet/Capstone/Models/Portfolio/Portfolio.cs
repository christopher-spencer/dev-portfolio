using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string ProfessionalSummary { get; set; }
        public List<string> InterestsAndHobbies { get; set; }
        public string Email { get; set; }
        public string GitHubURL { get; set; }
        public string LinkedInURL { get; set; }
        public List<TechSkill> TechSkills { get; set; }
        public List<SideProject> SideProjects { get; set; }
        public List<string> BackgroundExperiences { get; set; }
        public List<string> EducationHistory { get; set; }
        public List<Credential> CertificationsAndCredentials { get; set; }
        public List<string> VolunteerWorks { get; set; }
        public List<string> OpenSourceContributions { get; set; }
    }
}