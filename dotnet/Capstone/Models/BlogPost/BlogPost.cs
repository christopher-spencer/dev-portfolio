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
        public int MainImageId { get; set; }
        public Image MainImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    /*
        Categories or Tags - Allow categorization or tagging of blog posts to make them easier to organize and search.
        Comments - Include a collection of comments associated with each blog post.
        Likes or Up-votes - Could include a counter for the number of likes or up-votes a blog post receives.
        Views or Page Visits - Keep track of how many times a blog post has been viewed to gauge its popularity.
        Status - Include a field to indicate the status of the blog post (e.g., draft, published, archived).
        SEO Metadata - Add fields for SEO metadata such as meta title, meta description, and keywords to optimize search engine visibility.
        Related Posts - Include a list of related posts to encourage further reading and engagement.
        Author Information - If eventually multiple authors/contributors, could associate posts with additional author info or links 
            like bio, profile picture, or contact details.
        Featured Post Flag - Mark a post as featured, which could be used for highlighting important or noteworthy content.
        Attachments - Allow for the attachment of files or documents related to posts, such as PDFs, images, or other media.
    */
}