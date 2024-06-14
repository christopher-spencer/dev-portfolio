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

        private void SetUpHobbyNestedDaoMockObjects()
        {
            mockImageDao.Setup(m => m.GetImageByHobbyId(It.IsAny<int>())).Returns(new Image());
        }

        // TODO add Unit Tests for HobbyPostgresDaoTests

    }
}