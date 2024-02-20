using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Experience
    {
        public int Id { get; set; }
        public string PositionTitle { get; set; }
        public string CompanyName { get; set; }
        public Image CompanyLogoUrl { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> ResponsibilitiesAndAchievements { get; set; }
        public List<Skill> SkillsUsedOrObtained { get; set; }
        public Image MainImageUrl { get; set; }
        public List<Image> AdditionalImagesUrl { get; set; }

    }
}