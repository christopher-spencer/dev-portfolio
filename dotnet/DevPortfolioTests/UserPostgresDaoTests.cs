using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using System.Data.SqlClient;
using Capstone.DAO;
using Capstone.Models;
using Capstone.UnitTests.DAO;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class UserPostgresDAOTests : PostgresDaoTestBase
    {
        [TestMethod]
        public void GetUserTest()
        {
            UserPostgresDao access = new UserPostgresDao(TestConnectionString);

            User user = access.GetUserByUsername("testUser");

            Assert.IsNotNull(user);

            Assert.AreEqual("user", user.Role);
        }

        [TestMethod]
        public void AddUserTest()
        {
            UserPostgresDao access = new UserPostgresDao(TestConnectionString);

            User user = access.CreateUser("notAUser", "password", "admin");

            Assert.IsNotNull(user);

            Assert.AreEqual("admin", user.Role);

            user = access.GetUserByUsername("notAUser");

            Assert.IsNotNull(user);

            Assert.AreEqual("admin", user.Role);
        }
    }
}