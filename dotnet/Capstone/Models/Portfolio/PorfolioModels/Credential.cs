using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Credential
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IssuingOrganization { get; set; }
        public Image OrganizationLogoUrl { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int CredentialId { get; set; }
        public string CredentialURL { get; set; }
        public Image ImageUrl { get; set; }
        public List <Skill> AssociatedSkills { get; set; }
    } 
}