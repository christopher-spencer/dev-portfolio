using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MainImageId { get; set; }
        public Image MainImage { get; set; }
        // TODO create model for Country/Region, City, State, ZipCode (?)
        // NOTE better name for location?
        public string Location { get; set; }
        public string ProfessionalSummary { get; set; }
        public List<Hobby> Hobbies { get; set; }
        public string Email { get; set; }
        public int GitHubId { get; set; }
        public Website GitHub { get; set; }
        public int LinkedInId { get; set; }
        public Website LinkedIn { get; set; }
        public List<Skill> TechSkills { get; set; }
        public List<SideProject> SideProjects { get; set; }
        public List<WorkExperience> BackgroundExperiences { get; set; }
        public List<Education> EducationHistory { get; set; }
        public List<Credential> CertificationsAndCredentials { get; set; }
        public List<VolunteerWork> VolunteerWorks { get; set; }
        public List<OpenSourceContribution> OpenSourceContributions { get; set; }
        public List<Image> AdditionalImages { get; set; }

    }
}