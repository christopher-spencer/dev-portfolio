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
// FIXME need to build WorkExperience, Education, OpenSourceContribution, and VolunteerWork test data (which all hold achievements)
        // [TestMethod]
        // public void GetAchievements_Returns_All_Achievements()
        // {
        //     // Arrange
        //     dao.CreateAchievementByPortfolioId(1, new Achievement
        //     {
        //         Description = "This is the first achievement",
        //         IconId = 1
        //     });
        //     dao.CreateAchievementByPortfolioId(1, new Achievement
        //     {
        //         Description = "This is the second achievement",
        //         IconId = 2
        //     });

        //     imageDaoMock.Setup(m => m.GetImage(It.IsAny<int>())).Returns(new Image());
            
        //     List<Achievement> achievements = dao.GetAchievements();

        //     // Assert
        //     Assert.AreEqual(2, achievements.Count);
        //     Assert.AreEqual("This is the first achievement", achievements[0].Description);
        //     Assert.AreEqual("This is the second achievement", achievements[1].Description);
        // }

    }
}