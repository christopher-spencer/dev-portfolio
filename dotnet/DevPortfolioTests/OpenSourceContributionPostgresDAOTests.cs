using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class OpenSourceContributionPostgresDAOTests : PostgresDaoTestBase
    {
        private OpenSourceContributionPostgresDao dao = null!;
        private Mock<IImageDao> mockImageDao = null!;
        private Mock<IWebsiteDao> mockWebsiteDao = null!;
        private Mock<ISkillDao> mockSkillDao = null!;
        private Mock<IAchievementDao> mockAchievementDao = null!;

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
            mockSkillDao = new Mock<ISkillDao>();
            mockAchievementDao = new Mock<IAchievementDao>();
        }

        private OpenSourceContributionPostgresDao CreateDaoWithMocks()
        {
            return new OpenSourceContributionPostgresDao(
                TestConnectionString, 
                mockImageDao.Object, 
                mockWebsiteDao.Object, 
                mockSkillDao.Object, 
                mockAchievementDao.Object
            );
        }   

        private OpenSourceContribution CreateAnOpenSourceContributionTestObject1()
        {
            return new OpenSourceContribution
            {
                ProjectName = "Test Project Name",
                OrganizationName = "Test Organization Name",
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2021, 1, 31),
                ProjectDescription = "Test Project Description",
                ContributionDetails = "Test Contribution Details",
            };
        }

        private OpenSourceContribution CreateAnOpenSourceContributionTestObject2()
        {
            return new OpenSourceContribution
            {
                ProjectName = "Test Project Name 2",
                OrganizationName = "Test Organization Name 2",
                StartDate = new DateTime(2021, 2, 1),
                EndDate = new DateTime(2021, 2, 28),
                ProjectDescription = "Test Project Description 2",
                ContributionDetails = "Test Contribution Details 2",
            };
        }
    }
}