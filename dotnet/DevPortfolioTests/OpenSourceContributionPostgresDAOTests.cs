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

        
    }
}