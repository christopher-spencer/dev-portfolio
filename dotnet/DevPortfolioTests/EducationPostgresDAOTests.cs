using Capstone.DAO;
using Capstone.DAO.Interfaces;
using Capstone.Models;
using Moq;

namespace Capstone.UnitTests.DAO
{
    [TestClass]
    public class EducationPostgresDAOTests : PostgresDaoTestBase
    {
        private EducationPostgresDao dao = null!;
        private Mock<IImageDao> imageDaoMock = null!;
        private Mock<IWebsiteDao> websiteDaoMock = null!;
        private Mock<IAchievementDao> achievementDaoMock = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            base.Initialize();
            InitializeMocks();
            dao = CreateDaoWithMocks();
        }

        private void InitializeMocks()
        {
            imageDaoMock = new Mock<IImageDao>();
            websiteDaoMock = new Mock<IWebsiteDao>();
            achievementDaoMock = new Mock<IAchievementDao>();
        }

        private EducationPostgresDao CreateDaoWithMocks()
        {
            return new EducationPostgresDao(
                TestConnectionString, 
                imageDaoMock.Object, 
                websiteDaoMock.Object, 
                achievementDaoMock.Object
            );
        }

        private Education CreateAnEducationTestObject1()
        {
            return new Education
            {
                InstitutionName = "Test Institution 1",
                Location = "Test Location 1",
                Description = "Test Description 1",
                FieldOfStudy = "Test Field of Study 1",
                Major = "Test Major 1",
                Minor = "Test Minor 1",
                DegreeObtained = "Test Degree Obtained 1",
                GPAOverall = 3.5M,
                GPAInMajor = 3.7M,
                StartDate = new DateTime(2015, 1, 1),
                GraduationDate = new DateTime(2019, 5, 15)
            };
        }

        private Education CreateAnEducationTestObject2()
        {
            return new Education
            {
                InstitutionName = "Test Institution 2",
                Location = "Test Location 2",
                Description = "Test Description 2",
                FieldOfStudy = "Test Field of Study 2",
                Major = "Test Major 2",
                Minor = "Test Minor 2",
                DegreeObtained = "Test Degree Obtained 2",
                GPAOverall = 3.8M,
                GPAInMajor = 3.9M,
                StartDate = new DateTime(2016, 1, 1),
                GraduationDate = new DateTime(2020, 5, 15)
            };
        }

        private void SetUpEducationNestedDaoMockObjects()
        {
            imageDaoMock.Setup(m => m.GetImageByEducationId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Image());
            imageDaoMock.Setup(m => m.GetAdditionalImagesByEducationId(It.IsAny<int>())).Returns(new List<Image>());
            websiteDaoMock.Setup(m => m.GetWebsiteByEducationId(It.IsAny<int>())).Returns(new Website());
            achievementDaoMock.Setup(m => m.GetAchievementsByEducationId(It.IsAny<int>())).Returns(new List<Achievement>());
        }

    }
}