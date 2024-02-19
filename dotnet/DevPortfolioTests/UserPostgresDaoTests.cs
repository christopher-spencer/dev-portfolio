using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using System.Data.SqlClient;
using Capstone.DAO;
using Capstone.Models;
using Capstone.UnitTests.DAO;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class UserPostgresDAOTest : PostgresDaoTestBase
    {
        [TestMethod]
        public void GetUserTest()
        {
            UserPostgresDao access = new UserPostgresDao(ConnectionString);

            User user = access.GetUserByUsername("notauser");

            Assert.IsNotNull(user);

            Assert.AreEqual("user", user.Role);
        }

        [TestMethod]
        public void AddUserTest()
        {
            UserPostgresDao access = new UserPostgresDao(ConnectionString);

            User user = access.CreateUser("testuser", "password", "admin");

            Assert.IsNotNull(user);

            Assert.AreEqual("admin", user.Role);

            user = access.GetUserByUsername("testuser");

            Assert.IsNotNull(user);

            Assert.AreEqual("admin", user.Role);
        }
    }
}