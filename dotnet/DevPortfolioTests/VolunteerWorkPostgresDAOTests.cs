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


    }
}