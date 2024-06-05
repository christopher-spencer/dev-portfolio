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


    }
}