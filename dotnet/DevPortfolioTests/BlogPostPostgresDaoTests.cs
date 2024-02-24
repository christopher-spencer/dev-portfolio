using Capstone.DAO;
using Capstone.Models;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class BlogPostsPostgresDaoTests : PostgresDaoTestBase
    {
         [TestMethod]
        public void GetBlogPosts_Returns_All_Blog_Posts()
        {
            // Arrange
            BlogPostsPostgresDao dao = new BlogPostsPostgresDao(ConnectionString);

            // Act
            List<BlogPost> blogPosts = dao.GetBlogPosts();

            // Assert
            Assert.IsNotNull(blogPosts);
            Assert.IsTrue(blogPosts.Count > 0);
        }

        [TestMethod]
        public void GetBlogPostById_Returns_Correct_Blog_Post()
        {
            // Arrange
            BlogPostsPostgresDao dao = new BlogPostsPostgresDao(ConnectionString);

            // Act
            BlogPost blogPost = dao.GetBlogPostById(1);

            // Assert
            Assert.IsNotNull(blogPost);
            Assert.AreEqual(1, blogPost.Id);
        }

        [TestMethod]
        public void AddBlogPost_Inserts_New_Blog_Post()
        {
            // Arrange
            BlogPostsPostgresDao dao = new BlogPostsPostgresDao(ConnectionString);
            BlogPost newBlogPost = new BlogPost
            {
                Name = "New Blog Post",
                Author = "John Doe",
                Description = "Description of the new blog post",
                Content = "Content of the new blog post",
                ImageName = "image.jpg",
                ImageUrl = "https://example.com/image.jpg",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Act
            BlogPost addedBlogPost = dao.CreateBlogPost(newBlogPost);

            // Assert
            Assert.IsNotNull(addedBlogPost);
            Assert.AreNotEqual(0, addedBlogPost.Id); 
            Assert.AreEqual(newBlogPost.Name, addedBlogPost.Name);
            Assert.AreEqual(newBlogPost.Author, addedBlogPost.Author);
            // TODO Assert other properties...
        }

        [TestMethod]
        public void UpdateBlogPost_Updates_Existing_Blog_Post()
        {
            // Arrange
            BlogPostsPostgresDao dao = new BlogPostsPostgresDao(ConnectionString);
 
            BlogPost updatedBlogPost = new BlogPost
            {
                Id = 1, 
                Name = "Updated Blog Post",
                Author = "Jane Doe",
                Description = "Updated description",
                Content = "Updated content",
                ImageName = "updated_image.jpg",
                ImageUrl = "https://example.com/updated_image.jpg",
                CreatedAt = DateTime.Now, 
                UpdatedAt = DateTime.Now
            };

            // Act
            BlogPost result = dao.UpdateBlogPost(updatedBlogPost, updatedBlogPost.Id);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DeleteBlogPostByBlogPostId_Deletes_Existing_Blog_Post()
        {
            // Arrange
            BlogPostsPostgresDao dao = new BlogPostsPostgresDao(ConnectionString);

            // Act
            int rowsAffected = dao.DeleteBlogPostByBlogPostId(1);

            // Assert
            Assert.AreEqual(1, rowsAffected);
        }
    }
}



