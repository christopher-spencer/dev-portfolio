using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class AchievementPostgresDaoTests : PostgresDaoTestBase
    {
        private AchievementPostgresDao dao = null!;
        private Mock<IImageDao> imageDaoMock = null!;

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
        }

        private AchievementPostgresDao CreateDaoWithMocks()
        {
            return new AchievementPostgresDao(
                TestConnectionString,
                imageDaoMock.Object);
        }

        private Achievement CreateAWorkExperienceAchievementTestObject()
        {
            return new Achievement
            {
                Description = "Test work experience achievement"
            };
        }

        private Achievement CreateAnEducationAchievementTestObject()
        {
            return new Achievement
            {
                Description = "Test education achievement"
            };
        }

        private Achievement CreateAnOpenSourceContributionAchievementTestObject()
        {
            return new Achievement
            {
                Description = "Test open source contribution achievement"
            };
        }

        private Achievement CreateAVolunteerWorkAchievementTestObject()
        {
            return new Achievement
            {
                Description = "Test volunteer work achievement"
            };
        }

        private void SetupAchievementNestedDaoMockObjects()
        {
            imageDaoMock.Setup(m => m.GetImageByAchievementId(It.IsAny<int>())).Returns(new Image());
        }

// TODO add Unit Tests for AchievementPostgresDAOTests
// FIXME need to build WorkExperience, Education, OpenSourceContribution, and VolunteerWork test data (which all hold achievements)
        [TestMethod]
        public void GetAchievements_Returns_All_Achievements()
        {
            // Arrange
            int testId = 1;

            Achievement workExperienceAchievement = CreateAWorkExperienceAchievementTestObject();
            Achievement educationAchievement = CreateAnEducationAchievementTestObject();
            Achievement openSourceContributionAchievement = CreateAnOpenSourceContributionAchievementTestObject();
            Achievement volunteerWorkAchievement = CreateAVolunteerWorkAchievementTestObject();

            dao.CreateAchievementByWorkExperienceId(testId, workExperienceAchievement);
            dao.CreateAchievementByEducationId(testId, educationAchievement);
            dao.CreateAchievementByOpenSourceContributionId(testId, openSourceContributionAchievement);
            dao.CreateAchievementByVolunteerWorkId(testId, volunteerWorkAchievement);

            SetupAchievementNestedDaoMockObjects();
            
            List<Achievement> achievements = dao.GetAchievements();

            // Assert
            Assert.AreEqual(4, achievements.Count);
            
        }

    }
}