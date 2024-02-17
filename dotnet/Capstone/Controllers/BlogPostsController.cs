using Capstone.DAO.Interfaces;
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


    }

}