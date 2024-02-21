using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class DependencyLibrary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public WebsiteLink Website { get; set; }
        public Image Logo { get; set; }
    }
}