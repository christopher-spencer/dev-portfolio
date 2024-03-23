using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Education
    {
        public int Id { get; set; }
        public string InstitutionName { get; set; }
        public int InstitutionLogoId { get; set; }
        public Image InstitutionLogo { get; set; }
        public int InstitutionWebsiteId { get; set; }
        public Website InstitutionWebsite { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string FieldOfStudy { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }
        public string DegreeObtained { get; set; }
        public decimal GPAOverall { get; set; }
        public decimal GPAInMajor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime GraduationDate { get; set; }
        public List<Achievement> HonorsAndAwards { get; set; }
        public int MainImageId { get; set; }
        public Image MainImage { get; set; }
        public List<Image> AdditionalImages { get; set; }

        // Method to format overall GPA
        public string FormatOverallGPA()
        {
            return $"{GPAOverall} / 4";
        }

        // Method to format major GPA
        public string FormatGPAInMajor()
        {
            return $"{GPAInMajor} / 4";
        }

        // Override ToString() method
        public override string ToString()
        {
            string overallGPAString = FormatOverallGPA();
            string majorGPAString = FormatGPAInMajor();
            return $"Overall GPA: {overallGPAString}, Major GPA: {majorGPAString}";
        }
    }
}