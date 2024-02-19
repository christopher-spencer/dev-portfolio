using System;
using System.Collections.Generic;

namespace Capstone.Models
{
    public class SideProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TechSkill> ToolsUsed { get; set; }
    }
}