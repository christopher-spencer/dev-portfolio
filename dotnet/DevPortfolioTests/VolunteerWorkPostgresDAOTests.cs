using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class VolunteerWorkPostgresDAOTests : PostgresDaoTestBase
    {

        private VolunteerWorkPostgresDao dao = null!;
        private Mock<IImageDao> mockImageDao = null!;
        private Mock<IWebsiteDao> mockWebsiteDao = null!;
        private Mock<IAchievementDao> mockAchievementDao = null!;
        private Mock<ISkillDao> mockSkillDao = null!;

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

        private VolunteerWorkPostgresDao CreateDaoWithMocks()
        {
            return new VolunteerWorkPostgresDao(
                TestConnectionString, 
                mockImageDao.Object, 
                mockWebsiteDao.Object, 
                mockAchievementDao.Object, 
                mockSkillDao.Object
            );
        }

        private VolunteerWork CreateAVolunteerWorkTestObject1()
        {
            OrganizationName = "Test Organization 1",
            Location = "Test Location 1",
            OrganizationDescription = "Test Description 1",
            PositionTitle = "Test Position 1",
            StartDate = new DateTime(2021, 1, 1),
            EndDate = new DateTime(2021, 2, 1)
        }

        private VolunteerWork CreateAVolunteerWorkTestObject2()
        {
            OrganizationName = "Test Organization 2",
            Location = "Test Location 2",
            OrganizationDescription = "Test Description 2",
            PositionTitle = "Test Position 2",
            StartDate = new DateTime(2021, 3, 1),
            EndDate = new DateTime(2021, 4, 1)
        }

        private void SetUpVolunteerWorkDaoMockObjects()
        {
            mockImageDao.Setup(m => m.GetImageByVolunteerWorkId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            mockImageDao.Setup(m => m.GetAdditionalImagesByVolunteerWorkId(It.IsAny<int>())).Returns(new Image());
            mockWebsiteDao.Setup(m => m.GetWebsiteByVolunteerWorkId(It.IsAny<int>())).Returns(new Website());
            mockAchievementDao.Setup(m => m.GetAchievementsByVolunteerWorkId(It.IsAny<int>())).Returns(new List<Achievement>());
            mockSkillDao.Setup(m => m.GetSkillsByVolunteerWorkId(It.IsAny<int>())).Returns(new List<Skill>());
        }

        // TODO Add Unit Tests for VolunteerWorkPostgresDAOTests
    }
}