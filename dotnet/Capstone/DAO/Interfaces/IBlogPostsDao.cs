using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IBlogPostsDao
    {
        List<BlogPost> GetBlogPosts();
        BlogPost GetBlogPostById(int blogPostId);

    }
}