using System.Collections.Generic;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private IBlogPostsDao blogPostsDao;

        public BlogPostsController(IBlogPostsDao blogPostsDao) {
            this.blogPostsDao = blogPostsDao;
        }

        [HttpGet("/blogposts")]
        public ActionResult<List<BlogPost>> GetBlogPosts()
        {
            List<BlogPost> blogPosts = blogPostsDao.GetBlogPosts();

            if (blogPosts == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(blogPosts);
            }
        }

        [HttpGet("/blogpost/{blogPostId}")]
        public ActionResult<BlogPost> GetBlogPostById(int blogPostId)
        {
            BlogPost blogPost = blogPostsDao.GetBlogPostById(blogPostId);

            if (blogPost == null) 
            {
                return NotFound();
            }
            else
            {
                return Ok(blogPost);
            }
        }
    }

}