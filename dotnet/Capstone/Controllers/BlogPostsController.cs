using System.Collections.Generic;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost("/create-blogpost")]
        public ActionResult AddBlogPost(BlogPost blogPost)
        {
            BlogPost createdBlogPost = blogPostsDao.CreateBlogPost(blogPost);

            if (createdBlogPost == null) 
            {
                return BadRequest();
            }
            else
            {
                return CreatedAtAction(nameof(GetBlogPostById), new { blogPostId = createdBlogPost.Id }, createdBlogPost);

            }
        }

        [Authorize]
        [HttpPut("/update-blogpost/{blogPostId}/")]
        public ActionResult UpdateBlogPost(BlogPost blogPost, int blogPostId)
        {
            BlogPost updatedBlogPost = blogPostsDao.UpdateBlogPost(blogPost, blogPostId);

            if (updatedBlogPost == null)
            {
                 return BadRequest();
            }
            else 
            {
                return Ok(updatedBlogPost);
            }
        }

        // UPDATE THE EXCEPTIONS

        [Authorize]
        [HttpDelete("/blogpost/delete/{blogPostId}")]
        public ActionResult DeleteBlogPostByBlogPostId(int blogPostId)
        {
            try
            {
                int rowsAffected = blogPostsDao.DeleteBlogPostByBlogPostId(blogPostId);
        
                if (rowsAffected > 0)
                {
                    return Ok("Blog post deleted successfully");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DaoException ex)
            {
                return StatusCode(500, "An error occurred while deleting the blog post.");
            }
        }

    }
}