using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class ImagePostgresDAOTests : PostgresDaoTestBase
    {
        private ImagePostgresDao dao = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            base.Initialize();
            dao = new ImagePostgresDao(TestConnectionString);
        }

        private Image CreateAnImageTestObject1()
        {
            return new Image
            {
                Name = "Test Name 1",
                Url = "Test Url 1",
                Type = "Test Type 1"
            };
        }

        private Image CreateAnImageTestObject2()
        {
            return new Image
            {
                Name = "Test Name 2",
                Url = "Test Url 2",
                Type = "Test Type 2"
            };
        }

        // TODO add Unit Tests for ImagePostgresDaoTests

    }
}