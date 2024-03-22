using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Experience
    {
        public int Id { get; set; }
        public string PositionTitle { get; set; }
        public string CompanyName { get; set; }
        public int CompanyLogoId { get; set; }
        public Image CompanyLogo { get; set; }
        public int CompanyWebsiteId { get; set; }
        public Website CompanyWebsite { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Achievement> ResponsibilitiesAndAchievements { get; set; }
        public List<Skill> SkillsUsedOrObtained { get; set; }
        public int MainImageId { get; set; }
        public Image MainImage { get; set; }
        public List<Image> AdditionalImages { get; set; }

    }
}