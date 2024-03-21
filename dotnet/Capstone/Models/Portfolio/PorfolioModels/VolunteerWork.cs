using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class VolunteerWork
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public int OrganizationLogoId { get; set; }
        public Image OrganizationLogo { get; set; }
        public string Location { get; set; }
        public string OrganizationDescription { get; set; }
        public int OrganizationWebsiteId { get; set; }
        public Website OrganizationWebsite { get; set; }
        public string PositionTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Achievement> ResponsibilitiesAndAchievements { get; set; }
        public List<Skill> SkillsUsedAndObtained { get; set; }
        public int MainImageId { get; set; }
        public Image MainImage { get; set; }
        public List<Image> AdditionalImages { get; set; }

    }
}
