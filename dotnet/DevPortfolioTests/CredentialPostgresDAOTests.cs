using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class CredentialPostgresDAOTests : PostgresDaoTestBase
    {
        private CredentialPostgresDao dao = null!;
        private Mock<IImageDao> mockImageDao = null!;
        private Mock<IWebsiteDao> mockWebsiteDao = null!;
        private Mock<ISkillDao> mockSkillDao = null!;

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
        }

        private CredentialPostgresDao CreateDaoWithMocks()
        {
            return new CredentialPostgresDao(
                TestConnectionString, 
                mockImageDao.Object, 
                mockWebsiteDao.Object, 
                mockSkillDao.Object
            );
        }

        private Credential CreateACredentialTestObject1()
        {
            return new Credential
            {
                Id = 1,
                Name = "Test Credential 1",
                IssuingOrganization = "Test Issuing Organization 1",
                Description = "Test Description 1",
                IssueDate = new DateTime(2021, 1, 1),
                ExpirationDate = new DateTime(2021, 12, 31),
                CredentialIdNumber = 1542452325
            };
        }

        private Credential CreateACredentialTestObject2()
        {
            return new Credential
            {
                Id = 2,
                Name = "Test Credential 2",
                IssuingOrganization = "Test Issuing Organization 2",
                Description = "Test Description 2",
                IssueDate = new DateTime(2021, 1, 1),
                ExpirationDate = new DateTime(2021, 12, 31),
                CredentialIdNumber = 1542452325
            };
        }

        private void SetUpCredentialNestedDaoMockObjects()
        {
            mockImageDao.Setup(m => m.GetImageByCredentialId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            mockWebsiteDao.Setup(m => m.GetWebsiteByCredentialId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Website());
            mockSkillDao.Setup(m => m.GetSkillsByCredentialId(It.IsAny<int>())).Returns(new List<Skill>());
        }


    }
}