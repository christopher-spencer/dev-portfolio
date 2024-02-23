using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IconId { get; set; }
        public Image Icon { get; set; }
    }
}