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

        private Mock<IPortfolioDao> portfolioDaoMock = null!;

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

            portfolioDaoMock = new Mock<IPortfolioDao>();
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
            // Act: Create portfolio
            Portfolio createdPortfolio = portfolioDaoMock.Object.CreatePortfolio(new Portfolio
            {
                Name = "Test Portfolio 1",
                Location = "Test Location 1",
                ProfessionalSummary = "Test Professional Summary 1",
                Email = "Test Email 1"
            });

            // Assert: Verify portfolio creation
            Assert.IsNotNull(createdPortfolio);
            Assert.IsNotNull(createdPortfolio.Id);

            // Act: Create side project associated with the created portfolio
            dao.CreateSideProjectByPortfolioId(createdPortfolio.Id, new SideProject
            {
                Name = "Test SideProject 1",
                Description = "Test Description 1",
                StartDate = DateTime.Now,
                FinishDate = DateTime.Now
            });

            // Act: Retrieve side projects
            List<SideProject> sideProjects = dao.GetSideProjects();

            // Assert: Verify side project retrieval
            Assert.AreEqual(1, sideProjects.Count);

            // Additional assertions for properties
            foreach (var sideProject in sideProjects)
            {
                Assert.IsNotNull(sideProject.Id);
                Assert.IsNotNull(sideProject.Name);
                Assert.IsNotNull(sideProject.Description);
                Assert.IsNotNull(sideProject.StartDate);
                Assert.IsNotNull(sideProject.FinishDate);
            }
        }

        [TestMethod]
        public void GetSideProjects_Returns_Empty_List_When_No_SideProjects()
        {
            // Arrange
            int deleteResult = dao.DeleteSideProjectByPortfolioId(1, 1);
            bool isDeleted = deleteResult == 1;

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
            SideProject updatedSideProject = dao.UpdateSideProjectByPortfolioId(portfolioId, sideProject.Id, sideProject);

            // Assert
            Assert.IsNotNull(updatedSideProject);
            Assert.AreEqual(sideProject.Id, updatedSideProject.Id);
            Assert.AreEqual(sideProject.Name, updatedSideProject.Name);
            Assert.AreEqual(sideProject.Description, updatedSideProject.Description);
            Assert.AreEqual(sideProject.StartDate, updatedSideProject.StartDate);
            Assert.AreEqual(sideProject.FinishDate, updatedSideProject.FinishDate);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            base.Cleanup();
        }

    }
}