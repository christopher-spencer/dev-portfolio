using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int IconId { get; set; }
        public Image Icon { get; set; }
    }
}