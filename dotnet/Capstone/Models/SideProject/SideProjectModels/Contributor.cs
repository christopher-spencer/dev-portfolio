using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Image ContributorImage { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string ContributionDetails { get; set; }
        public Website LinkedInLink { get; set; }
        public Website GitHubLink { get; set; }
        public Website PortfolioLink { get; set; }

    }
}