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
                achievementDaoMock.Object);
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
            WorkExperience testWorkExperience1 = CreateAWorkExperienceTestObject1();
            WorkExperience testWorkExperience2 = CreateAWorkExperienceTestObject2();

            dao.CreateWorkExperienceByPortfolioId(1, testWorkExperience1);
            dao.CreateWorkExperienceByPortfolioId(1, testWorkExperience2);
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
            WorkExperience workExperienceTestObj = CreateAWorkExperienceTestObject1();
            WorkExperience createdWorkExperience = dao.CreateWorkExperienceByPortfolioId(1, workExperienceTestObj);
            SetUpWorkExperienceNestedDaoMockObjects();

            // Act
            WorkExperience workExperience = dao.GetWorkExperienceByPortfolioId(1, createdWorkExperience.Id);

            // Assert
            Assert.AreEqual(createdWorkExperience.PositionTitle, workExperience.PositionTitle);
            Assert.AreEqual(createdWorkExperience.CompanyName, workExperience.CompanyName);
            Assert.AreEqual(createdWorkExperience.StartDate, workExperience.StartDate);
            Assert.AreEqual(createdWorkExperience.EndDate, workExperience.EndDate);
            Assert.AreEqual(createdWorkExperience.Description, workExperience.Description);
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
            WorkExperience createdWorkExperience = dao.CreateWorkExperienceByPortfolioId(1, workExperience);

            // Assert
            Assert.AreEqual(workExperience.PositionTitle, createdWorkExperience.PositionTitle);
            Assert.AreEqual(workExperience.CompanyName, createdWorkExperience.CompanyName);
            Assert.AreEqual(workExperience.StartDate, createdWorkExperience.StartDate);
            Assert.AreEqual(workExperience.EndDate, createdWorkExperience.EndDate);
            Assert.AreEqual(workExperience.Description, createdWorkExperience.Description);
        }

        [TestMethod]
        public void CreateWorkExperienceByPortfolioId_Throws_Argument_Exception_When_Portfolio_Doesnt_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;

            WorkExperience workExperience = new WorkExperience
            {
                PositionTitle = "Software Engineer",
                CompanyName = "Microsoft",
                Location = "Redmond, WA",
                Description = "Worked on the Windows team",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2021, 1, 1)
            };

            imageDaoMock.Setup(m => m.GetImageByWorkExperienceId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            skillDaoMock.Setup(m => m.GetSkillsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Skill>());
            websiteDaoMock.Setup(m => m.GetWebsiteByWorkExperienceId(It.IsAny<int>())).Returns(new Website());
            achievementDaoMock.Setup(m => m.GetAchievementsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Achievement>());

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => 
                dao.CreateWorkExperienceByPortfolioId(nonExistentPortfolioId, workExperience),
                "PortfolioId must be greater than zero.");
        }

        [TestMethod]
        public void UpdateWorkExperienceByPortfolioId_Updates_Work_Experience()
        {
            // Arrange
            WorkExperience createdWorkExperience = dao.CreateWorkExperienceByPortfolioId(1, new WorkExperience
            {
                PositionTitle = "Software Engineer",
                CompanyName = "Microsoft",
                Location = "Redmond, WA",
                Description = "Worked on the Windows team",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2021, 1, 1)
            });

            int workExperienceId = createdWorkExperience.Id;

            WorkExperience updatedWorkExperience = new WorkExperience
            {
                Id = workExperienceId,
                PositionTitle = "Software Engineer",
                CompanyName = "Google",
                Location = "Mountain View, CA",
                Description = "Worked on the Android team",
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2022, 1, 1)
            };

            imageDaoMock.Setup(m => m.GetImageByWorkExperienceId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            skillDaoMock.Setup(m => m.GetSkillsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Skill>());
            websiteDaoMock.Setup(m => m.GetWebsiteByWorkExperienceId(It.IsAny<int>())).Returns(new Website());
            achievementDaoMock.Setup(m => m.GetAchievementsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Achievement>());

            // Act
            dao.UpdateWorkExperienceByPortfolioId(1, updatedWorkExperience.Id, updatedWorkExperience);

            // Assert
            WorkExperience workExperience = dao.GetWorkExperienceByPortfolioId(1, workExperienceId);
            Assert.AreEqual("Software Engineer", workExperience.PositionTitle);
            Assert.AreEqual("Google", workExperience.CompanyName);
            Assert.AreEqual(new DateTime(2021, 1, 1), workExperience.StartDate);
            Assert.AreEqual(new DateTime(2022, 1, 1), workExperience.EndDate);
            Assert.AreEqual("Worked on the Android team", workExperience.Description);
        }

        [TestMethod]
        public void UpdateWorkExperienceByPortfolioId_Throws_Argument_Exception_When_Portfolio_Doesnt_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;

            WorkExperience workExperience = new WorkExperience
            {
                PositionTitle = "Software Engineer",
                CompanyName = "Microsoft",
                Location = "Redmond, WA",
                Description = "Worked on the Windows team",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2021, 1, 1)
            };

            imageDaoMock.Setup(m => m.GetImageByWorkExperienceId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            skillDaoMock.Setup(m => m.GetSkillsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Skill>());
            websiteDaoMock.Setup(m => m.GetWebsiteByWorkExperienceId(It.IsAny<int>())).Returns(new Website());
            achievementDaoMock.Setup(m => m.GetAchievementsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Achievement>());

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                dao.UpdateWorkExperienceByPortfolioId(nonExistentPortfolioId, 1, workExperience),
                "PortfolioId must be greater than zero.");
        }

        [TestMethod]
        public void UpdateWorkExperienceByPortfolioId_Throws_Argument_Exception_When_Work_Experience_Doesnt_Exist()
        {
            // Arrange
            WorkExperience workExperience = new WorkExperience
            {
                Id = -1,
                PositionTitle = "Software Engineer",
                CompanyName = "Microsoft",
                Location = "Redmond, WA",
                Description = "Worked on the Windows team",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2021, 1, 1)
            };

            imageDaoMock.Setup(m => m.GetImageByWorkExperienceId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            skillDaoMock.Setup(m => m.GetSkillsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Skill>());
            websiteDaoMock.Setup(m => m.GetWebsiteByWorkExperienceId(It.IsAny<int>())).Returns(new Website());
            achievementDaoMock.Setup(m => m.GetAchievementsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Achievement>());

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                dao.UpdateWorkExperienceByPortfolioId(1, workExperience.Id, workExperience),
                "WorkExperienceId must be greater than zero.");
        }

        [TestMethod]
        public void DeleteWorkExperienceByPortfolioId_Deletes_Work_Experience()
        {
            // Arrange
            WorkExperience createdWorkExperience = dao.CreateWorkExperienceByPortfolioId(1, new WorkExperience
            {
                PositionTitle = "Software Engineer",
                CompanyName = "Microsoft",
                Location = "Redmond, WA",
                Description = "Worked on the Windows team",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2021, 1, 1)
            });

            int workExperienceId = createdWorkExperience.Id;

            imageDaoMock.Setup(m => m.GetImageByWorkExperienceId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            skillDaoMock.Setup(m => m.GetSkillsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Skill>());
            websiteDaoMock.Setup(m => m.GetWebsiteByWorkExperienceId(It.IsAny<int>())).Returns(new Website());
            achievementDaoMock.Setup(m => m.GetAchievementsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Achievement>());

            // Act
            dao.DeleteWorkExperienceByPortfolioId(1, workExperienceId);

            // Assert
            WorkExperience? workExperience = dao.GetWorkExperienceByPortfolioId(1, workExperienceId);
            Assert.IsNull(workExperience);
        }

        



    }
}