using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class DependencyLibraryPostgresDAOTests : PostgresDaoTestBase
    {
        private DependencyLibraryPostgresDao dao = null!;
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

        private DependencyLibraryPostgresDao CreateDaoWithMocks()
        {
            return new DependencyLibraryPostgresDao(
                TestConnectionString,
                mockImageDao.Object,
                mockWebsiteDao.Object
            );
        }

        private DependencyLibrary CreateADependencyLibraryTestObject1()
        {
            return new DependencyLibrary
            {
                Name = "Test Name 1",
                Description = "Test Description 1"
            };
        }

        private DependencyLibrary CreateADependencyLibraryTestObject2()
        {
            return new DependencyLibrary
            {
                Name = "Test Name 2",
                Description = "Test Description 2"
            };
        }

        private void SetUpDependencyLibraryNestedDaoMockObjects()
        {
            mockImageDao.Setup(m => m.GetImageByDependencyLibraryId(It.IsAny<int>())).Returns(new Image());
            mockWebsiteDao.Setup(m => m.GetWebsiteByDependencyLibraryId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Website());
        }

        // TODO Add tests for DependencyLibraryPostgresDao
    }
}