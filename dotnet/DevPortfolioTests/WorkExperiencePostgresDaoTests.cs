using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class WorkExperiencePostgresDaoTests : PostgresDaoTestBase
    {
        private WorkExperiencePostgresDao dao = null!;
        private Mock<IImageDao> imageDaoMock = null!;
        private Mock<ISkillDao> skillDaoMock = null!;
        private Mock<IWebsiteDao> websiteDaoMock = null!;
        private Mock<IAchievementDao> achievementDaoMock = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            base.Initialize();
            InitializeMocks();
            dao = CreateDaoWithMocks();
        }

        private void InitializeMocks()
        {
            imageDaoMock = new Mock<IImageDao>();
            skillDaoMock = new Mock<ISkillDao>();
            websiteDaoMock = new Mock<IWebsiteDao>();
            achievementDaoMock = new Mock<IAchievementDao>();
        }

        private WorkExperiencePostgresDao CreateDaoWithMocks()
        {
            return new WorkExperiencePostgresDao(
                TestConnectionString,
                imageDaoMock.Object,
                skillDaoMock.Object,
                websiteDaoMock.Object,
                achievementDaoMock.Object
            );
        }

// FIXME refactor all tests to cut back on repetitive code and use as a model for other test classes
        private WorkExperience CreateAWorkExperienceTestObject1()
        {
            return new WorkExperience
            {
                PositionTitle = "Software Engineer",
                CompanyName = "Microsoft",
                Location = "Redmond, WA",
                Description = "Worked on the Windows team",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2021, 1, 1)
            };
        }

        private WorkExperience CreateAWorkExperienceTestObject2()
        {
            return new WorkExperience
            {
                PositionTitle = "Software Engineer",
                CompanyName = "Google",
                Location = "Mountain View, CA",
                Description = "Worked on the Android team",
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2022, 1, 1)
            };
        }

        private void SetUpWorkExperienceNestedDaoMockObjects()
        {
            imageDaoMock.Setup(m => m.GetImageByWorkExperienceId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            skillDaoMock.Setup(m => m.GetSkillsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Skill>());
            websiteDaoMock.Setup(m => m.GetWebsiteByWorkExperienceId(It.IsAny<int>())).Returns(new Website());
            achievementDaoMock.Setup(m => m.GetAchievementsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Achievement>());
        }

        [TestMethod]
        public void GetWorkExperiences_Returns_All_Work_Experiences()
        {
            // Arrange
            int portfolioId = 1;
            WorkExperience testWorkExperience1 = CreateAWorkExperienceTestObject1();
            WorkExperience testWorkExperience2 = CreateAWorkExperienceTestObject2();

            dao.CreateWorkExperienceByPortfolioId(portfolioId, testWorkExperience1);
            dao.CreateWorkExperienceByPortfolioId(portfolioId, testWorkExperience2);
            SetUpWorkExperienceNestedDaoMockObjects();

            List<WorkExperience> workExperiences = dao.GetWorkExperiences();

            // Assert
            Assert.AreEqual(2, workExperiences.Count);
            Assert.AreEqual(testWorkExperience1.PositionTitle, workExperiences[0].PositionTitle);
            Assert.AreEqual(testWorkExperience1.CompanyName, workExperiences[0].CompanyName);
            Assert.AreEqual(testWorkExperience1.StartDate, workExperiences[0].StartDate);
            Assert.AreEqual(testWorkExperience1.EndDate, workExperiences[0].EndDate);
            Assert.AreEqual(testWorkExperience1.Description, workExperiences[0].Description);

            Assert.AreEqual(testWorkExperience2.PositionTitle, workExperiences[1].PositionTitle);
            Assert.AreEqual(testWorkExperience2.CompanyName, workExperiences[1].CompanyName);
            Assert.AreEqual(testWorkExperience2.StartDate, workExperiences[1].StartDate);
            Assert.AreEqual(testWorkExperience2.EndDate, workExperiences[1].EndDate);
            Assert.AreEqual(testWorkExperience2.Description, workExperiences[1].Description);
        }

        [TestMethod]
        public void GetWorkExperiences_Returns_Empty_List_When_No_Work_Experiences()
        {
            // Act
            List<WorkExperience> workExperiences = dao.GetWorkExperiences();

            // Assert
            Assert.AreEqual(0, workExperiences.Count);
        }

        [TestMethod]
        public void GetWorkExperienceByPortfolioId_Returns_The_Correct_Work_Experience()
        {
            // Arrange
            int portfolioId = 1;
            WorkExperience workExperienceOriginalObject = CreateAWorkExperienceTestObject1();
            WorkExperience createdOriginalWorkExperience = dao.CreateWorkExperienceByPortfolioId(1, workExperienceOriginalObject);
            SetUpWorkExperienceNestedDaoMockObjects();

            // Act
            WorkExperience workExperience = dao.GetWorkExperienceByPortfolioId(portfolioId, createdOriginalWorkExperience.Id);

            // Assert
            Assert.AreEqual(createdOriginalWorkExperience.PositionTitle, workExperience.PositionTitle);
            Assert.AreEqual(createdOriginalWorkExperience.CompanyName, workExperience.CompanyName);
            Assert.AreEqual(createdOriginalWorkExperience.StartDate, workExperience.StartDate);
            Assert.AreEqual(createdOriginalWorkExperience.EndDate, workExperience.EndDate);
            Assert.AreEqual(createdOriginalWorkExperience.Description, workExperience.Description);
        }

        [TestMethod]
        public void GetWorkExperienceByPortfolioId_Returns_Null_When_Work_Experience_Not_Found()
        {
            // Arrange
            int portfolioId = 1;
            int workExperienceId = 1;

            // Act
            WorkExperience? workExperience = dao.GetWorkExperienceByPortfolioId(portfolioId, workExperienceId);

            // Assert
            Assert.IsNull(workExperience);
        }

        [TestMethod]
        public void CreateWorkExperienceByPortfolioId_Creates_Work_Experience()
        {
            // Arrange
            WorkExperience workExperience = CreateAWorkExperienceTestObject1();

            SetUpWorkExperienceNestedDaoMockObjects();

            // Act
            WorkExperience createdOriginalWorkExperience = dao.CreateWorkExperienceByPortfolioId(1, workExperience);

            // Assert
            Assert.AreEqual(workExperience.PositionTitle, createdOriginalWorkExperience.PositionTitle);
            Assert.AreEqual(workExperience.CompanyName, createdOriginalWorkExperience.CompanyName);
            Assert.AreEqual(workExperience.StartDate, createdOriginalWorkExperience.StartDate);
            Assert.AreEqual(workExperience.EndDate, createdOriginalWorkExperience.EndDate);
            Assert.AreEqual(workExperience.Description, createdOriginalWorkExperience.Description);
        }

        [TestMethod]
        public void CreateWorkExperienceByPortfolioId_Throws_Argument_Exception_When_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;

            WorkExperience workExperience = CreateAWorkExperienceTestObject1();
            SetUpWorkExperienceNestedDaoMockObjects();

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => 
                dao.CreateWorkExperienceByPortfolioId(nonExistentPortfolioId, workExperience),
                "PortfolioId must be greater than zero."
            );
        }

        [TestMethod]
        public void UpdateWorkExperienceByPortfolioId_Updates_Work_Experience()
        {
            // Arrange
            int portfolioId = 1;

            WorkExperience workExperienceOriginalObject = CreateAWorkExperienceTestObject1();
            WorkExperience workExperienceUpdateObject = CreateAWorkExperienceTestObject2();

            WorkExperience createdOriginalWorkExperience = dao.CreateWorkExperienceByPortfolioId(portfolioId, workExperienceOriginalObject);

            SetUpWorkExperienceNestedDaoMockObjects();

            // Act
            WorkExperience updatedWorkExperience = dao.UpdateWorkExperienceByPortfolioId(portfolioId, createdOriginalWorkExperience.Id, workExperienceUpdateObject);

            // Assert
            WorkExperience workExperience = dao.GetWorkExperienceByPortfolioId(portfolioId, createdOriginalWorkExperience.Id);

            Assert.AreEqual(updatedWorkExperience.PositionTitle, workExperience.PositionTitle);
            Assert.AreEqual(updatedWorkExperience.CompanyName, workExperience.CompanyName);
            Assert.AreEqual(updatedWorkExperience.StartDate, workExperience.StartDate);
            Assert.AreEqual(updatedWorkExperience.EndDate, workExperience.EndDate);
            Assert.AreEqual(updatedWorkExperience.Description, workExperience.Description);
        }

        [TestMethod]
        public void UpdateWorkExperienceByPortfolioId_Throws_Argument_Exception_When_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;

            WorkExperience workExperience = CreateAWorkExperienceTestObject1();
            SetUpWorkExperienceNestedDaoMockObjects();

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                dao.UpdateWorkExperienceByPortfolioId(nonExistentPortfolioId, workExperience.Id, workExperience),
                "PortfolioId must be greater than zero.");
        }

        [TestMethod]
        public void UpdateWorkExperienceByPortfolioId_Throws_Argument_Exception_When_Work_Experience_Does_Not_Exist()
        {
            // Arrange
            int portfolioId = 1;
            WorkExperience workExperience = CreateAWorkExperienceTestObject1();

            SetUpWorkExperienceNestedDaoMockObjects();

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                dao.UpdateWorkExperienceByPortfolioId(portfolioId, workExperience.Id, workExperience),
                "WorkExperienceId must be greater than zero.");
        }

        [TestMethod]
        public void DeleteWorkExperienceByPortfolioId_Deletes_Work_Experience()
        {
            // Arrange
            int portfolioId = 1;

            WorkExperience workExperienceTestObject = CreateAWorkExperienceTestObject1();
            WorkExperience createdOriginalWorkExperience = dao.CreateWorkExperienceByPortfolioId(1, workExperienceTestObject);

            int workExperienceId = createdOriginalWorkExperience.Id;

            SetUpWorkExperienceNestedDaoMockObjects();

            // Act
            dao.DeleteWorkExperienceByPortfolioId(portfolioId, workExperienceId);

            // Assert
            WorkExperience? workExperience = dao.GetWorkExperienceByPortfolioId(portfolioId, workExperienceId);
            Assert.IsNull(workExperience);
        }

        [TestMethod]
        public void DeleteWorkExperienceByPortfolioId_Returns_Correct_Number_Of_Rows_Affected()
        {
            // Arrange
            int portfolioId = 1;
            int expectedRowsAffected = 1;

            WorkExperience workExperienceTestObject = CreateAWorkExperienceTestObject1();
            WorkExperience workExperience = dao.CreateWorkExperienceByPortfolioId(portfolioId, workExperienceTestObject);

            SetUpWorkExperienceNestedDaoMockObjects();

            // Act
            int rowsAffected = dao.DeleteWorkExperienceByPortfolioId(portfolioId, workExperience.Id);

            // Assert
            Assert.AreEqual(expectedRowsAffected, rowsAffected);
        }

        [TestMethod]
        public void DeleteWorkExperienceByPortfolioId_Throws_Argument_Exception_When_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;
            int workExperienceId = 1;

            WorkExperience workExperience = CreateAWorkExperienceTestObject1();

            SetUpWorkExperienceNestedDaoMockObjects();

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                dao.DeleteWorkExperienceByPortfolioId(nonExistentPortfolioId, workExperienceId),
                "PortfolioId must be greater than zero.");
        }



    }
}