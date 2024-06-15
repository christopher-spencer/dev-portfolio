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
            return new VolunteerWork
            {
                OrganizationName = "Test Organization 1",
                Location = "Test Location 1",
                OrganizationDescription = "Test Description 1",
                PositionTitle = "Test Position 1",
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2021, 2, 1)
            };
        }

        private VolunteerWork CreateAVolunteerWorkTestObject2()
        {
            return new VolunteerWork
            {
                OrganizationName = "Test Organization 2",
                Location = "Test Location 2",
                OrganizationDescription = "Test Description 2",
                PositionTitle = "Test Position 2",
                StartDate = new DateTime(2021, 3, 1),
                EndDate = new DateTime(2021, 4, 1)
            };
        }

        private void SetUpVolunteerWorkDaoMockObjects()
        {
            mockImageDao.Setup(m => m.GetImageByVolunteerWorkId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            mockImageDao.Setup(m => m.GetAdditionalImagesByVolunteerWorkId(It.IsAny<int>())).Returns(new List<Image>());
            mockWebsiteDao.Setup(m => m.GetWebsiteByVolunteerWorkId(It.IsAny<int>())).Returns(new Website());
            mockAchievementDao.Setup(m => m.GetAchievementsByVolunteerWorkId(It.IsAny<int>())).Returns(new List<Achievement>());
            mockSkillDao.Setup(m => m.GetSkillsByVolunteerWorkId(It.IsAny<int>())).Returns(new List<Skill>());
        }

        // TODO Add Unit Tests for VolunteerWorkPostgresDAOTests

        [TestMethod]
        public void GetVolunteerWorks_Returns_All_Volunteer_Works()
        {
            // Arrange
            int portfolioId = 1;
            VolunteerWork volunteerWork1 = CreateAVolunteerWorkTestObject1();
            VolunteerWork volunteerWork2 = CreateAVolunteerWorkTestObject2();

            dao.CreateVolunteerWorkByPortfolioId(portfolioId, volunteerWork1);
            dao.CreateVolunteerWorkByPortfolioId(portfolioId, volunteerWork2);
            SetUpVolunteerWorkDaoMockObjects();

            // Act
            List<VolunteerWork> volunteerWorks = dao.GetVolunteerWorks();

            // Assert
            Assert.AreEqual(2, volunteerWorks.Count);
            Assert.AreEqual(volunteerWork1.OrganizationName, volunteerWorks[0].OrganizationName);
            Assert.AreEqual(volunteerWork1.Location, volunteerWorks[0].Location);
            Assert.AreEqual(volunteerWork1.OrganizationDescription, volunteerWorks[0].OrganizationDescription);
            Assert.AreEqual(volunteerWork1.PositionTitle, volunteerWorks[0].PositionTitle);
            Assert.AreEqual(volunteerWork1.StartDate, volunteerWorks[0].StartDate);
            Assert.AreEqual(volunteerWork1.EndDate, volunteerWorks[0].EndDate);

            Assert.AreEqual(volunteerWork2.OrganizationName, volunteerWorks[1].OrganizationName);
            Assert.AreEqual(volunteerWork2.Location, volunteerWorks[1].Location);
            Assert.AreEqual(volunteerWork2.OrganizationDescription, volunteerWorks[1].OrganizationDescription);
            Assert.AreEqual(volunteerWork2.PositionTitle, volunteerWorks[1].PositionTitle);
            Assert.AreEqual(volunteerWork2.StartDate, volunteerWorks[1].StartDate);
            Assert.AreEqual(volunteerWork2.EndDate, volunteerWorks[1].EndDate);
        }

        [TestMethod]
        public void GetVolunteerWorks_Returns_Empty_List_When_No_Volunteer_Works()
        {
            // Act
            List<VolunteerWork> volunteerWorks = dao.GetVolunteerWorks();

            // Assert
            Assert.AreEqual(0, volunteerWorks.Count);
        }

        [TestMethod]
        public void GetVolunteerWorkByPortfolioId_Returns_Correct_Volunteer_Work()
        {
            // Arrange
            int portfolioId = 1;
            VolunteerWork volunteerWorkTestObject = CreateAVolunteerWorkTestObject1();
            VolunteerWork createdVolunteerWork = dao.CreateVolunteerWorkByPortfolioId(portfolioId, volunteerWorkTestObject);
            SetUpVolunteerWorkDaoMockObjects();

            // Act
            VolunteerWork volunteerWork = dao.GetVolunteerWorkByPortfolioId(portfolioId, createdVolunteerWork.Id);

            // Assert
            Assert.AreEqual(createdVolunteerWork.OrganizationName, volunteerWork.OrganizationName);
            Assert.AreEqual(createdVolunteerWork.Location, volunteerWork.Location);
            Assert.AreEqual(createdVolunteerWork.OrganizationDescription, volunteerWork.OrganizationDescription);
            Assert.AreEqual(createdVolunteerWork.PositionTitle, volunteerWork.PositionTitle);
            Assert.AreEqual(createdVolunteerWork.StartDate, volunteerWork.StartDate);
            Assert.AreEqual(createdVolunteerWork.EndDate, volunteerWork.EndDate);
        }




    }
}