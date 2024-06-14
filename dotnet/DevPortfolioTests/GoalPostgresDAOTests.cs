using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class GoalPostgresDAOTests : PostgresDaoTestBase
    {
        private GoalPostgresDao dao = null!;
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

        private GoalPostgresDao CreateDaoWithMocks()
        {
            return new GoalPostgresDao(
                TestConnectionString,
                mockImageDao.Object
            );
        }

        private Goal CreateAGoalTestObject1()
        {
            return new Goal
            {
                Description = "Test Description 1"
            };
        }

        private Goal CreateAGoalTestObject2()
        {
            return new Goal
            {
                Description = "Test Description 2"
            };
        }

        private void SetUpGoalNestedDaoMockObjects()
        {
            mockImageDao.Setup(m => m.GetImageByGoalId(It.IsAny<int>())).Returns(new Image());
        }

        // TODO Add Unit Tests for GoalPostgresDaoTests

    }
}