using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class SkillPostgresDAOTests : PostgresDaoTestBase
    {
        private SkillPostgresDao dao = null!;
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

        private SkillPostgresDao CreateDaoWithMocks()
        {
            return new SkillPostgresDao(
                TestConnectionString,
                mockImageDao.Object
            );
        }

        private Skill CreateASkillTestObject1()
        {
            return new Skill
            {
                Name = "Test Name 1"
            };
        }

        private Skill CreateASkillTestObject2()
        {
            return new Skill
            {
                Name = "Test Name 2"
            };
        }

        private void SetUpSkillNestedDaoMockObjects()
        {
            mockImageDao.Setup(m => m.GetImageBySkillId(It.IsAny<int>())).Returns(new Image());
        }

        // TODO add Unit Tests for SkillPostgresDaoTests

    }
}