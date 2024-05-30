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
    
    }
}