using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class OpenSourceContributionPostgresDAOTests : PostgresDaoTestBase
    {
        private OpenSourceContributionPostgresDao dao = null!;
        private Mock<IImageDao> mockImageDao = null!;
        private Mock<IWebsiteDao> mockWebsiteDao = null!;
        private Mock<ISkillDao> mockSkillDao = null!;
        private Mock<IAchievementDao> mockAchievementDao = null!;

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
            mockAchievementDao = new Mock<IAchievementDao>();
            mockSkillDao = new Mock<ISkillDao>();
        }

        private OpenSourceContributionPostgresDao CreateDaoWithMocks()
        {
            return new OpenSourceContributionPostgresDao(
                TestConnectionString, 
                mockImageDao.Object, 
                mockWebsiteDao.Object, 
                mockAchievementDao.Object,
                mockSkillDao.Object
            );
        }   

        private OpenSourceContribution CreateAnOpenSourceContributionTestObject1()
        {
            return new OpenSourceContribution
            {
                ProjectName = "Test Project Name",
                OrganizationName = "Test Organization Name",
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2021, 1, 31),
                ProjectDescription = "Test Project Description",
                ContributionDetails = "Test Contribution Details",
            };
        }

        private OpenSourceContribution CreateAnOpenSourceContributionTestObject2()
        {
            return new OpenSourceContribution
            {
                ProjectName = "Test Project Name 2",
                OrganizationName = "Test Organization Name 2",
                StartDate = new DateTime(2021, 2, 1),
                EndDate = new DateTime(2021, 2, 28),
                ProjectDescription = "Test Project Description 2",
                ContributionDetails = "Test Contribution Details 2",
            };
        }

        private void SetUpOpenSourceContributionNestedDaoMockObjects()
        {
            mockImageDao.Setup(d => d.GetImageByOpenSourceContributionId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            mockImageDao.Setup(d => d.GetAdditionalImagesByOpenSourceContributionId(It.IsAny<int>())).Returns(new List<Image>());
            mockWebsiteDao.Setup(d => d.GetWebsiteByOpenSourceContributionId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Website());
            mockAchievementDao.Setup(d => d.GetAchievementsByOpenSourceContributionId(It.IsAny<int>())).Returns(new List<Achievement>());
            mockSkillDao.Setup(d => d.GetSkillsByOpenSourceContributionId(It.IsAny<int>())).Returns(new List<Skill>());
        }

        [TestMethod]
        public void GetOpenSourceContributions_Returns_All_Contributions()
        {
            // Arrange
            int portfolioId = 1;
            OpenSourceContribution testOpenSourceContribution1 = CreateAnOpenSourceContributionTestObject1();
            OpenSourceContribution testOpenSourceContribution2 = CreateAnOpenSourceContributionTestObject2();

            dao.CreateOpenSourceContributionByPortfolioId(portfolioId, testOpenSourceContribution1);
            dao.CreateOpenSourceContributionByPortfolioId(portfolioId, testOpenSourceContribution2);
            SetUpOpenSourceContributionNestedDaoMockObjects();

            // Act
            List<OpenSourceContribution> contributions = dao.GetOpenSourceContributions();

            // Assert
            Assert.AreEqual(2, contributions.Count);
            Assert.AreEqual(testOpenSourceContribution1.ProjectName, contributions[0].ProjectName);
            Assert.AreEqual(testOpenSourceContribution1.OrganizationName, contributions[0].OrganizationName);
            Assert.AreEqual(testOpenSourceContribution1.StartDate, contributions[0].StartDate);
            Assert.AreEqual(testOpenSourceContribution1.EndDate, contributions[0].EndDate);
            Assert.AreEqual(testOpenSourceContribution1.ProjectDescription, contributions[0].ProjectDescription);
            Assert.AreEqual(testOpenSourceContribution1.ContributionDetails, contributions[0].ContributionDetails);

            Assert.AreEqual(testOpenSourceContribution2.ProjectName, contributions[1].ProjectName);
            Assert.AreEqual(testOpenSourceContribution2.OrganizationName, contributions[1].OrganizationName);
            Assert.AreEqual(testOpenSourceContribution2.StartDate, contributions[1].StartDate);
            Assert.AreEqual(testOpenSourceContribution2.EndDate, contributions[1].EndDate);
            Assert.AreEqual(testOpenSourceContribution2.ProjectDescription, contributions[1].ProjectDescription);
            Assert.AreEqual(testOpenSourceContribution2.ContributionDetails, contributions[1].ContributionDetails);
        }

        [TestMethod]
        public void GetOpenSourceContributions_Returns_Empty_List_When_No_Contributions()
        {
            // Act
            List<OpenSourceContribution> contributions = dao.GetOpenSourceContributions();

            // Assert
            Assert.AreEqual(0, contributions.Count);
        }

        [TestMethod]
        public void GetOpenSourceContributionById_Returns_Correct_Contribution()
        {
            // Arrange
            int portfolioId = 1;
            OpenSourceContribution testOpenSourceContribution1 = CreateAnOpenSourceContributionTestObject1();
            OpenSourceContribution createdOpenSourceContribution = dao.CreateOpenSourceContributionByPortfolioId(portfolioId, testOpenSourceContribution1);
            SetUpOpenSourceContributionNestedDaoMockObjects();

            // Act
            OpenSourceContribution contribution = dao.GetOpenSourceContributionByPortfolioId(portfolioId, createdOpenSourceContribution.Id);

            // Assert
            Assert.AreEqual(createdOpenSourceContribution.ProjectName, contribution.ProjectName);
            Assert.AreEqual(createdOpenSourceContribution.OrganizationName, contribution.OrganizationName);
            Assert.AreEqual(createdOpenSourceContribution.StartDate, contribution.StartDate);
            Assert.AreEqual(createdOpenSourceContribution.EndDate, contribution.EndDate);
            Assert.AreEqual(createdOpenSourceContribution.ProjectDescription, contribution.ProjectDescription);
            Assert.AreEqual(createdOpenSourceContribution.ContributionDetails, contribution.ContributionDetails);
        }

        [TestMethod]
        public void GetOpenSourceContributionById_Returns_Null_When_Not_Found()
        {
            // Arrange
            int portfolioId = 1;
            int contributionId = 1;       

            // Act
            OpenSourceContribution? contribution = dao.GetOpenSourceContributionByPortfolioId(portfolioId, contributionId);

            // Assert
            Assert.IsNull(contribution);
        }

        [TestMethod]
        public void CreateOpenSourceContribution_Returns_Created_Contribution()
        {
            // Arrange
            int portfolioId = 1;
            OpenSourceContribution openSourceContribution = CreateAnOpenSourceContributionTestObject1();
            SetUpOpenSourceContributionNestedDaoMockObjects();

            // Act
            OpenSourceContribution createdOpenSourceContribution = dao.CreateOpenSourceContributionByPortfolioId(portfolioId, openSourceContribution);

            // Assert
            Assert.AreEqual(openSourceContribution.ProjectName, createdOpenSourceContribution.ProjectName);
            Assert.AreEqual(openSourceContribution.OrganizationName, createdOpenSourceContribution.OrganizationName);
            Assert.AreEqual(openSourceContribution.StartDate, createdOpenSourceContribution.StartDate);
            Assert.AreEqual(openSourceContribution.EndDate, createdOpenSourceContribution.EndDate);
            Assert.AreEqual(openSourceContribution.ProjectDescription, createdOpenSourceContribution.ProjectDescription);
            Assert.AreEqual(openSourceContribution.ContributionDetails, createdOpenSourceContribution.ContributionDetails);
        }

        [TestMethod]
        public void CreateOpenSourceContributionByPortfolioId_Throws_Argument_Exception_When_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;
            OpenSourceContribution openSourceContribution = CreateAnOpenSourceContributionTestObject1();
            SetUpOpenSourceContributionNestedDaoMockObjects();

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => 
                dao.CreateOpenSourceContributionByPortfolioId(nonExistentPortfolioId, openSourceContribution),
                "PortfolioId must be greater than zero.");
        }

        [TestMethod]
        public void UpdateOpenSourceContribution_Returns_Updated_Contribution()
        {
            // Arrange
            int portfolioId = 1;

            OpenSourceContribution openSourceContributionOriginalObject = CreateAnOpenSourceContributionTestObject1();
            OpenSourceContribution openSourceContributionUpdatedObject = CreateAnOpenSourceContributionTestObject2();


            OpenSourceContribution createdOpenSourceContribution = dao.CreateOpenSourceContributionByPortfolioId(portfolioId, openSourceContributionOriginalObject);
            
            SetUpOpenSourceContributionNestedDaoMockObjects();

            // Act
            OpenSourceContribution updatedContribution = dao.UpdateOpenSourceContributionByPortfolioId(portfolioId, openSourceContributionOriginalObject.Id, openSourceContributionUpdatedObject);

            // Assert
            OpenSourceContribution contribution = dao.GetOpenSourceContributionByPortfolioId(portfolioId, createdOpenSourceContribution.Id);

            Assert.AreEqual(updatedContribution.ProjectName, contribution.ProjectName);
            Assert.AreEqual(updatedContribution.OrganizationName, contribution.OrganizationName);
            Assert.AreEqual(updatedContribution.StartDate, contribution.StartDate);
            Assert.AreEqual(updatedContribution.EndDate, contribution.EndDate);
            Assert.AreEqual(updatedContribution.ProjectDescription, contribution.ProjectDescription);
            Assert.AreEqual(updatedContribution.ContributionDetails, contribution.ContributionDetails);
        }

        [TestMethod]
        public void UpdateOpenSourceContributionByPortfolioId_Throws_Argument_Exception_When_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;

            OpenSourceContribution openSourceContribution = CreateAnOpenSourceContributionTestObject1();
            SetUpOpenSourceContributionNestedDaoMockObjects();

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => 
                dao.UpdateOpenSourceContributionByPortfolioId(nonExistentPortfolioId, openSourceContribution.Id, openSourceContribution),
                "PortfolioId must be greater than zero.");
        }

        [TestMethod]
        public void UpdateOpenSourceContributionByPortfolioId_Throws_Argument_Exception_When_Contribution_Does_Not_Exist()
        {
            // Arrange
            int portfolioId = 1;
            int nonExistentContributionId = -1;

            OpenSourceContribution openSourceContribution = CreateAnOpenSourceContributionTestObject1();
            SetUpOpenSourceContributionNestedDaoMockObjects();

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => 
                dao.UpdateOpenSourceContributionByPortfolioId(portfolioId, nonExistentContributionId, openSourceContribution),
                "ContributionId must be greater than zero.");
        }




    }
}