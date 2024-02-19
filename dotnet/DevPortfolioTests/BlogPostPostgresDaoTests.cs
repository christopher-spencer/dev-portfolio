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
            // Assuming there are already some blog posts inserted in the database
            // Insert blog posts here (SQL provided below)

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
            // Assuming there's a blog post with ID 1 in the database
            // Insert blog post with ID 1 here (SQL provided below)

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
            BlogPost addedBlogPost = dao.AddBlogPost(newBlogPost);

            // Assert
            Assert.IsNotNull(addedBlogPost);
            Assert.AreNotEqual(0, addedBlogPost.Id); // Assuming ID is auto-generated
            // Additional assertions based on expected data
            Assert.AreEqual(newBlogPost.Name, addedBlogPost.Name);
            Assert.AreEqual(newBlogPost.Author, addedBlogPost.Author);
            // Assert other properties...
        }

        [TestMethod]
        public void UpdateBlogPost_Updates_Existing_Blog_Post()
        {
            // Arrange
            BlogPostsPostgresDao dao = new BlogPostsPostgresDao(ConnectionString);
            // Assuming there's a blog post with ID 1 in the database
            // Insert blog post with ID 1 here (SQL provided below)
            BlogPost updatedBlogPost = new BlogPost
            {
                Id = 1, // Assuming this is the ID of the existing blog post to update
                Name = "Updated Blog Post",
                Author = "Jane Doe",
                Description = "Updated description",
                Content = "Updated content",
                ImageName = "updated_image.jpg",
                ImageUrl = "https://example.com/updated_image.jpg",
                CreatedAt = DateTime.Now, // Assuming these values will not change during update
                UpdatedAt = DateTime.Now
            };

            // Act
            BlogPost result = dao.UpdateBlogPost(updatedBlogPost, updatedBlogPost.Id);

            // Assert
            Assert.IsNotNull(result);
            // Additional assertions if needed
        }

        [TestMethod]
        public void DeleteBlogPostByBlogPostId_Deletes_Existing_Blog_Post()
        {
            // Arrange
            BlogPostsPostgresDao dao = new BlogPostsPostgresDao(ConnectionString);
            // Assuming there's a blog post with ID 1 in the database
            // Insert blog post with ID 1 here (SQL provided below)

            // Act
            int rowsAffected = dao.DeleteBlogPostByBlogPostId(1);

            // Assert
            Assert.AreEqual(1, rowsAffected);
            // Additional assertions if needed
        }
    }
}



