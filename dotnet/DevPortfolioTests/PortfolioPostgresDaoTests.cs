using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class PortfolioPostgresDaoTests : PostgresDaoTestBase
    {
        private PortfolioPostgresDao dao = null!;
        private Mock<ISideProjectDao> sideProjectDaoMock = null!;
        private Mock<IWebsiteDao> websiteDaoMock = null!;
        private Mock<IImageDao> imageDaoMock = null!;
        private Mock<ISkillDao> skillDaoMock = null!;
        private Mock<IWorkExperienceDao> workExperienceDaoMock = null!;
        private Mock<IEducationDao> educationDaoMock = null!;
        private Mock<ICredentialDao> credentialDaoMock = null!;
        private Mock<IVolunteerWorkDao> volunteerWorkDaoMock = null!;
        private Mock<IOpenSourceContributionDao> openSourceContributionDaoMock = null!;
        private Mock<IHobbyDao> hobbyDaoMock = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            try
            {
                base.Initialize();
                InitializeMocks();
                dao = CreateDaoWithMocks();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during TestInitialize: {ex}");
                throw;
            }
        }

        private void InitializeMocks()
        {
            sideProjectDaoMock = new Mock<ISideProjectDao>();
            websiteDaoMock = new Mock<IWebsiteDao>();
            imageDaoMock = new Mock<IImageDao>();
            skillDaoMock = new Mock<ISkillDao>();
            workExperienceDaoMock = new Mock<IWorkExperienceDao>();
            educationDaoMock = new Mock<IEducationDao>();
            credentialDaoMock = new Mock<ICredentialDao>();
            volunteerWorkDaoMock = new Mock<IVolunteerWorkDao>();
            openSourceContributionDaoMock = new Mock<IOpenSourceContributionDao>();
            hobbyDaoMock = new Mock<IHobbyDao>();
        }

        private PortfolioPostgresDao CreateDaoWithMocks()
        {
            return new PortfolioPostgresDao(
                TestConnectionString,
                sideProjectDaoMock.Object,
                websiteDaoMock.Object,
                imageDaoMock.Object,
                skillDaoMock.Object,
                workExperienceDaoMock.Object,
                educationDaoMock.Object,
                credentialDaoMock.Object,
                volunteerWorkDaoMock.Object,
                openSourceContributionDaoMock.Object,
                hobbyDaoMock.Object);
        }

        // private PortfolioPostgresDao CreateDaoWithMocks()
        // {
        //     var sideProjectDaoMock = new Mock<ISideProjectDao>();
        //     var websiteDaoMock = new Mock<IWebsiteDao>();
        //     var imageDaoMock = new Mock<IImageDao>();
        //     var skillDaoMock = new Mock<ISkillDao>();
        //     var workExperienceDaoMock = new Mock<IWorkExperienceDao>();
        //     var educationDaoMock = new Mock<IEducationDao>();
        //     var credentialDaoMock = new Mock<ICredentialDao>();
        //     var volunteerWorkDaoMock = new Mock<IVolunteerWorkDao>();
        //     var openSourceContributionDaoMock = new Mock<IOpenSourceContributionDao>();
        //     var hobbyDaoMock = new Mock<IHobbyDao>();

        //     return new PortfolioPostgresDao(
        //         TestConnectionString,
        //         sideProjectDaoMock.Object,
        //         websiteDaoMock.Object,
        //         imageDaoMock.Object,
        //         skillDaoMock.Object,
        //         workExperienceDaoMock.Object,
        //         educationDaoMock.Object,
        //         credentialDaoMock.Object,
        //         volunteerWorkDaoMock.Object,
        //         openSourceContributionDaoMock.Object,
        //         hobbyDaoMock.Object);
        // }

        [TestMethod]
        public void GetPortfolios_Returns_All_Portfolios()
        {

            // Act
            List<Portfolio> portfolios = dao.GetPortfolios();

            // Assert
            Assert.IsNotNull(portfolios);
            Assert.IsTrue(portfolios.Count > 0);

            // Additional assertions for properties
            foreach (var portfolio in portfolios)
            {
                Assert.IsNotNull(portfolio.Id);
                Assert.IsNotNull(portfolio.Name);
                Assert.IsNotNull(portfolio.Location);
                Assert.IsNotNull(portfolio.ProfessionalSummary);
                Assert.IsNotNull(portfolio.Email);
            }
        }

        // [TestMethod]
        // public void GetPortfolios_Returns_Empty_List_If_No_Portfolios()
        // {
        //     // Arrange
        //     using (var conn = new Npgsql.NpgsqlConnection(TestConnectionString))
        //     {
        //         conn.Open();

        //         using (var cmd = new Npgsql.NpgsqlCommand("DELETE FROM portfolios", conn))
        //         {
        //             cmd.ExecuteNonQuery();
        //         }
        //     }

        //     // Act
        //     List<Portfolio> portfolios = dao.GetPortfolios();

        //     // Assert
        //     Assert.IsNotNull(portfolios);
        //     Assert.AreEqual(0, portfolios.Count);
        // }

        [TestMethod]
        public void GetPortfolioById_Returns_Correct_Portfolio()
        {
            // Act
            Portfolio portfolio = dao.GetPortfolio(1);

            // Assert
            Assert.IsNotNull(portfolio);
            Assert.AreEqual(1, portfolio.Id);

            // Additional assertions for properties
            Assert.IsNotNull(portfolio.Name);
            Assert.IsNotNull(portfolio.Location);
            Assert.IsNotNull(portfolio.ProfessionalSummary);
            Assert.IsNotNull(portfolio.Email);
        }

        [TestMethod]
        public void GetPortfolioById_Returns_Null_If_Portfolio_Does_Not_Exist()
        {
            // Act
            Portfolio portfolio = dao.GetPortfolio(999);

            // Assert
            Assert.IsNull(portfolio);
        }

        [TestMethod]
        public void AddPortfolio_Returns_Portfolio_With_Id()
        {
            // Arrange
            Portfolio portfolio = new Portfolio
            {
                Name = "Test Portfolio",
                Location = "Test Location",
                ProfessionalSummary = "Test Professional Summary",
                Email = "Test Email"
            };

            // Act
            Portfolio addedPortfolio = dao.CreatePortfolio(portfolio);

            // Assert
            Assert.IsNotNull(addedPortfolio);
            Assert.IsNotNull(addedPortfolio.Id);
            Assert.AreEqual(portfolio.Name, addedPortfolio.Name);
            Assert.AreEqual(portfolio.Location, addedPortfolio.Location);
            Assert.AreEqual(portfolio.ProfessionalSummary, addedPortfolio.ProfessionalSummary);
            Assert.AreEqual(portfolio.Email, addedPortfolio.Email);

            // Additional assertions for properties
            Assert.IsNotNull(addedPortfolio.Name);
            Assert.IsNotNull(addedPortfolio.Location);
            Assert.IsNotNull(addedPortfolio.ProfessionalSummary);
            Assert.IsNotNull(addedPortfolio.Email);
        }

        [TestMethod]
        public void UpdatePortfolio_Returns_Updated_Portfolio()
        {
            // Arrange
            Portfolio portfolio = new Portfolio
            {
                Id = 1,
                Name = "Updated Portfolio",
                Location = "Updated Location",
                ProfessionalSummary = "Updated Professional Summary",
                Email = "Updated Email"
            };

            // Act
            Portfolio updatedPortfolio = dao.UpdatePortfolio(portfolio, portfolio.Id);

            // Assert
            Assert.IsNotNull(updatedPortfolio);
            Assert.AreEqual(portfolio.Id, updatedPortfolio.Id);
            Assert.AreEqual(portfolio.Name, updatedPortfolio.Name);
            Assert.AreEqual(portfolio.Location, updatedPortfolio.Location);
            Assert.AreEqual(portfolio.ProfessionalSummary, updatedPortfolio.ProfessionalSummary);
            Assert.AreEqual(portfolio.Email, updatedPortfolio.Email);

            // Additional assertions for properties
            Assert.IsNotNull(updatedPortfolio.Name);
            Assert.IsNotNull(updatedPortfolio.Location);
            Assert.IsNotNull(updatedPortfolio.ProfessionalSummary);
            Assert.IsNotNull(updatedPortfolio.Email);
        }

        [TestMethod]
        public void UpdatePortfolio_Returns_Null_If_Portfolio_Does_Not_Exist()
        {
            // Arrange
            Portfolio portfolio = new Portfolio
            {
                Id = 999,
                Name = "Updated Portfolio",
                Location = "Updated Location",
                ProfessionalSummary = "Updated Professional Summary",
                Email = "Updated Email"
            };

            // Act
            Portfolio updatedPortfolio = dao.UpdatePortfolio(portfolio, portfolio.Id);

            // Assert
            Assert.IsNull(updatedPortfolio);
        }

        [TestMethod]
        public void DeletePortfolio_Returns_True_If_Portfolio_Deleted()
        {
            // Arrange
            Portfolio portfolio = new Portfolio
            {
                Name = "Test Portfolio",
                Location = "Test Location",
                ProfessionalSummary = "Test Professional Summary",
                Email = "Test Email"
            };

            Portfolio addedPortfolio = dao.CreatePortfolio(portfolio);

            // Act
            int deleteResult = dao.DeletePortfolio(addedPortfolio.Id);
            bool isDeleted = deleteResult == 1;

            // Assert
            Assert.IsTrue(isDeleted);
        }

        [TestMethod]
        public void DeletePortfolio_Returns_False_If_Portfolio_Does_Not_Exist()
        {
            // Act
            int deleteResult = dao.DeletePortfolio(999);
            bool isDeleted = deleteResult == 1;

            // Assert
            Assert.IsFalse(isDeleted);
        }

    }
}