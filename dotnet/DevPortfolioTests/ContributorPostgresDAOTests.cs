using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class ContributorPostgresDAOTests : PostgresDaoTestBase
    {
        private ContributorPostgresDao dao = null!;
        private Mock<IImageDao> mockImageDao = null!;
        private Mock<IWebsiteDao> mockWebsiteDao = null!;

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
        }

        private ContributorPostgresDao CreateDaoWithMocks()
        {
            return new ContributorPostgresDao(
                TestConnectionString,
                mockImageDao.Object,
                mockWebsiteDao.Object
            );
        }

        private Contributor CreateAContributorTestObject1()
        {
            return new Contributor
            {
                FirstName = "Test First Name 1",
                LastName = "Test Last Name 1",
                Email = "Test Email 1",
                Bio = "Test Bio 1",
                ContributionDetails = "Test Contribution Details 1"
            };
        }

        private Contributor CreateAContributorTestObject2()
        {
            return new Contributor
            {
                FirstName = "Test First Name 2",
                LastName = "Test Last Name 2",
                Email = "Test Email 2",
                Bio = "Test Bio 2",
                ContributionDetails = "Test Contribution Details 2"
            };
        }

        private void SetUpContributorNestedDaoMockObjects()
        {
            mockImageDao.Setup(m => m.GetImageByContributorId(It.IsAny<int>())).Returns(new Image());
            mockWebsiteDao.Setup(m => m.GetWebsiteByContributorId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Website());
        }

        // TODO Add tests for ContributorPostgresDao
    }
}