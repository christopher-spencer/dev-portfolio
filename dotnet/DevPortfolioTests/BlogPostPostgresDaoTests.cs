using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq; 

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class BlogPostsPostgresDaoTests : PostgresDaoTestBase
    {
        private BlogPostPostgresDao dao = null!;
        private Mock<IImageDao> imageDaoMock = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            base.Initialize();
            imageDaoMock = new Mock<IImageDao>();
            dao = new BlogPostPostgresDao(TestConnectionString, imageDaoMock.Object);
        }

        //FIXME Add TEST INTIALIZE

        //FIXME MainImage needs fixed for BlogPosts
        [TestMethod]
        public void GetBlogPosts_Returns_All_Blog_Posts()
        {
            // Act
            dao.CreateBlogPost(new BlogPost
            {
                Name = "John Doe's Blog Post",
                Author = "John Doe",
                Description = "Description of the new blog post",
                Content = "Content of the new blog post",
                // CreatedAt = DateTime.Now,
                // UpdatedAt = DateTime.Now
            });

            List<BlogPost> blogPosts = dao.GetBlogPosts();

            // Assert
            Assert.IsNotNull(blogPosts);
            Assert.IsTrue(blogPosts.Count > 0);

            // Additional assertions for properties
            foreach (var blogPost in blogPosts)
            {
                Assert.IsNotNull(blogPost.Id);
                Assert.IsNotNull(blogPost.Name);
                Assert.IsNotNull(blogPost.Author);
                Assert.IsNotNull(blogPost.Description);
                Assert.IsNotNull(blogPost.Content);
                Assert.IsNotNull(blogPost.MainImageId);
                //Assert.IsNotNull(blogPost.MainImage);
                Assert.IsNotNull(blogPost.CreatedAt);
                Assert.IsNotNull(blogPost.UpdatedAt);
            }
        }

        [TestMethod]
        public void GetBlogPostById_Returns_Correct_Blog_Post()
        {
            // Act
            dao.CreateBlogPost(new BlogPost
            {
                Name = "John Doe's Blog Post",
                Author = "John Doe",
                Description = "Description of the new blog post",
                Content = "Content of the new blog post",
                // CreatedAt = DateTime.Now,
                // UpdatedAt = DateTime.Now
            });

            BlogPost blogPost = dao.GetBlogPost(1);

            // Assert
            Assert.IsNotNull(blogPost);
            Assert.AreEqual(1, blogPost.Id);

            // Additional assertions for properties
            Assert.IsNotNull(blogPost.Name);
            Assert.IsNotNull(blogPost.Author);
            Assert.IsNotNull(blogPost.Description);
            Assert.IsNotNull(blogPost.Content);
            Assert.IsNotNull(blogPost.MainImageId);
            //Assert.IsNotNull(blogPost.MainImage);
            Assert.IsNotNull(blogPost.CreatedAt);
            Assert.IsNotNull(blogPost.UpdatedAt);
        }

// FIXME update for created at and updated at
        [TestMethod]
        public void AddBlogPost_Inserts_New_Blog_Post()
        {
            // Arrange
            BlogPost newBlogPost = new BlogPost
            {
                Name = "John Doe's Blog Post",
                Author = "John Doe",
                Description = "Description of the new blog post",
                Content = "Content of the new blog post",
                // CreatedAt = DateTime.Now,
                // UpdatedAt = DateTime.Now
            };

            // Act
            BlogPost addedBlogPost = dao.CreateBlogPost(newBlogPost);

            // Assert
            Assert.IsNotNull(addedBlogPost);
            Assert.AreNotEqual(0, addedBlogPost.Id);
            Assert.AreEqual(newBlogPost.Name, addedBlogPost.Name);
            Assert.AreEqual(newBlogPost.Author, addedBlogPost.Author);
            Assert.AreEqual(newBlogPost.Description, addedBlogPost.Description);
            Assert.AreEqual(newBlogPost.Content, addedBlogPost.Content);
            //Assert.AreEqual(newBlogPost.CreatedAt, addedBlogPost.CreatedAt);
            //Assert.AreEqual(newBlogPost.UpdatedAt, addedBlogPost.UpdatedAt);
            Assert.AreEqual(newBlogPost.MainImageId, addedBlogPost.MainImageId);
            //Assert.AreEqual(newBlogPost.MainImage, addedBlogPost.MainImage);
        }

        [TestMethod]
        public void UpdateBlogPost_Updates_Existing_Blog_Post()
        {
            // Arrange
            BlogPost blogPost = dao.CreateBlogPost(new BlogPost
            {
                Name = "John Doe's Blog Post",
                Author = "John Doe",
                Description = "Description of the new blog post",
                Content = "Content of the new blog post",
                // CreatedAt = DateTime.Now,
                // UpdatedAt = DateTime.Now
            });

            BlogPost updatedBlogPost = new BlogPost
            {
                Id = 1, 
                Name = "Updated Blog Post",
                Author = "Jane Doe",
                Description = "Updated description",
                Content = "Updated content",
                CreatedAt = DateTime.Now, 
                UpdatedAt = DateTime.Now,
                MainImageId = 2, // Example values for additional properties
                MainImage = new Image { Id = 2, Url = "example.jpg" }
            };

            // Act
            BlogPost result = dao.UpdateBlogPost(updatedBlogPost, blogPost.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(blogPost.Id, result.Id);
            Assert.AreEqual(updatedBlogPost.Name, result.Name);
            Assert.AreEqual(updatedBlogPost.Author, result.Author);
            Assert.AreEqual(updatedBlogPost.Description, result.Description);
            Assert.AreEqual(updatedBlogPost.Content, result.Content);
            Assert.AreEqual(updatedBlogPost.CreatedAt, result.CreatedAt);
            Assert.AreEqual(updatedBlogPost.UpdatedAt, result.UpdatedAt);
            Assert.AreEqual(updatedBlogPost.MainImageId, result.MainImageId);
            Assert.AreEqual(updatedBlogPost.MainImage, result.MainImage);
        }
// FIXME foreign key constraint violation
        // [TestMethod]
        // public void DeleteBlogPostByBlogPostId_Deletes_Existing_Blog_Post()
        // {
        //     // Arrange
        //     var imageDaoMock = new Mock<IImageDao>(); // Mock IImageDao
        //     var blogPostId = 1;

        //     // Setup mock to delete images and return the number of rows affected
        //     imageDaoMock.Setup(dao => dao.DeleteImageByBlogPostId(blogPostId, It.IsAny<int>())).Returns(1);

        //     BlogPostPostgresDao dao = new BlogPostPostgresDao(TestConnectionString, imageDaoMock.Object);

        //     // Act
        //     int blogPostRowsAffected = dao.DeleteBlogPost(blogPostId);

        //     // Assert
        //     imageDaoMock.Verify(dao => dao.DeleteImageByBlogPostId(blogPostId, It.IsAny<int>()), Times.Once);
        //     Assert.AreEqual(1, blogPostRowsAffected);
        // }
    }
}




