using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capstone.DAO;
using Capstone.Models;
using Moq;
using System;
using System.Collections.Generic;
using Npgsql;
using System.Data;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class BlogPostsPostgresDaoTests
    {
        // [TestMethod]
        // public void GetBlogPosts_Returns_All_Blog_Posts()
        // {
        //     // Arrange
        //     var mockDataReader = new Mock<NpgsqlDataReader>();
        //     mockDataReader.SetupSequence(m => m.Read())
        //         .Returns(true)
        //         .Returns(true)
        //         .Returns(false); // Simulating two rows in the result set

        //     mockDataReader.Setup(m => m["blogpost_id"]).Returns(1);
        //     mockDataReader.Setup(m => m["blogpost_name"]).Returns("Test Post 1");
        //     mockDataReader.Setup(m => m["blogpost_author"]).Returns("Author 1");
        //     mockDataReader.Setup(m => m["blogpost_description"]).Returns("Description 1");
        //     mockDataReader.Setup(m => m["blogpost_content"]).Returns("Content 1");
        //     mockDataReader.Setup(m => m["image_name"]).Returns("Image 1");
        //     mockDataReader.Setup(m => m["image_url"]).Returns("URL 1");
        //     mockDataReader.Setup(m => m["created_at"]).Returns(DateTime.Now);
        //     mockDataReader.Setup(m => m["updated_at"]).Returns(DateTime.Now);

        //     var mockConnection = new Mock<NpgsqlConnection>();
        //     mockConnection.Setup(m => m.State).Returns(System.Data.ConnectionState.Open);
        //     mockConnection.Setup(m => m.CreateCommand()).Returns(() =>
        //     {
        //         var cmdMock = new Mock<NpgsqlCommand>();
        //         cmdMock.SetupAllProperties();
        //         cmdMock.SetupProperty(m => m.Connection, mockConnection.Object);
        //         cmdMock.Setup(m => m.ExecuteReader(CommandBehavior.Default)).Returns(mockDataReader.Object);

        //         return cmdMock.Object;
        //     });

        //     var dao = new BlogPostsPostgresDao("fake_connection_string");

        //     // Act
        //     List<BlogPost> result = dao.GetBlogPosts();

        //     // Assert
        //     Assert.AreEqual(2, result.Count); // Assuming we expect two blog posts
        //     // Add more assertions as needed for the returned blog posts
        //     Assert.AreEqual("Test Post 1", result[0].Name);
        //     Assert.AreEqual("Author 1", result[0].Author);
        //     Assert.AreEqual("Description 1", result[0].Description);
        //     Assert.AreEqual("Content 1", result[0].Content);
        //     // Assert other properties as well
        // }
    }
}




