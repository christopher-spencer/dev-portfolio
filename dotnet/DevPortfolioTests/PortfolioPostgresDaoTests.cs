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

        [TestMethod]

        public void GetPortfolios_Returns_All_Portfolios()
        {
            // Arrange
            var imageDaoMock = new Mock<IImageDao>();
            PortfolioPostgresDao dao = new PortfolioPostgresDao(TestConnectionString, imageDaoMock.Object);

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

    }
}