using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class AchievementPostgresDaoTests : PostgresDaoTestBase
    {
        private AchievementPostgresDao dao = null!;
        private Mock<IImageDao> imageDaoMock = null!;



    }
}