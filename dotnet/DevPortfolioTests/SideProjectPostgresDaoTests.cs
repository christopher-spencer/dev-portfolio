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
    public class SideProjectPostgresDaoTests : PostgresDaoTestBase
    {
        private SideProjectPostgresDao dao = null!;
        private Mock<IGoalDao> goalDaoMock = null!;
        private Mock<IImageDao> imageDaoMock = null!;
        private Mock<ISkillDao> skillDaoMock = null!;
        private Mock<IContributorDao> contributorDaoMock = null!;
        private Mock<IApiServiceDao> apiServiceDaoMock = null!;
        private Mock<IWebsiteDao> websiteDaoMock = null!;
        private Mock<IDependencyLibraryDao> dependencyLibraryDaoMock = null!;

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
            goalDaoMock = new Mock<IGoalDao>();
            imageDaoMock = new Mock<IImageDao>();
            skillDaoMock = new Mock<ISkillDao>();
            contributorDaoMock = new Mock<IContributorDao>();
            apiServiceDaoMock = new Mock<IApiServiceDao>();
            websiteDaoMock = new Mock<IWebsiteDao>();
            dependencyLibraryDaoMock = new Mock<IDependencyLibraryDao>();
        }

        private SideProjectPostgresDao CreateDaoWithMocks()
        {
            return new SideProjectPostgresDao(
                TestConnectionString,
                goalDaoMock.Object,
                imageDaoMock.Object,
                skillDaoMock.Object,
                contributorDaoMock.Object,
                apiServiceDaoMock.Object,
                dependencyLibraryDaoMock.Object,
                websiteDaoMock.Object);
        }

        [TestMethod]
        public void GetSideProjects_ShouldReturnAllSideProjects()
        {
            //Act
            List<SideProject> sideProjects = dao.GetSideProjects();

            //Assert
            Assert.AreEqual(2, sideProjects.Count);
            Assert.IsTrue(sideProjects.Count > 0);

            // Additional assertions for properties
            foreach (var sideproject in sideProjects)
            {
                Assert.IsNotNull(sideproject.Id);
                Assert.IsNotNull(sideproject.Name);
                Assert.IsNotNull(sideproject.Description);
                Assert.IsNotNull(sideproject.StartDate);
                Assert.IsNotNull(sideproject.FinishDate);
            }
        }


    
    }
}