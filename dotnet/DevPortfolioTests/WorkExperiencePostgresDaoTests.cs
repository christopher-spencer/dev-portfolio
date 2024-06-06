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

        [TestMethod]
        public void GetWorkExperiences_Returns_All_WorkExperiences()
        {
            // Arrange
            dao.CreateWorkExperienceByPortfolioId(1, new WorkExperience
            {
                PositionTitle = "Software Engineer",
                CompanyName = "Microsoft",
                Location = "Redmond, WA",
                Description = "Worked on the Windows team",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2021, 1, 1)
            });
            dao.CreateWorkExperienceByPortfolioId(1, new WorkExperience
            {
                PositionTitle = "Software Engineer",
                CompanyName = "Google",
                Location = "Mountain View, CA",
                Description = "Worked on the Android team",
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2022, 1, 1)
            });
// FIXME add to other tests as needed
            imageDaoMock.Setup(m => m.GetImageByWorkExperienceId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            skillDaoMock.Setup(m => m.GetSkillsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Skill>());
            websiteDaoMock.Setup(m => m.GetWebsiteByWorkExperienceId(It.IsAny<int>())).Returns(new Website());
            achievementDaoMock.Setup(m => m.GetAchievementsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Achievement>());

            List<WorkExperience> workExperiences = dao.GetWorkExperiences();

            // Assert
            Assert.AreEqual(2, workExperiences.Count);
            Assert.AreEqual("Software Engineer", workExperiences[0].PositionTitle);
            Assert.AreEqual("Microsoft", workExperiences[0].CompanyName);
            Assert.AreEqual(new DateTime(2020, 1, 1), workExperiences[0].StartDate);
            Assert.AreEqual(new DateTime(2021, 1, 1), workExperiences[0].EndDate);
            Assert.AreEqual("Worked on the Windows team", workExperiences[0].Description);
            Assert.AreEqual("Software Engineer", workExperiences[1].PositionTitle);
            Assert.AreEqual("Google", workExperiences[1].CompanyName);
            Assert.AreEqual(new DateTime(2021, 1, 1), workExperiences[1].StartDate);
            Assert.AreEqual(new DateTime(2022, 1, 1), workExperiences[1].EndDate);
            Assert.AreEqual("Worked on the Android team", workExperiences[1].Description);
        }

        [TestMethod]
        public void GetWorkExperiences_Returns_Empty_List_When_No_Work_Experiences()
        {
            // Arrange
            imageDaoMock.Setup(m => m.GetImageByWorkExperienceId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            skillDaoMock.Setup(m => m.GetSkillsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Skill>());
            websiteDaoMock.Setup(m => m.GetWebsiteByWorkExperienceId(It.IsAny<int>())).Returns(new Website());
            achievementDaoMock.Setup(m => m.GetAchievementsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Achievement>());

            // Act
            List<WorkExperience> workExperiences = dao.GetWorkExperiences();

            // Assert
            Assert.AreEqual(0, workExperiences.Count);
        }

        [TestMethod]
        public void GetWorkExperienceByPortfolioId_Returns_The_Correct_Work_Experience()
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
            WorkExperience workExperience = dao.GetWorkExperienceByPortfolioId(1, workExperienceId);

            // Assert
            Assert.AreEqual("Software Engineer", workExperience.PositionTitle);
            Assert.AreEqual("Microsoft", workExperience.CompanyName);
            Assert.AreEqual(new DateTime(2020, 1, 1), workExperience.StartDate);
            Assert.AreEqual(new DateTime(2021, 1, 1), workExperience.EndDate);
            Assert.AreEqual("Worked on the Windows team", workExperience.Description);
        }

        [TestMethod]
        public void GetWorkExperienceByPortfolioId_Returns_Null_When_Work_Experience_Not_Found()
        {
            // Arrange
            imageDaoMock.Setup(m => m.GetImageByWorkExperienceId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            skillDaoMock.Setup(m => m.GetSkillsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Skill>());
            websiteDaoMock.Setup(m => m.GetWebsiteByWorkExperienceId(It.IsAny<int>())).Returns(new Website());
            achievementDaoMock.Setup(m => m.GetAchievementsByWorkExperienceId(It.IsAny<int>())).Returns(new List<Achievement>());

            // Act
            WorkExperience? workExperience = dao.GetWorkExperienceByPortfolioId(1, 1);

            // Assert
            Assert.IsNull(workExperience);
        }

        [TestMethod]
        public void CreateWorkExperienceByPortfolioId_Creates_Work_Experience()
        {
            // Arrange
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

            // Act
            WorkExperience createdWorkExperience = dao.CreateWorkExperienceByPortfolioId(1, workExperience);

            // Assert
            Assert.AreEqual("Software Engineer", createdWorkExperience.PositionTitle);
            Assert.AreEqual("Microsoft", createdWorkExperience.CompanyName);
            Assert.AreEqual(new DateTime(2020, 1, 1), createdWorkExperience.StartDate);
            Assert.AreEqual(new DateTime(2021, 1, 1), createdWorkExperience.EndDate);
            Assert.AreEqual("Worked on the Windows team", createdWorkExperience.Description);
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

        



    }
}