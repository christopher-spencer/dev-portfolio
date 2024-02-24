using System;

namespace Capstone.Models
{
    public class BlogPost
    {
         // TODO: add new Blog Post properties after initial integration testing
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        //TODO currently this is being set to zero until possible id associated? Need to Look into THIS!!!
        public int MainImageId { get; set; }
        public Image MainImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}