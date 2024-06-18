using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class HobbyPostgresDAOTests : PostgresDaoTestBase
    {

        private HobbyPostgresDao dao = null!;
        private Mock<IImageDao> mockImageDao = null!;

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
        }

        private HobbyPostgresDao CreateDaoWithMocks()
        {
            return new HobbyPostgresDao(
                TestConnectionString, 
                mockImageDao.Object
            );
        }

        private Hobby CreateAHobbyTestObject1()
        {
            return new Hobby
            {
                Description = "Test Description 1"
            };
        }

        private Hobby CreateAHobbyTestObject2()
        {
            return new Hobby
            {
                Description = "Test Description 2"
            };
        }

        private void SetUpHobbyNestedDaoMockObjects()
        {
            mockImageDao.Setup(m => m.GetImageByHobbyId(It.IsAny<int>())).Returns(new Image());
        }

        [TestMethod]
        public void GetHobbies_Returns_All_Hobbies()
        {
            // Arrange
            int portfolioId = 1;
            Hobby testHobby1 = CreateAHobbyTestObject1();
            Hobby testHobby2 = CreateAHobbyTestObject2();

            dao.CreateHobbyByPortfolioId(portfolioId, testHobby1);
            dao.CreateHobbyByPortfolioId(portfolioId, testHobby2);
            SetUpHobbyNestedDaoMockObjects();

            // Act
            List<Hobby> hobbies = dao.GetHobbiesByPortfolioId(portfolioId);

            // Assert
            Assert.AreEqual(2, hobbies.Count);
            Assert.AreEqual(testHobby1.Description, hobbies[0].Description);
        
            Assert.AreEqual(testHobby2.Description, hobbies[1].Description);
        }



    }
}