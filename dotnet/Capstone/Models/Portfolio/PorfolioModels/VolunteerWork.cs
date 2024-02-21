using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class VolunteerWork
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public Image OrganizationLogoUrl { get; set; }
        public string Location { get; set; }
        public string OrganizationDescription { get; set; }
        public Website OrganizationWebsiteUrl { get; set; }
        public string PositionTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Achievement> ResponsibilitiesAndAchievements { get; set; }
        public List<Skill> SkillsUsedAndObtained { get; set; }
        public Image MainImageUrl { get; set; }
        public List<Image> AdditionalImagesUrl { get; set; }

    }
}
