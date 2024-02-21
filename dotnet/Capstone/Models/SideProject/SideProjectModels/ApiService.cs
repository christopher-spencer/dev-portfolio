using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class ApiService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Website Url { get; set; }
        public Image ImageLogoUrl { get; set; }
    }
}