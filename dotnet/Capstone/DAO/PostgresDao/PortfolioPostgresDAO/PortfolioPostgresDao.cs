using System;
using System.Collections.Generic;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class PortfolioPostgresDao : IPortfolioDao
    {
        private readonly string connectionString;
        private readonly ISideProjectDao _sideProjectDao;
        private readonly IWebsiteDao _websiteDao;
        private readonly IImageDao _imageDao;
        private readonly ISkillDao _skillDao;
        private readonly IWorkExperienceDao _workExperienceDao;
        private readonly IEducationDao _educationDao;
        private readonly ICredentialDao _credentialDao;
        private readonly IVolunteerWorkDao _volunteerWorkDao;
        private readonly IOpenSourceContributionDao _openSourceContributionDao;
        private readonly IHobbyDao _hobbyDao;


        public PortfolioPostgresDao(string dbConnectionString, ISideProjectDao sideProjectDao,
            IWebsiteDao websiteDao, IImageDao imageDao, ISkillDao skillDao,
            IWorkExperienceDao workExperienceDao, IEducationDao educationDao, ICredentialDao credentialDao,
            IVolunteerWorkDao volunteerWorkDao, IOpenSourceContributionDao openSourceContributionDao,
            IHobbyDao hobbyDao)
        {
            connectionString = dbConnectionString;
            _sideProjectDao = sideProjectDao;
            _websiteDao = websiteDao;
            _imageDao = imageDao;
            _skillDao = skillDao;
            _workExperienceDao = workExperienceDao;
            _educationDao = educationDao;
            _credentialDao = credentialDao;
            _volunteerWorkDao = volunteerWorkDao;
            _openSourceContributionDao = openSourceContributionDao;
            _hobbyDao = hobbyDao;
        }

        /*  
            **********************************************************************************************
                                            PORTFOLIO CRUD
            **********************************************************************************************
        */

        public Portfolio CreatePortfolio(Portfolio portfolio)
        {
            if (string.IsNullOrEmpty(portfolio.Name))
            {
                throw new ArgumentException("Portfolio name is required.");
            }

            if (string.IsNullOrEmpty(portfolio.ProfessionalSummary))
            {
                throw new ArgumentException("Professional summary is required.");
            }

            string sql = "INSERT INTO portfolios (name, location, professional_summary, email) " +
                         "VALUES (@name, @location, @professionalSummary, @email) " +
                         "RETURNING id";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", portfolio.Name);
                        cmd.Parameters.AddWithValue("@location", portfolio.Location);
                        cmd.Parameters.AddWithValue("@professionalSummary", portfolio.ProfessionalSummary);
                        cmd.Parameters.AddWithValue("@email", portfolio.Email);

                        int portfolioId = Convert.ToInt32(cmd.ExecuteScalar());
                        portfolio.Id = portfolioId;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the portfolio.", ex);
            }

            return portfolio;
        }

        public List<Portfolio> GetPortfolios()
        {
            List<Portfolio> portfolios = new List<Portfolio>();

            string sql = "SELECT id, name, main_image_id, location, professional_summary, email, " +
                "github_repo_link_id, linkedin_id FROM portfolios";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Portfolio portfolio = MapRowToPortfolio(reader);
                                portfolios.Add(portfolio);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the portfolios.", ex);
            }

            return portfolios;
        }

        public Portfolio GetPortfolio(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            Portfolio portfolio = null;

            string sql = "SELECT id, name, main_image_id, location, professional_summary, email, " +
                "github_repo_link_id, linkedin_id FROM portfolios WHERE id = @portfolioId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                portfolio = MapRowToPortfolio(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the portfolio.", ex);
            }

            return portfolio;
        }

        public Portfolio UpdatePortfolio(Portfolio portfolio, int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            if (string.IsNullOrEmpty(portfolio.Name))
            {
                throw new ArgumentException("Portfolio name is required.");
            }

            if (string.IsNullOrEmpty(portfolio.ProfessionalSummary))
            {
                throw new ArgumentException("Professional summary is required.");
            }

            string sql = "UPDATE portfolios SET name = @name, location = @location, " +
                         "professional_summary = @professionalSummary, email = @email " +
                         "WHERE id = @portfolioId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", portfolio.Name);
                        cmd.Parameters.AddWithValue("@location", portfolio.Location);
                        cmd.Parameters.AddWithValue("@professionalSummary", portfolio.ProfessionalSummary);
                        cmd.Parameters.AddWithValue("@email", portfolio.Email);
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return portfolio;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the portfolio.", ex);
            }
        }

        public int DeletePortfolio(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("PortfolioId must be greater than zero.");
            }

            string sql = "DELETE FROM portfolios WHERE id = @portfolioId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int rowsAffected;

                            int? mainImageId = GetMainImageIdByPortfolioId(portfolioId);
                            int? gitHubIt = GetGitHubIdByPortfolioId(portfolioId);
                            int? linkedInId = GetLinkedInIdByPortfolioId(portfolioId);

                            if (mainImageId.HasValue)
                            {
                                _imageDao.DeleteImageByPortfolioId(portfolioId, mainImageId.Value);
                            }

                            if (gitHubIt.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByPortfolioId(portfolioId, gitHubIt.Value);
                            }

                            if (linkedInId.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByPortfolioId(portfolioId, linkedInId.Value);
                            }

                            DeleteAdditionalImagesByPortfolioId(portfolioId);
                            DeleteHobbiesByPortfolioId(portfolioId);
                            DeleteTechSkillsByPortfolioId(portfolioId);
                            DeleteWorkExperiencesByPortfolioId(portfolioId);
                            DeleteEducationsByPortfolioId(portfolioId);
                            DeleteCredentialsByPortfolioId(portfolioId);
                            DeleteVolunteerWorksByPortfolioId(portfolioId);
                            DeleteOpenSourceContributionsByPortfolioId(portfolioId);
                            DeleteSideProjectsByPortfolioId(portfolioId);


                            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                            {
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Transaction = transaction;

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the portfolio.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        /*  
            **********************************************************************************************
                                        PORTFOLIO HELPER METHODS
            **********************************************************************************************
        */

        private int? GetMainImageIdByPortfolioId(int portfolioId)
        {
            string sql = "SELECT main_image_id FROM portfolios WHERE id = @portfolioId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Main Image ID by Portfolio ID.", ex);
            }
        }

        private int? GetGitHubIdByPortfolioId(int portfolioId)
        {
            string sql = "SELECT github_repo_link_id FROM portfolios WHERE id = @portfolioId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the GitHub ID by Portfolio ID.", ex);
            }
        }

        private int? GetLinkedInIdByPortfolioId(int portfolioId)
        {
            string sql = "SELECT linkedin_id FROM portfolios WHERE id = @portfolioId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the LinkedIn ID by Portfolio ID.", ex);
            }
        }

        private int DeleteHobbiesByPortfolioId(int portfolioId)
        {
            List<Hobby> hobbies = _hobbyDao.GetHobbiesByPortfolioId(portfolioId);

            int hobbiesDeletedCount = 0;

            foreach (Hobby hobby in hobbies)
            {
                int hobbyId = hobby.Id;

                try
                {
                    _hobbyDao.DeleteHobbyByPortfolioId(portfolioId, hobbyId);
                    hobbiesDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the hobby with ID {hobbyId} for portfolio with ID {portfolioId}: {ex.Message}");
                }
            }

            return hobbiesDeletedCount;
        }

        private int DeleteTechSkillsByPortfolioId(int portfolioId)
        {
            List<Skill> skills = _skillDao.GetSkillsByPortfolioId(portfolioId);

            int skillsDeletedCount = 0;

            foreach (Skill skill in skills)
            {
                int skillId = skill.Id;

                try
                {
                    _skillDao.DeleteSkillByPortfolioId(portfolioId, skillId);
                    skillsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the skill with ID {skillId} for portfolio with ID {portfolioId}: {ex.Message}");
                }
            }

            return skillsDeletedCount;
        }

        private int DeleteSideProjectsByPortfolioId(int portfolioId)
        {
            List<SideProject> sideProjects = _sideProjectDao.GetSideProjectsByPortfolioId(portfolioId);

            int sideProjectsDeletedCount = 0;

            foreach (SideProject sideProject in sideProjects)
            {
                int sideProjectId = sideProject.Id;

                try
                {
                    _sideProjectDao.DeleteSideProjectByPortfolioId(portfolioId, sideProjectId);
                    sideProjectsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the side project with ID {sideProjectId} for portfolio with ID {portfolioId}: {ex.Message}");
                }
            }

            return sideProjectsDeletedCount;
        }

        private int DeleteWorkExperiencesByPortfolioId(int portfolioId)
        {
            List<WorkExperience> workExperiences = _workExperienceDao.GetWorkExperiencesByPortfolioId(portfolioId);

            int workExperiencesDeletedCount = 0;

            foreach (WorkExperience workExperience in workExperiences)
            {
                int workExperienceId = workExperience.Id;

                try
                {
                    _workExperienceDao.DeleteWorkExperienceByPortfolioId(portfolioId, workExperienceId);
                    workExperiencesDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the work experience with ID {workExperienceId} for portfolio with ID {portfolioId}: {ex.Message}");
                }
            }

            return workExperiencesDeletedCount;
        }

        private int DeleteEducationsByPortfolioId(int portfolioId)
        {
            List<Education> educations = _educationDao.GetEducationsByPortfolioId(portfolioId);

            int educationsDeletedCount = 0;

            foreach (Education education in educations)
            {
                int educationId = education.Id;

                try
                {
                    _educationDao.DeleteEducationByPortfolioId(portfolioId, educationId);
                    educationsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the education with ID {educationId} for portfolio with ID {portfolioId}: {ex.Message}");
                }
            }

            return educationsDeletedCount;
        }

        private int DeleteCredentialsByPortfolioId(int portfolioId)
        {
            List<Credential> credentials = _credentialDao.GetCredentialsByPortfolioId(portfolioId);

            int credentialsDeletedCount = 0;

            foreach (Credential credential in credentials)
            {
                int credentialId = credential.Id;

                try
                {
                    _credentialDao.DeleteCredentialByPortfolioId(portfolioId, credentialId);
                    credentialsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the credential with ID {credentialId} for portfolio with ID {portfolioId}: {ex.Message}");
                }
            }

            return credentialsDeletedCount;
        }

        private int DeleteVolunteerWorksByPortfolioId(int portfolioId)
        {
            List<VolunteerWork> volunteerWorks = _volunteerWorkDao.GetVolunteerWorksByPortfolioId(portfolioId);

            int volunteerWorksDeletedCount = 0;

            foreach (VolunteerWork volunteerWork in volunteerWorks)
            {
                int volunteerWorkId = volunteerWork.Id;

                try
                {
                    _volunteerWorkDao.DeleteVolunteerWorkByPortfolioId(portfolioId, volunteerWorkId);
                    volunteerWorksDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the volunteer work with ID {volunteerWorkId} for portfolio with ID {portfolioId}: {ex.Message}");
                }
            }

            return volunteerWorksDeletedCount;
        }

        private int DeleteOpenSourceContributionsByPortfolioId(int portfolioId)
        {
            List<OpenSourceContribution> openSourceContributions = _openSourceContributionDao.GetOpenSourceContributionsByPortfolioId(portfolioId);

            int openSourceContributionsDeletedCount = 0;

            foreach (OpenSourceContribution openSourceContribution in openSourceContributions)
            {
                int openSourceContributionId = openSourceContribution.Id;

                try
                {
                    _openSourceContributionDao.DeleteOpenSourceContributionByPortfolioId(portfolioId, openSourceContributionId);
                    openSourceContributionsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the open source contribution with ID {openSourceContributionId} for portfolio with ID {portfolioId}: {ex.Message}");
                }
            }

            return openSourceContributionsDeletedCount;
        }

        private int DeleteAdditionalImagesByPortfolioId(int portfolioId)
        {
            List<Image> additionalImages = _imageDao.GetAdditionalImagesByPortfolioId(portfolioId);

            int additionalImagesDeletedCount = 0;

            foreach (Image image in additionalImages)
            {
                int imageId = image.Id;

                try
                {
                    _imageDao.DeleteImageByPortfolioId(portfolioId, imageId);
                    additionalImagesDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the additional image with ID {imageId} for portfolio with ID {portfolioId}: {ex.Message}");
                }
            }

            return additionalImagesDeletedCount;
        }

        /*  
            **********************************************************************************************
                                            PORTFOLIO MAP ROW
            **********************************************************************************************
        */

        private Portfolio MapRowToPortfolio(NpgsqlDataReader reader)
        {
            Portfolio portfolio = new Portfolio
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Location = Convert.ToString(reader["location"]),
                ProfessionalSummary = Convert.ToString(reader["professional_summary"]),
                Email = Convert.ToString(reader["email"])
            };

            int portfolioId = portfolio.Id;

            SetPortfolioMainImageIdProperties(reader, portfolio, portfolioId);
            SetPortfolioGitHubRepoLinkIdProperties(reader, portfolio, portfolioId);
            SetPortfolioLinkedInIdProperties(reader, portfolio, portfolioId);

            portfolio.Hobbies = _hobbyDao.GetHobbiesByPortfolioId(portfolioId);
            portfolio.TechSkills = _skillDao.GetSkillsByPortfolioId(portfolioId);
            portfolio.SideProjects = _sideProjectDao.GetSideProjectsByPortfolioId(portfolioId);
            portfolio.BackgroundExperiences = _workExperienceDao.GetWorkExperiencesByPortfolioId(portfolioId);
            portfolio.EducationHistory = _educationDao.GetEducationsByPortfolioId(portfolioId);
            portfolio.CertificationsAndCredentials = _credentialDao.GetCredentialsByPortfolioId(portfolioId);
            portfolio.VolunteerWorks = _volunteerWorkDao.GetVolunteerWorksByPortfolioId(portfolioId);
            portfolio.OpenSourceContributions = _openSourceContributionDao.GetOpenSourceContributionsByPortfolioId(portfolioId);
            portfolio.AdditionalImages = _imageDao.GetAdditionalImagesByPortfolioId(portfolioId);

            return portfolio;
        }

        private void SetPortfolioMainImageIdProperties(NpgsqlDataReader reader, Portfolio portfolio, int portfolioId)
        {
            if (reader["main_image_id"] != DBNull.Value)
            {
                portfolio.MainImageId = Convert.ToInt32(reader["main_image_id"]);

                portfolio.MainImage = _imageDao.GetMainImageByPortfolioId(portfolioId);
            }
            else
            {
                portfolio.MainImageId = 0;
            }
        }

        private void SetPortfolioGitHubRepoLinkIdProperties(NpgsqlDataReader reader, Portfolio portfolio, int portfolioId)
        {
            if (reader["github_repo_link_id"] != DBNull.Value)
            {
                portfolio.GitHubId = Convert.ToInt32(reader["github_repo_link_id"]);

                portfolio.GitHub = _websiteDao.GetWebsiteByPortfolioId(portfolioId, portfolio.GitHubId);
            }
            else
            {
                portfolio.GitHubId = 0;
            }
        }

        private void SetPortfolioLinkedInIdProperties(NpgsqlDataReader reader, Portfolio portfolio, int portfolioId)
        {
            if (reader["linkedin_id"] != DBNull.Value)
            {
                portfolio.LinkedInId = Convert.ToInt32(reader["linkedin_id"]);

                portfolio.LinkedIn = _websiteDao.GetWebsiteByPortfolioId(portfolioId, portfolio.LinkedInId);
            }
            else
            {
                portfolio.LinkedInId = 0;
            }
        }
    }
}