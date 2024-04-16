using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Credential
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IssuingOrganization { get; set; }
        public string Description { get; set; }
        public int OrganizationLogoId { get; set; }
        public Image OrganizationLogo { get; set; }
        public int OrganizationWebsiteId { get; set; }
        public Website OrganizationWebsite { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CredentialIdNumber { get; set; }
        public int CredentialWebsiteId { get; set; }
        public Website CredentialWebsite { get; set; }
        public int MainImageId { get; set; }
        public Image MainImage { get; set; }
        public List <Skill> AssociatedSkills { get; set; }
    } 
}