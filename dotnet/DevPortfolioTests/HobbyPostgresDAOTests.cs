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

        [TestMethod]
        public void GetHobbies_Returns_Empty_List_When_No_Hobbies()
        {
            // Act
            List<Hobby> hobbies = dao.GetHobbies();

            // Assert
            Assert.AreEqual(0, hobbies.Count);
        }

        [TestMethod]
        public void GetHobbyByPortfolioId_Returns_Correct_Hobby()
        {
            // Arrange
            int portfolioId = 1;
            Hobby testHobby1 = CreateAHobbyTestObject1();
            Hobby createdHobby = dao.CreateHobbyByPortfolioId(portfolioId, testHobby1);
            SetUpHobbyNestedDaoMockObjects();

            // Act
            Hobby hobby = dao.GetHobbyByPortfolioId(portfolioId, testHobby1.Id);

            // Assert
            Assert.AreEqual(createdHobby.Description, hobby.Description);
        }

        [TestMethod]
        public void GetHobbyByPortfolioId_Returns_Null_When_Hobby_Not_Found()
        {
            // Arrange
            int portfolioId = 1;
            int hobbyId = 1;
            
            // Act
            Hobby? hobby = dao.GetHobbyByPortfolioId(portfolioId, hobbyId);

            // Assert
            Assert.IsNull(hobby);
        }

        [TestMethod]
        public void CreateHobbyByPortfolioId_Returns_Created_Hobby()
        {
            // Arrange
            int portfolioId = 1;
            Hobby testHobby1 = CreateAHobbyTestObject1();
            SetUpHobbyNestedDaoMockObjects();

            // Act
            Hobby createdHobby = dao.CreateHobbyByPortfolioId(portfolioId, testHobby1);

            // Assert
            Assert.AreEqual(testHobby1.Description, createdHobby.Description);
        }

        [TestMethod]
        public void CreateHobbyByPortfolioId_Throws_Argument_Exception_When_A_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;

            Hobby hobby = CreateAHobbyTestObject1();
            SetUpHobbyNestedDaoMockObjects();

            // Act
            Assert.ThrowsException<ArgumentException>(() => 
                dao.CreateHobbyByPortfolioId(nonExistentPortfolioId, hobby),
                "PortfolioId must be greater than zero."
            );
        }

        [TestMethod]
        public void UpdateHobbyByPortfolioId_Updates_A_Hobby()
        {
            // Arrange
            int portfolioId = 1;
            Hobby hobbyOriginalObject = CreateAHobbyTestObject1();
            Hobby hobbyUpdatedObject = CreateAHobbyTestObject2();

            Hobby createdOriginalHobby = dao.CreateHobbyByPortfolioId(portfolioId, hobbyOriginalObject);

            SetUpHobbyNestedDaoMockObjects();

            // Act
            Hobby updatedHobby = dao.UpdateHobbyByPortfolioId(portfolioId, createdOriginalHobby.Id, hobbyUpdatedObject);

            Hobby hobby = dao.GetHobbyByPortfolioId(portfolioId, createdOriginalHobby.Id);

            // Assert
            Assert.AreEqual(updatedHobby.Description, hobby.Description);
        }

        [TestMethod]
        public void UpdateHobbyByPortfolioId_Throws_Argument_Exception_When_Portfolio_Not_Found()
        {
            // Arrange
            int nonExistentPortfolioId = -1;

            Hobby hobby = CreateAHobbyTestObject1();
            SetUpHobbyNestedDaoMockObjects();

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => 
                dao.UpdateHobbyByPortfolioId(nonExistentPortfolioId, hobby.Id, hobby),
                "PortfolioId must be greater than zero."
            );
        }

        [TestMethod]
        public void UpdateHobbyByPortfolioId_Throws_Argument_Exception_When_Hobby_Not_Found()
        {
            // Arrange
            int portfolioId = 1;
            int nonExistentHobbyId = -1;

            Hobby hobby = CreateAHobbyTestObject1();
            SetUpHobbyNestedDaoMockObjects();

            // Act
            Assert.ThrowsException<ArgumentException>(() => 
                dao.UpdateHobbyByPortfolioId(portfolioId, nonExistentHobbyId, hobby),
                "HobbyId must be greater than zero."
            );
        }

        [TestMethod]
        public void DeleteHobbyByPortfolioId_Deletes_A_Hobby()
        {
            // Arrange
            int portfolioId = 1;

            Hobby hobby = CreateAHobbyTestObject1();
            Hobby createdHobby = dao.CreateHobbyByPortfolioId(portfolioId, hobby);
            SetUpHobbyNestedDaoMockObjects();

            // Act
            dao.DeleteHobbyByPortfolioId(portfolioId, createdHobby.Id);

            Hobby? deletedHobby = dao.GetHobbyByPortfolioId(portfolioId, createdHobby.Id);

            // Assert
            Assert.IsNull(deletedHobby);
        }

        [TestMethod]
        public void DeleteHobbyByPortfolioId__Returns_Correct_Number_Of_Rows_Affected()
        {
            // Arrange
            int portfolioId = 1;

            Hobby hobby = CreateAHobbyTestObject1();
            Hobby createdHobby = dao.CreateHobbyByPortfolioId(portfolioId, hobby);
            SetUpHobbyNestedDaoMockObjects();

            // Act
            int rowsAffected = dao.DeleteHobbyByPortfolioId(portfolioId, createdHobby.Id);

            // Assert
            Assert.AreEqual(1, rowsAffected);
        }

        [TestMethod]
        public void DeleteHobbyByPortfolioId_Throws_Argument_Exception_When_Portfolio_Not_Found()
        {
            // Arrange
            int nonExistentPortfolioId = -1;
            int hobbyId = 1;

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => 
                dao.DeleteHobbyByPortfolioId(nonExistentPortfolioId, hobbyId),
                "PortfolioId must be greater than zero."
            );
        }



    }
}