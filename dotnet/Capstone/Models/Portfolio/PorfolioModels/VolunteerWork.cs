using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class VolunteerWork
    {
        public string OrganizationName { get; set; }
        public string OrganizationLogoUrl { get; set; }
        public string Location { get; set; }
        public string OrganizationDescription { get; set; }
        public string OrganizationWebsiteUrl { get; set; }
        public string RoleOrPosition { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> KeyResponsibilities { get; set; }
        public List<Skill> SkillsUsedAndObtained { get; set; }
        public string MainImageUrl { get; set; }
        public List<string> AdditionalImagesUrl { get; set; }

    }
}
