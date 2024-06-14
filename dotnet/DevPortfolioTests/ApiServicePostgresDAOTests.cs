using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class ApiServicePostgresDAOTests : PostgresDaoTestBase
    {
        private ApiServicePostgresDao dao = null!;
        private Mock<IImageDao> mockImageDao = null!;
        private Mock<IWebsiteDao> mockWebsiteDao = null!;

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
            mockWebsiteDao = new Mock<IWebsiteDao>();
        }

        private ApiServicePostgresDao CreateDaoWithMocks()
        {
            return new ApiServicePostgresDao(
                TestConnectionString,
                mockImageDao.Object,
                mockWebsiteDao.Object
            );
        }

        private ApiService CreateAnApiServiceTestObject1()
        {
            return new ApiService
            {
                Name = "Test Name 1",
                Description = "Test Description 1"
            };
        }

        private ApiService CreateAnApiServiceTestObject2()
        {
            return new ApiService
            {
                Name = "Test Name 2",
                Description = "Test Description 2"
            };
        }

        private void SetUpApiServiceNestedDaoMockObjects()
        {
            mockImageDao.Setup(m => m.GetImageByApiServiceId(It.IsAny<int>())).Returns(new Image());
            mockWebsiteDao.Setup(m => m.GetWebsiteByApiServiceId(It.IsAny<int>())).Returns(new Website());
        }

        // TODO Add tests for ApiServicePostgresDao


    }
}