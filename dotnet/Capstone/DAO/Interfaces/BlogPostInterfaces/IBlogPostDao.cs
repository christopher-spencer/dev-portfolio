using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IBlogPostDao
    {
        /*  
            **********************************************************************************************
                                        BLOG POST CRUD
            **********************************************************************************************
        */
        List<BlogPost> GetBlogPosts();
        BlogPost GetBlogPost(int blogPostId);
        BlogPost CreateBlogPost(BlogPost blogPost);
        BlogPost UpdateBlogPost(BlogPost blogPost, int blogPostId);
        int DeleteBlogPost(int blogPostId);

    }
}