using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IBlogPostsDao
    {
        /*
            BLOGPOST CRUD
        */
        List<BlogPost> GetBlogPosts();
        BlogPost GetBlogPostById(int blogPostId);
        BlogPost CreateBlogPost(BlogPost blogPost);
        BlogPost UpdateBlogPost(BlogPost blogPost, int blogPostId);
        int DeleteBlogPostByBlogPostId(int blogPostId);

    }
}