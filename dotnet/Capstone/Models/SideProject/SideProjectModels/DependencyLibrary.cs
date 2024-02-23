using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class DependencyLibrary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int WebsiteId { get; set; }
        public Website Website { get; set; }
        public int LogoId { get; set; }
        public Image Logo { get; set; }
    }
}