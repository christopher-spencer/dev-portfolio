using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Certification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IssuingOrganization { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int CredentialId { get; set; }
        public string CredentialURL { get; set; }
        public string ImageUrl { get; set; }
        public List <TechSkill> AssociatedSkills { get; set; }
    } 
}