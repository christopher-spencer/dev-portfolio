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

        [TestMethod]
        public void GetVolunteerWorkByPortfolioId_Returns_Null_When_Volunteer_Work_Not_Found()
        {
            // Arrange
            int portfolioId = 1;
            int volunteerWorkId = 1;

            // Act
            VolunteerWork? volunteerWork = dao.GetVolunteerWorkByPortfolioId(portfolioId, volunteerWorkId);

            // Assert
            Assert.IsNull(volunteerWork);
        }

        [TestMethod]
        public void CreateVolunteerWorkByPortfolioId_Returns_Created_Volunteer_Work()
        {
            // Arrange
            int portfolioId = 1;
            VolunteerWork volunteerWorkTestObject = CreateAVolunteerWorkTestObject1();
            SetUpVolunteerWorkDaoMockObjects();

            // Act
            VolunteerWork createdVolunteerWork = dao.CreateVolunteerWorkByPortfolioId(portfolioId, volunteerWorkTestObject);

            // Assert
            Assert.AreEqual(volunteerWorkTestObject.OrganizationName, createdVolunteerWork.OrganizationName);
            Assert.AreEqual(volunteerWorkTestObject.Location, createdVolunteerWork.Location);
            Assert.AreEqual(volunteerWorkTestObject.OrganizationDescription, createdVolunteerWork.OrganizationDescription);
            Assert.AreEqual(volunteerWorkTestObject.PositionTitle, createdVolunteerWork.PositionTitle);
            Assert.AreEqual(volunteerWorkTestObject.StartDate, createdVolunteerWork.StartDate);
            Assert.AreEqual(volunteerWorkTestObject.EndDate, createdVolunteerWork.EndDate);
        }

        [TestMethod]
        public void CreateVolunteerWorkByPortfolioId_Throws_Argument_Exception_When_A_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;
            VolunteerWork volunteerWorkTestObject = CreateAVolunteerWorkTestObject1();
            SetUpVolunteerWorkDaoMockObjects();

            // Act
            Assert.ThrowsException<ArgumentException>(() => 
                dao.CreateVolunteerWorkByPortfolioId(nonExistentPortfolioId, volunteerWorkTestObject),
                "PortfolioId must be greater than zero.");
        }

        [TestMethod]
        public void UpdateVolunteerWorkByPortfolioId_Returns_Updated_Volunteer_Work()
        {
            // Arrange
            int portfolioId = 1;
            VolunteerWork volunteerWorkTestObject = CreateAVolunteerWorkTestObject1();
            VolunteerWork volunteerWorkUpdatedObject = CreateAVolunteerWorkTestObject2(); 

            VolunteerWork createdVolunteerWork = dao.CreateVolunteerWorkByPortfolioId(portfolioId, volunteerWorkTestObject);
           
            SetUpVolunteerWorkDaoMockObjects();

            // Act
            VolunteerWork updatedVolunteerWork = dao.UpdateVolunteerWorkByPortfolioId(portfolioId, createdVolunteerWork.Id, volunteerWorkUpdatedObject);

            VolunteerWork volunteerWork = dao.GetVolunteerWorkByPortfolioId(portfolioId, createdVolunteerWork.Id);

            // Assert
            Assert.AreEqual(updatedVolunteerWork.OrganizationName, volunteerWork.OrganizationName);
            Assert.AreEqual(updatedVolunteerWork.Location, volunteerWork.Location);
            Assert.AreEqual(updatedVolunteerWork.OrganizationDescription, volunteerWork.OrganizationDescription);
            Assert.AreEqual(updatedVolunteerWork.PositionTitle, volunteerWork.PositionTitle);
            Assert.AreEqual(updatedVolunteerWork.StartDate, volunteerWork.StartDate);
            Assert.AreEqual(updatedVolunteerWork.EndDate, volunteerWork.EndDate);
        }

        [TestMethod]
        public void UpdateVolunteerWorkByPortfolioId_Throws_Argument_Exception_When_A_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;

            VolunteerWork volunteerWorkTestObject = CreateAVolunteerWorkTestObject1();           
            SetUpVolunteerWorkDaoMockObjects();

            // Act
            Assert.ThrowsException<ArgumentException>(() => 
                dao.UpdateVolunteerWorkByPortfolioId(nonExistentPortfolioId, volunteerWorkTestObject.Id, volunteerWorkTestObject),
                "PortfolioId must be greater than zero.");
        }

        [TestMethod]
        public void UpdateVolunteerWorkByPortfolioId_Throws_Argument_Exception_When_A_Volunteer_Work_Does_Not_Exist()
        {
            // Arrange
            int portfolioId = 1;
            int nonExistentVolunteerWorkId = -1;

            VolunteerWork volunteerWorkTestObject = CreateAVolunteerWorkTestObject1();           
            SetUpVolunteerWorkDaoMockObjects();

            // Act
            Assert.ThrowsException<ArgumentException>(() => 
                dao.UpdateVolunteerWorkByPortfolioId(portfolioId, nonExistentVolunteerWorkId, volunteerWorkTestObject),
                "VolunteerWorkId must be greater than zero.");
        }

        [TestMethod]
        public void DeleteVolunteerWorkByPortfolioId_Deletes_A_Volunteer_Work()
        {
            // Arrange
            int portfolioId = 1;
            VolunteerWork volunteerWorkTestObject = CreateAVolunteerWorkTestObject1();
            VolunteerWork createdVolunteerWork = dao.CreateVolunteerWorkByPortfolioId(portfolioId, volunteerWorkTestObject);
            SetUpVolunteerWorkDaoMockObjects();

            // Act
            dao.DeleteVolunteerWorkByPortfolioId(portfolioId, createdVolunteerWork.Id);

            // Assert
            VolunteerWork? deletedVolunteerWork = dao.GetVolunteerWorkByPortfolioId(portfolioId, createdVolunteerWork.Id);
            Assert.IsNull(deletedVolunteerWork);
        }

        [TestMethod]
        public void DeleteVolunteerWorkByPortfolioId_Returns_Correct_Number_Of_Rows_Affected()
        {
            // Arrange
            int portfolioId = 1;
            
            VolunteerWork volunteerWorkTestObject = CreateAVolunteerWorkTestObject1();
            VolunteerWork createdVolunteerWork = dao.CreateVolunteerWorkByPortfolioId(portfolioId, volunteerWorkTestObject);
            SetUpVolunteerWorkDaoMockObjects();

            // Act
            int rowsAffected = dao.DeleteVolunteerWorkByPortfolioId(portfolioId, createdVolunteerWork.Id);

            // Assert
            Assert.AreEqual(1, rowsAffected);
        }

        [TestMethod]
        public void DeleteVolunteerWorkByPortfolioId_Throws_Argument_Exception_When_A_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;
            int volunteerWorkId = 1;

            // Act
            Assert.ThrowsException<ArgumentException>(() => 
                dao.DeleteVolunteerWorkByPortfolioId(nonExistentPortfolioId, volunteerWorkId),
                "PortfolioId must be greater than zero.");
        }


    }
}