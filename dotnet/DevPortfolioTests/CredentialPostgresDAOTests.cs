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

        [TestMethod]
        public void GetCredentials_Returns_All_Credentials()    
        {
            // Arrange
            int portfolioId = 1;
            Credential testCredential1 = CreateACredentialTestObject1();
            Credential testCredential2 = CreateACredentialTestObject2();

            dao.CreateCredentialByPortfolioId(portfolioId, testCredential1);
            dao.CreateCredentialByPortfolioId(portfolioId, testCredential2);
            SetUpCredentialNestedDaoMockObjects();

            // Act
            List<Credential> credentials = dao.GetCredentials();

            // Assert
            Assert.AreEqual(2, credentials.Count);
            Assert.AreEqual(testCredential1.Name, credentials[0].Name);
            Assert.AreEqual(testCredential1.IssuingOrganization, credentials[0].IssuingOrganization);
            Assert.AreEqual(testCredential1.Description, credentials[0].Description);
            Assert.AreEqual(testCredential1.IssueDate, credentials[0].IssueDate);
            Assert.AreEqual(testCredential1.ExpirationDate, credentials[0].ExpirationDate);
            Assert.AreEqual(testCredential1.CredentialIdNumber, credentials[0].CredentialIdNumber);

            Assert.AreEqual(testCredential2.Name, credentials[1].Name);
            Assert.AreEqual(testCredential2.IssuingOrganization, credentials[1].IssuingOrganization);
            Assert.AreEqual(testCredential2.Description, credentials[1].Description);
            Assert.AreEqual(testCredential2.IssueDate, credentials[1].IssueDate);
            Assert.AreEqual(testCredential2.ExpirationDate, credentials[1].ExpirationDate);
            Assert.AreEqual(testCredential2.CredentialIdNumber, credentials[1].CredentialIdNumber);
        }

        [TestMethod]
        public void GetCredentials_Returns_Empty_List_When_No_Credentials_Exist()
        {
            // Act
            List<Credential> credentials = dao.GetCredentials();

            // Assert
            Assert.AreEqual(0, credentials.Count);
        }

        [TestMethod]
        public void GetCredentialByPortfolioId_Returns_Correct_Credential()
        {
            // Arrange
            int portfolioId = 1;
            Credential testCredential1 = CreateACredentialTestObject1();
            Credential createdCredential = dao.CreateCredentialByPortfolioId(portfolioId, testCredential1);
            SetUpCredentialNestedDaoMockObjects();

            // Act
            Credential credential = dao.GetCredentialByPortfolioId(portfolioId, createdCredential.Id);

            // Assert
            Assert.AreEqual(createdCredential.Name, credential.Name);
            Assert.AreEqual(createdCredential.IssuingOrganization, credential.IssuingOrganization);
            Assert.AreEqual(createdCredential.Description, credential.Description);
            Assert.AreEqual(createdCredential.IssueDate, credential.IssueDate);
            Assert.AreEqual(createdCredential.ExpirationDate, credential.ExpirationDate);
            Assert.AreEqual(createdCredential.CredentialIdNumber, credential.CredentialIdNumber);
        }

        [TestMethod]
        public void GetCredentialByPortfolioId_Returns_Null_When_Credential_Does_Not_Exist()
        {
            // Arrange
            int portfolioId = 1;
            int credentialId = 1;

            // Act
            Credential? credential = dao.GetCredentialByPortfolioId(portfolioId, credentialId);

            // Assert
            Assert.IsNull(credential);
        }

        [TestMethod]
        public void CreateCredentialByPortfolioId_Creates_Credential()
        {
            // Arrange
            int portfolioId = 1;
            Credential credential = CreateACredentialTestObject1();
            SetUpCredentialNestedDaoMockObjects();

            // Act
            Credential createdCredential = dao.CreateCredentialByPortfolioId(portfolioId, credential);

            // Assert
            Assert.IsNotNull(createdCredential);
            Assert.AreEqual(credential.Name, createdCredential.Name);
            Assert.AreEqual(credential.IssuingOrganization, createdCredential.IssuingOrganization);
            Assert.AreEqual(credential.Description, createdCredential.Description);
            Assert.AreEqual(credential.IssueDate, createdCredential.IssueDate);
            Assert.AreEqual(credential.ExpirationDate, createdCredential.ExpirationDate);
            Assert.AreEqual(credential.CredentialIdNumber, createdCredential.CredentialIdNumber);
        }

        [TestMethod]
        public void CreateCredentialByPortfolioId_Throws_Argument_Exception_When_A_Portfolio_Does_Not_Exist()
        {
            // Arrange
            int nonExistentPortfolioId = -1;

            Credential credential = CreateACredentialTestObject1();
            SetUpCredentialNestedDaoMockObjects();

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => 
                dao.CreateCredentialByPortfolioId(nonExistentPortfolioId, credential),
                "PortfolioId must be greater than zero."
            );
        }

        [TestMethod]
        public void UpdateCredentialByPortfolioId_Updates_Credential()
        {
            // Arrange
            int portfolioId = 1;

            Credential credential = CreateACredentialTestObject1();
            Credential updatedCredential = CreateACredentialTestObject2();

            Credential createdCredential = dao.CreateCredentialByPortfolioId(portfolioId, credential);
            
            SetUpCredentialNestedDaoMockObjects();

            // Act
            Credential? updatedCredentialResult = dao.UpdateCredentialByPortfolioId(portfolioId, createdCredential.Id, updatedCredential);

            // Assert
            Assert.IsNotNull(updatedCredentialResult);
            Assert.AreEqual(updatedCredential.Name, updatedCredentialResult.Name);
            Assert.AreEqual(updatedCredential.IssuingOrganization, updatedCredentialResult.IssuingOrganization);
            Assert.AreEqual(updatedCredential.Description, updatedCredentialResult.Description);
            Assert.AreEqual(updatedCredential.IssueDate, updatedCredentialResult.IssueDate);
            Assert.AreEqual(updatedCredential.ExpirationDate, updatedCredentialResult.ExpirationDate);
            Assert.AreEqual(updatedCredential.CredentialIdNumber, updatedCredentialResult.CredentialIdNumber);
        }


    }
}