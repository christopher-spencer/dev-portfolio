using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class WebsitePostgresDAOTests : PostgresDaoTestBase
    {
        private WebsitePostgresDao dao = null!;
        private Mock<IImageDao> mockImageDao = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            base.Initialize();
            InitializeMocks();
            dao = CreateDaoWithMocks();
        }

        private void InitializeMocks()
        {
            mockImageDao = new Mock<IImageDao>();
        }

        private WebsitePostgresDao CreateDaoWithMocks()
        {
            return new WebsitePostgresDao(
                TestConnectionString,
                mockImageDao.Object
            );
        }

        private Website CreateAWebsiteTestObject1()
        {
            return new Website
            {
                Name = "Test Name 1",
                Url = "Test Url 1",
                Type = "Test Type 1"
            };
        }

        private Website CreateAWebsiteTestObject2()
        {
            return new Website
            {
                Name = "Test Name 2",
                Url = "Test Url 2",
                Type = "Test Type 2"
            };
        }

        private void SetUpWebsiteNestedDaoMockObjects()
        {
            mockImageDao.Setup(m => m.GetImageByWebsiteId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
        }

        // TODO add Unit Tests for WebsitePostgresDaoTests

    }
}