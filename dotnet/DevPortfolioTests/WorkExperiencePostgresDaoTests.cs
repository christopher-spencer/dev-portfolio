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
        private MOck<IAchievementDao> achievementDaoMock = null!;

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


    }
}