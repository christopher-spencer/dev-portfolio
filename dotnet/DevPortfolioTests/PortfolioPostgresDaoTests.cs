using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class PortfolioPostgresDaoTests : PostgresDaoTestBase
    {
        private PortfolioPostgresDao dao = null!;
        private Mock<ISideProjectDao> sideProjectDaoMock = null!;
        private Mock<IWebsiteDao> websiteDaoMock = null!;
        private Mock<IImageDao> imageDaoMock = null!;
        private Mock<ISkillDao> skillDaoMock = null!;
        private Mock<IWorkExperienceDao> workExperienceDaoMock = null!;
        private Mock<IEducationDao> educationDaoMock = null!;
        private Mock<ICredentialDao> credentialDaoMock = null!;
        private Mock<IVolunteerWorkDao> volunteerWorkDaoMock = null!;
        private Mock<IOpenSourceContributionDao> openSourceContributionDaoMock = null!;
        private Mock<IHobbyDao> hobbyDaoMock = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            base.Initialize();
            InitializeMocks();
            dao = CreateDaoWithMocks();
        }

        private void InitializeMocks()
        {
            sideProjectDaoMock = new Mock<ISideProjectDao>();
            websiteDaoMock = new Mock<IWebsiteDao>();
            imageDaoMock = new Mock<IImageDao>();
            skillDaoMock = new Mock<ISkillDao>();
            workExperienceDaoMock = new Mock<IWorkExperienceDao>();
            educationDaoMock = new Mock<IEducationDao>();
            credentialDaoMock = new Mock<ICredentialDao>();
            volunteerWorkDaoMock = new Mock<IVolunteerWorkDao>();
            openSourceContributionDaoMock = new Mock<IOpenSourceContributionDao>();
            hobbyDaoMock = new Mock<IHobbyDao>();
        }

        private PortfolioPostgresDao CreateDaoWithMocks()
        {
            return new PortfolioPostgresDao(
                TestConnectionString,
                sideProjectDaoMock.Object,
                websiteDaoMock.Object,
                imageDaoMock.Object,
                skillDaoMock.Object,
                workExperienceDaoMock.Object,
                educationDaoMock.Object,
                credentialDaoMock.Object,
                volunteerWorkDaoMock.Object,
                openSourceContributionDaoMock.Object,
                hobbyDaoMock.Object);
        }

        [TestMethod]
        public void GetPortfolios_Returns_All_Portfolios()
        {

            // Act
            dao.CreatePortfolio(new Portfolio
            {
                Name = "Test Portfolio",
                Location = "Test Location",
                ProfessionalSummary = "Test Professional Summary",
                Email = "Test Email"
            });

            List<Portfolio> portfolios = dao.GetPortfolios();

            // Assert
            Assert.IsNotNull(portfolios);
            Assert.IsTrue(portfolios.Count > 0);

            // Additional assertions for properties
            foreach (var portfolio in portfolios)
            {
                Assert.IsNotNull(portfolio.Id);
                Assert.IsNotNull(portfolio.Name);
                Assert.IsNotNull(portfolio.Location);
                Assert.IsNotNull(portfolio.ProfessionalSummary);
                Assert.IsNotNull(portfolio.Email);
            }
        }

        [TestMethod]
        public void GetPortfolios_Returns_Empty_List_If_No_Portfolios()
        {
            // Arrange

            websiteDaoMock.Setup(x => x.GetWebsiteByPortfolioId(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int portfolioId, int websiteId) => new Website { Id = websiteId });

            imageDaoMock.Setup(x => x.GetMainImageByPortfolioId(It.IsAny<int>()))
                .Returns((int portfolioId) => new Image { Id = 1 });

            dao.DeletePortfolio(1);
            dao.DeletePortfolio(2);

            // Act
            List<Portfolio> portfolios = dao.GetPortfolios();

            // Assert
            Assert.IsNotNull(portfolios);
            Assert.AreEqual(0, portfolios.Count);
        }

        [TestMethod]
        public void GetPortfolioById_Returns_Correct_Portfolio()
        {
            // Act
            dao.CreatePortfolio(new Portfolio
            {
                Id = 1,
                Name = "Test Portfolio",
                Location = "Test Location",
                ProfessionalSummary = "Test Professional Summary",
                Email = "Test Email"
            });
            
            Portfolio portfolio = dao.GetPortfolio(1);

            // Assert
            Assert.IsNotNull(portfolio);
            Assert.AreEqual(1, portfolio.Id);

            // Additional assertions for properties
            Assert.IsNotNull(portfolio.Name);
            Assert.IsNotNull(portfolio.Location);
            Assert.IsNotNull(portfolio.ProfessionalSummary);
            Assert.IsNotNull(portfolio.Email);
        }

        [TestMethod]
        public void GetPortfolioById_Returns_Null_If_Portfolio_Does_Not_Exist()
        {
            // Act
            Portfolio portfolio = dao.GetPortfolio(999);

            // Assert
            Assert.IsNull(portfolio);
        }

        [TestMethod]
        public void CreatePortfolio_Returns_Portfolio_With_Id()
        {
            // Arrange
            Portfolio portfolio = new Portfolio
            {
                Id = 2,
                Name = "Test Portfolio",
                Location = "Test Location",
                ProfessionalSummary = "Test Professional Summary",
                Email = "Test Email"
            };

            // Act
            Portfolio createdPortfolio = dao.CreatePortfolio(portfolio);

            // Assert
            Assert.IsNotNull(createdPortfolio);
            Assert.IsNotNull(createdPortfolio.Id);
            Assert.AreEqual(portfolio.Name, createdPortfolio.Name);
            Assert.AreEqual(portfolio.Location, createdPortfolio.Location);
            Assert.AreEqual(portfolio.ProfessionalSummary, createdPortfolio.ProfessionalSummary);
            Assert.AreEqual(portfolio.Email, createdPortfolio.Email);

            // Additional assertions for properties
            Assert.IsNotNull(createdPortfolio.Name);
            Assert.IsNotNull(createdPortfolio.Location);
            Assert.IsNotNull(createdPortfolio.ProfessionalSummary);
            Assert.IsNotNull(createdPortfolio.Email);
        }

        [TestMethod]
        public void UpdatePortfolio_Returns_Updated_Portfolio()
        {
            // Arrange
            Portfolio portfolio = new Portfolio
            {
                Id = 2,
                Name = "Updated Portfolio",
                Location = "Updated Location",
                ProfessionalSummary = "Updated Professional Summary",
                Email = "Updated Email"
            };

            // Act
            Portfolio createdPortfolio = dao.CreatePortfolio(portfolio);
            Portfolio updatedPortfolio = dao.UpdatePortfolio(createdPortfolio, createdPortfolio.Id);

            // Assert
            Assert.IsNotNull(updatedPortfolio);
            Assert.AreEqual(portfolio.Id, updatedPortfolio.Id);
            Assert.AreEqual(portfolio.Name, updatedPortfolio.Name);
            Assert.AreEqual(portfolio.Location, updatedPortfolio.Location);
            Assert.AreEqual(portfolio.ProfessionalSummary, updatedPortfolio.ProfessionalSummary);
            Assert.AreEqual(portfolio.Email, updatedPortfolio.Email);

            // Additional assertions for properties
            Assert.IsNotNull(updatedPortfolio.Name);
            Assert.IsNotNull(updatedPortfolio.Location);
            Assert.IsNotNull(updatedPortfolio.ProfessionalSummary);
            Assert.IsNotNull(updatedPortfolio.Email);
        }

        [TestMethod]
        public void UpdatePortfolio_Returns_Null_If_Portfolio_Does_Not_Exist()
        {
            // Arrange
            Portfolio portfolio = new Portfolio
            {
                Id = 999,
                Name = "Updated Portfolio",
                Location = "Updated Location",
                ProfessionalSummary = "Updated Professional Summary",
                Email = "Updated Email"
            };

            // Act
            Portfolio updatedPortfolio = dao.UpdatePortfolio(portfolio, portfolio.Id);

            // Assert
            Assert.IsNull(updatedPortfolio);
        }

        [TestMethod]
        public void DeletePortfolio_Returns_True_If_Portfolio_Deleted()
        {
            // Arrange
            Portfolio portfolio = new Portfolio
            {
                Name = "Test Portfolio",
                Location = "Test Location",
                ProfessionalSummary = "Test Professional Summary",
                Email = "Test Email"
            };

            Portfolio addedPortfolio = dao.CreatePortfolio(portfolio);

            // Act
            int deleteResult = dao.DeletePortfolio(addedPortfolio.Id);
            bool isDeleted = deleteResult == 1;

            // Assert
            Assert.IsTrue(isDeleted);
        }

        [TestMethod]
        public void DeletePortfolio_Returns_False_If_Portfolio_Does_Not_Exist()
        {
            // Act
            int deleteResult = dao.DeletePortfolio(999);
            bool isDeleted = deleteResult == 1;

            // Assert
            Assert.IsFalse(isDeleted);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            base.Cleanup(); 
        }


    }
}