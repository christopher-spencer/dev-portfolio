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
//FIXME Need to Create a SideProject object in TestData file for DependencyLibraryPostgresDaoTests
        // TODO Add tests for DependencyLibraryPostgresDao
        [TestMethod]
        public void GetDependenciesAndLibraries_Returns_All_Dependencies_And_Libraries()
        {
            // Arrange
            int sideProjectId = 1;
            DependencyLibrary dependencyLibrary1 = CreateADependencyLibraryTestObject1();
            DependencyLibrary dependencyLibrary2 = CreateADependencyLibraryTestObject2();

            dao.CreateDependencyOrLibraryBySideProjectId(sideProjectId, dependencyLibrary1);
            dao.CreateDependencyOrLibraryBySideProjectId(sideProjectId, dependencyLibrary2);
            SetUpDependencyLibraryNestedDaoMockObjects();

            // Act
            List<DependencyLibrary> dependenciesAndLibraries = dao.GetDependenciesAndLibrariesBySideProjectId(sideProjectId);

            // Assert
            Assert.AreEqual(2, dependenciesAndLibraries.Count);

            Assert.AreEqual(dependencyLibrary1.Name, dependenciesAndLibraries[0].Name);
            Assert.AreEqual(dependencyLibrary1.Description, dependenciesAndLibraries[0].Description);

            Assert.AreEqual(dependencyLibrary2.Name, dependenciesAndLibraries[1].Name);
            Assert.AreEqual(dependencyLibrary2.Description, dependenciesAndLibraries[1].Description);
        }
    }
}