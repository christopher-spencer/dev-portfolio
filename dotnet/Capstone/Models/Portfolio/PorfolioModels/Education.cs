using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Education
    {
        public int Id { get; set; }
        public string SchoolOrInstitution { get; set; }
        public string InstitutionLogoUrl { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string FieldOfStudy { get; set; }
        public string DegreeObtained { get; set; }
        public int GPA { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime GraduationDate { get; set; }
        public List<string> HonorsAndAwards { get; set; }
        public Image MainImageUrl { get; set; }
        public List<Image> AdditionalImagesUrl { get; set; }
    }
}