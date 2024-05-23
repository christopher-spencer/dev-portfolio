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
        private PortfolioPostgresDao dao;

        [TestInitialize]
        public void TestInitialize()
        {
            base.Initialize();
            dao = CreateDaoWithMocks();
        }

        private PortfolioPostgresDao CreateDaoWithMocks()
        {
            var sideProjectDaoMock = new Mock<ISideProjectDao>();
            var websiteDaoMock = new Mock<IWebsiteDao>();
            var imageDaoMock = new Mock<IImageDao>();
            var skillDaoMock = new Mock<ISkillDao>();
            var workExperienceDaoMock = new Mock<IWorkExperienceDao>();
            var educationDaoMock = new Mock<IEducationDao>();
            var credentialDaoMock = new Mock<ICredentialDao>();
            var volunteerWorkDaoMock = new Mock<IVolunteerWorkDao>();
            var openSourceContributionDaoMock = new Mock<IOpenSourceContributionDao>();
            var hobbyDaoMock = new Mock<IHobbyDao>();

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



    }
}