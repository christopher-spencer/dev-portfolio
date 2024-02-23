using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ContributorImageId { get; set; }
        public Image ContributorImage { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string ContributionDetails { get; set; }
        public int LinkedInId { get; set; }
        public Website LinkedIn { get; set; }
        public int GitHubId { get; set; }
        public Website GitHub { get; set; }
        public int PortfolioId { get; set; }
        public Website Portfolio { get; set; }

    }
}