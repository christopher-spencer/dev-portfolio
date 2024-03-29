using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Website
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public int LogoId { get; set; }
        public Image Logo { get; set; }
    }
}