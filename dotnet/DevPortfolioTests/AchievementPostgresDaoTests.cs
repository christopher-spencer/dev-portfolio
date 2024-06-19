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

        private Achievement CreateAWorkExperienceAchievementTestObject1()
        {
            return new Achievement
            {
                Description = "Test work experience achievement 1"
            };
        }

        private Achievement CreateAWorkExperienceAchievementTestObject2()
        {
            return new Achievement
            {
                Description = "Test work experience achievement 2"
            };
        }

        private Achievement CreateAnEducationAchievementTestObject1()
        {
            return new Achievement
            {
                Description = "Test education achievement 1"
            };
        }

        private Achievement CreateAnEducationAchievementTestObject2()
        {
            return new Achievement
            {
                Description = "Test education achievement 2"
            };
        }

        private Achievement CreateAnOpenSourceContributionAchievementTestObject1()
        {
            return new Achievement
            {
                Description = "Test open source contribution achievement 1"
            };
        }

        private Achievement CreateAnOpenSourceContributionAchievementTestObject2()
        {
            return new Achievement
            {
                Description = "Test open source contribution achievement 2"
            };
        }

        private Achievement CreateAVolunteerWorkAchievementTestObject1()
        {
            return new Achievement
            {
                Description = "Test volunteer work achievement 1"
            };
        }

        private Achievement CreateAVolunteerWorkAchievementTestObject2()
        {
            return new Achievement
            {
                Description = "Test volunteer work achievement 2"
            };
        }

        private void SetupAchievementNestedDaoMockObjects()
        {
            imageDaoMock.Setup(m => m.GetImageByAchievementId(It.IsAny<int>())).Returns(new Image());
        }

// FIXME need to build WorkExperience, Education, OpenSourceContribution, and VolunteerWork test data (which all hold achievements)
        [TestMethod]
        public void GetAchievements_Returns_All_Achievements()
        {
            // Arrange
            int testId = 1;

            Achievement workExperienceAchievement1 = CreateAWorkExperienceAchievementTestObject1();
            Achievement workExperienceAchievement2 = CreateAWorkExperienceAchievementTestObject2();

            Achievement educationAchievement1 = CreateAnEducationAchievementTestObject1();
            Achievement educationAchievement2 = CreateAnEducationAchievementTestObject2();

            Achievement openSourceContributionAchievement1 = CreateAnOpenSourceContributionAchievementTestObject1();
            Achievement openSourceContributionAchievement2 = CreateAnOpenSourceContributionAchievementTestObject2();
            
            Achievement volunteerWorkAchievement1 = CreateAVolunteerWorkAchievementTestObject1();
            Achievement volunteerWorkAchievement2 = CreateAVolunteerWorkAchievementTestObject2();

            dao.CreateAchievementByWorkExperienceId(testId, workExperienceAchievement1);
            dao.CreateAchievementByWorkExperienceId(testId, workExperienceAchievement2);

            dao.CreateAchievementByEducationId(testId, educationAchievement1);
            dao.CreateAchievementByEducationId(testId, educationAchievement2);

            dao.CreateAchievementByOpenSourceContributionId(testId, openSourceContributionAchievement1);
            dao.CreateAchievementByOpenSourceContributionId(testId, openSourceContributionAchievement2);

            dao.CreateAchievementByVolunteerWorkId(testId, volunteerWorkAchievement1);
            dao.CreateAchievementByVolunteerWorkId(testId, volunteerWorkAchievement2);

            SetupAchievementNestedDaoMockObjects();
            
            // Act
            List<Achievement> achievements = dao.GetAchievements();            

            // Assert
            Assert.AreEqual(8, achievements.Count);

            Assert.AreEqual(workExperienceAchievement1.Description, achievements[0].Description);
            Assert.AreEqual(workExperienceAchievement2.Description, achievements[1].Description);

            Assert.AreEqual(educationAchievement1.Description, achievements[2].Description);
            Assert.AreEqual(educationAchievement2.Description, achievements[3].Description);

            Assert.AreEqual(openSourceContributionAchievement1.Description, achievements[4].Description);
            Assert.AreEqual(openSourceContributionAchievement2.Description, achievements[5].Description);

            Assert.AreEqual(volunteerWorkAchievement1.Description, achievements[6].Description);
            Assert.AreEqual(volunteerWorkAchievement2.Description, achievements[7].Description);
        }

    }
}