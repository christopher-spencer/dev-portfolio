using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public string GitHubURL { get; set; }
        public string LinkedInURL { get; set; }
        public List<string> TechSkills { get; set; }
        public List<string> Projects { get; set; }
        public List<string> Experience { get; set; }
        public List<string> Education { get; set; }
        public List<string> Certifications { get; set; }
    }
}