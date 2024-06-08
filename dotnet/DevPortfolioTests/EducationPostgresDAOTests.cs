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

        [TestMethod]
        public void GetEducations_Returns_All_Educations()
        {
            // Arrange
            int portfolioId = 1;
            Education testEducation1 = CreateAnEducationTestObject1();
            Education testEducation2 = CreateAnEducationTestObject2();
            
            dao.CreateEducationByPortfolioId(portfolioId, testEducation1);
            dao.CreateEducationByPortfolioId(portfolioId, testEducation2);
            SetUpEducationNestedDaoMockObjects();

            List<Education> educations = dao.GetEducations();

            //Assert
            Assert.AreEqual(2, educations.Count);
            Assert.AreEqual(testEducation1.InstitutionName, educations[0].InstitutionName);
            Assert.AreEqual(testEducation1.Location, educations[0].Location);
            Assert.AreEqual(testEducation1.Description, educations[0].Description);
            Assert.AreEqual(testEducation1.FieldOfStudy, educations[0].FieldOfStudy);
            Assert.AreEqual(testEducation1.Major, educations[0].Major);
            Assert.AreEqual(testEducation1.Minor, educations[0].Minor);
            Assert.AreEqual(testEducation1.DegreeObtained, educations[0].DegreeObtained);
            Assert.AreEqual(testEducation1.GPAOverall, educations[0].GPAOverall);
            Assert.AreEqual(testEducation1.GPAInMajor, educations[0].GPAInMajor);
            Assert.AreEqual(testEducation1.StartDate, educations[0].StartDate);
            Assert.AreEqual(testEducation1.GraduationDate, educations[0].GraduationDate);

            Assert.AreEqual(testEducation2.InstitutionName, educations[1].InstitutionName);
            Assert.AreEqual(testEducation2.Location, educations[1].Location);
            Assert.AreEqual(testEducation2.Description, educations[1].Description);
            Assert.AreEqual(testEducation2.FieldOfStudy, educations[1].FieldOfStudy);
            Assert.AreEqual(testEducation2.Major, educations[1].Major);
            Assert.AreEqual(testEducation2.Minor, educations[1].Minor);
            Assert.AreEqual(testEducation2.DegreeObtained, educations[1].DegreeObtained);
            Assert.AreEqual(testEducation2.GPAOverall, educations[1].GPAOverall);
            Assert.AreEqual(testEducation2.GPAInMajor, educations[1].GPAInMajor);
            Assert.AreEqual(testEducation2.StartDate, educations[1].StartDate);
            Assert.AreEqual(testEducation2.GraduationDate, educations[1].GraduationDate);
        }

        public void GetEducations_Returns_Empty_List_When_No_Educations()
        {
            // Arrange
            List<Education> educations = dao.GetEducations();

            //Assert
            Assert.AreEqual(0, educations.Count);
        }

        public void GetEducationByPortfolioId_Returns_Correct_Education()
        {
            // Arrange
            int portfolioId = 1;
            Education testEducation1 = CreateAnEducationTestObject1();
            Education createdEducation = dao.CreateEducationByPortfolioId(portfolioId, testEducation1);
            SetUpEducationNestedDaoMockObjects();

            // Act
            Education education = dao.GetEducationByPortfolioId(portfolioId, createdEducation.Id);

            // Assert
            Assert.AreEqual(createdEducation.InstitutionName, education.InstitutionName);
            Assert.AreEqual(createdEducation.Location, education.Location);
            Assert.AreEqual(createdEducation.Description, education.Description);
            Assert.AreEqual(createdEducation.FieldOfStudy, education.FieldOfStudy);
            Assert.AreEqual(createdEducation.Major, education.Major);
            Assert.AreEqual(createdEducation.Minor, education.Minor);
            Assert.AreEqual(createdEducation.DegreeObtained, education.DegreeObtained);
            Assert.AreEqual(createdEducation.GPAOverall, education.GPAOverall);
            Assert.AreEqual(createdEducation.GPAInMajor, education.GPAInMajor);
            Assert.AreEqual(createdEducation.StartDate, education.StartDate);
            Assert.AreEqual(createdEducation.GraduationDate, education.GraduationDate);
        }

        public void GetEducationByPortfolioId_Returns_Null_When_Education_Not_Found()
        {
            // Arrange
            int portfolioId = 1;
            int educationId = 1;

            // Act
            Education? education = dao.GetEducationByPortfolioId(portfolioId, educationId);

            // Assert
            Assert.IsNull(education);
        }

        public void CreateEducationByPortfolioId_Creates_An_Education()
        {
            // Arrange
            int portfolioId = 1;
            Education education = CreateAnEducationTestObject1();
            SetUpEducationNestedDaoMockObjects();

            // Act
            Education createdEducation = dao.CreateEducationByPortfolioId(portfolioId, education);

            // Assert
            Assert.IsNotNull(createdEducation);
            Assert.AreEqual(education.InstitutionName, createdEducation.InstitutionName);
            Assert.AreEqual(education.Location, createdEducation.Location);
            Assert.AreEqual(education.Description, createdEducation.Description);
            Assert.AreEqual(education.FieldOfStudy, createdEducation.FieldOfStudy);
            Assert.AreEqual(education.Major, createdEducation.Major);
            Assert.AreEqual(education.Minor, createdEducation.Minor);
            Assert.AreEqual(education.DegreeObtained, createdEducation.DegreeObtained);
            Assert.AreEqual(education.GPAOverall, createdEducation.GPAOverall);
            Assert.AreEqual(education.GPAInMajor, createdEducation.GPAInMajor);
            Assert.AreEqual(education.StartDate, createdEducation.StartDate);
            Assert.AreEqual(education.GraduationDate, createdEducation.GraduationDate);
        }

    }
}