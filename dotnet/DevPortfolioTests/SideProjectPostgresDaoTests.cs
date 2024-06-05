using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class SideProjectPostgresDaoTests : PostgresDaoTestBase
    {
        private SideProjectPostgresDao dao = null!;
        private Mock<IGoalDao> goalDaoMock = null!;
        private Mock<IImageDao> imageDaoMock = null!;
        private Mock<ISkillDao> skillDaoMock = null!;
        private Mock<IContributorDao> contributorDaoMock = null!;
        private Mock<IApiServiceDao> apiServiceDaoMock = null!;
        private Mock<IWebsiteDao> websiteDaoMock = null!;
        private Mock<IDependencyLibraryDao> dependencyLibraryDaoMock = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            base.Initialize();
            InitializeMocks();
            dao = CreateDaoWithMocks();
        }

        private void InitializeMocks()
        {
            goalDaoMock = new Mock<IGoalDao>();
            imageDaoMock = new Mock<IImageDao>();
            skillDaoMock = new Mock<ISkillDao>();
            contributorDaoMock = new Mock<IContributorDao>();
            apiServiceDaoMock = new Mock<IApiServiceDao>();
            websiteDaoMock = new Mock<IWebsiteDao>();
            dependencyLibraryDaoMock = new Mock<IDependencyLibraryDao>();
        }

        private SideProjectPostgresDao CreateDaoWithMocks()
        {
            return new SideProjectPostgresDao(
                TestConnectionString,
                goalDaoMock.Object,
                imageDaoMock.Object,
                skillDaoMock.Object,
                contributorDaoMock.Object,
                apiServiceDaoMock.Object,
                dependencyLibraryDaoMock.Object,
                websiteDaoMock.Object);
        }

        [TestMethod]
        public void GetSideProject_Returns_All_SideProjects()
        {
            //Act
            dao.CreateSideProjectByPortfolioId(1, new SideProject
            {
                Name = "Test SideProject 1",
                Description = "Test Description 1",
                StartDate = DateTime.Now,
                FinishDate = DateTime.Now
            });
            
            List<SideProject> sideProjects = dao.GetSideProjects();

            //Assert
            Assert.AreEqual(1, sideProjects.Count);
            Assert.IsTrue(sideProjects.Count > 0);

            // Additional assertions for properties
            foreach (var sideproject in sideProjects)
            {
                Assert.IsNotNull(sideproject.Id);
                Assert.IsNotNull(sideproject.Name);
                Assert.IsNotNull(sideproject.Description);
                Assert.IsNotNull(sideproject.StartDate);
                Assert.IsNotNull(sideproject.FinishDate);
            }
        }

        [TestMethod]
        public void GetSideProjects_Returns_Empty_List_When_No_SideProjects()
        {
            // Arrange


            // Act
            List<SideProject> sideProjects = dao.GetSideProjects();

            // Assert
            Assert.IsNotNull(sideProjects);
            Assert.IsTrue(sideProjects.Count == 0);
        }

        [TestMethod]
        public void GetSideProject_Returns_Correct_Sideproject()
        {
            // Act
            dao.CreateSideProjectByPortfolioId(1, new SideProject
            {
                Name = "Test SideProject 1",
                Description = "Test Description 1",
                StartDate = DateTime.Now,
                FinishDate = DateTime.Now
            });

            SideProject sideProject = dao.GetSideProject(1);

            // Assert
            Assert.IsNotNull(sideProject);
            Assert.AreEqual(1, sideProject.Id);

            // Additional assertions for properties
            Assert.IsNotNull(sideProject.Name);
            Assert.IsNotNull(sideProject.Description);
            Assert.IsNotNull(sideProject.StartDate);
            Assert.IsNotNull(sideProject.FinishDate);
        }

        [TestMethod]
        public void GetSideproject_Returns_Null_When_Sideproject_Does_Not_Exist()
        {
            // Act
            SideProject sideProject = dao.GetSideProject(3);

            // Assert
            Assert.IsNull(sideProject);
        }

        [TestMethod]
        public void CreateSideProjectByPortfolioId_Returns_Correct_Sideproject()
        {
            // Arrange
            SideProject sideProject = new SideProject
            {
                Name = "Test SideProject",
                Description = "Test Description",
                StartDate = DateTime.Now,
                FinishDate = DateTime.Now
            };

            // Act
            SideProject createdSideProject = dao.CreateSideProjectByPortfolioId(1, sideProject);

            // Assert
            Assert.IsNotNull(createdSideProject);
            Assert.AreEqual(sideProject.Name, createdSideProject.Name);
            Assert.AreEqual(sideProject.Description, createdSideProject.Description);
            Assert.AreEqual(sideProject.StartDate, createdSideProject.StartDate);
            Assert.AreEqual(sideProject.FinishDate, createdSideProject.FinishDate);
        }

        [TestMethod]
        public void CreateSideProjectByPortfolioId_Returns_Null_When_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;

            SideProject sideProject = new SideProject
            {
                Name = "Test SideProject",
                Description = "Test Description",
                StartDate = DateTime.Now,
                FinishDate = DateTime.Now
            };

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                dao.CreateSideProjectByPortfolioId(nonExistentPortfolioId, sideProject),
                "PortfolioId must be greater than zero.");

        }

        [TestMethod]
        public void UpdateSideProjectByPortfolioId_Returns_Updated_SideProject()
        {
            // Arrange
            int portfolioId = 1;

            SideProject sideProject = new SideProject
            {
                Id = 2,
                Name = "Test SideProject Updated Name Test 777",
                Description = "Test Description Test",
                StartDate = DateTime.Now,
                FinishDate = DateTime.Now
            };

            // Act
            SideProject createdSideProject = dao.CreateSideProjectByPortfolioId(portfolioId, sideProject);
           
            SideProject updatedSideProject = dao.UpdateSideProjectByPortfolioId(portfolioId, createdSideProject.Id, createdSideProject);

            // Assert
            Assert.IsNotNull(updatedSideProject);
            Assert.AreEqual(sideProject.Id, updatedSideProject.Id);
            Assert.AreEqual(sideProject.Name, updatedSideProject.Name);
            Assert.AreEqual(sideProject.Description, updatedSideProject.Description);
            Assert.AreEqual(sideProject.StartDate, updatedSideProject.StartDate);
            Assert.AreEqual(sideProject.FinishDate, updatedSideProject.FinishDate);
        }

        [TestMethod]
        public void UpdateSideProjectByPortfolioId_Returns_Null_When_SideProject_Does_Not_Exist()
        {
            // Arrange
            int portfolioId = 1;
            int nonExistentSideProjectId = -1;

            SideProject sideProject = new SideProject
            {
                Id = nonExistentSideProjectId,
                Name = "Test SideProject Updated Name Test 777",
                Description = "Test Description Test",
                StartDate = DateTime.Now,
                FinishDate = DateTime.Now
            };

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                dao.UpdateSideProjectByPortfolioId(portfolioId, nonExistentSideProjectId, sideProject),
                "SideProjectId must be greater than zero.");
        }

        [TestMethod]
        public void UpdateSideProjectByPortfolioId_Returns_Null_When_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;
            int sideProjectId = 1;

            SideProject sideProject = new SideProject
            {
                Id = sideProjectId,
                Name = "Test SideProject Updated Name Test 777",
                Description = "Test Description Test",
                StartDate = DateTime.Now,
                FinishDate = DateTime.Now
            };

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                dao.UpdateSideProjectByPortfolioId(nonExistentPortfolioId, sideProjectId, sideProject),
                "PortfolioId must be greater than zero.");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            base.Cleanup(); 
        }

    }
}