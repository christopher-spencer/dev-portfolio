using System;
using System.Collections.Generic;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class WorkExperiencePostgresDao : IWorkExperienceDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;
        private readonly ISkillDao _skillDao;
        private readonly IWebsiteDao _websiteDao;
        private readonly IAchievementDao _achievementDao;

        public WorkExperiencePostgresDao(string dbConnectionString, IImageDao imageDao, ISkillDao skillDao, IWebsiteDao websiteDao, IAchievementDao achievementDao)
        {
            connectionString = dbConnectionString;
            _imageDao = imageDao;
            _skillDao = skillDao;
            _websiteDao = websiteDao;
            _achievementDao = achievementDao;
        }

        /*  
            **********************************************************************************************
                                               WORK EXPERIENCE CRUD
            **********************************************************************************************
        */

        // public WorkExperience CreateWorkExperience(WorkExperience experience)
        // {
        //     if (string.IsNullOrEmpty(experience.PositionTitle))
        //     {
        //         throw new ArgumentException("Position Title is required to create a Work Experience.");
        //     }

        //     if (string.IsNullOrEmpty(experience.CompanyName))
        //     {
        //         throw new ArgumentException("Company Name is required to create a Work Experience.");
        //     }

        //     if (string.IsNullOrEmpty(experience.Location))
        //     {
        //         throw new ArgumentException("Location is required to create a Work Experience.");
        //     }

        //     if (experience.StartDate == DateTime.MinValue || experience.StartDate > DateTime.Now)
        //     {
        //         throw new ArgumentException("Start Date must be a valid date in the past or present to create a Work Experience.");
        //     }

        //     string sql = "INSERT INTO work_experiences (position_title, company_name, location, description, start_date, " +
        //                  "end_date) " +
        //                  "VALUES (@positionTitle, @companyName, @location, @description, @startDate, @endDate) " +
        //                  "RETURNING id;";

        //     try
        //     {
        //         using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        //         {
        //             connection.Open();

        //             using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
        //             {
        //                 cmd.Parameters.AddWithValue("@positionTitle", experience.PositionTitle);
        //                 cmd.Parameters.AddWithValue("@companyName", experience.CompanyName);
        //                 cmd.Parameters.AddWithValue("@location", experience.Location);
        //                 cmd.Parameters.AddWithValue("@description", experience.Description);
        //                 cmd.Parameters.AddWithValue("@startDate", experience.StartDate);
        //                 cmd.Parameters.AddWithValue("@endDate", experience.EndDate);

        //                 int experienceId = Convert.ToInt32(cmd.ExecuteScalar());

        //                 experience.Id = experienceId;
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new DaoException("An error occurred while creating the Work Experience.", ex);
        //     }

        //     return experience;
        // }

        public List<WorkExperience> GetWorkExperiences()
        {
            List<WorkExperience> experiences = new List<WorkExperience>();

            string sql = "SELECT id, position_title, company_name, company_logo_id, company_website_id, " +
                         "location, description, start_date, end_date, main_image_id " +
                         "FROM work_experiences;";

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
                                WorkExperience experience = MapRowToWorkExperience(reader);

                                experiences.Add(experience);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DaoException("An error occurred while retrieving Work Experiences.", ex);
            }

            return experiences;
        }

        public WorkExperience GetWorkExperience(int experienceId)
        {
            WorkExperience experience = null;

            string sql = "SELECT id, position_title, company_name, company_logo_id, company_website_id, " +
                         "location, description, start_date, end_date, main_image_id " +
                         "FROM work_experiences " +
                         "WHERE id = @experienceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                experience = MapRowToWorkExperience(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DaoException("An error occurred while retrieving the Work Experience.", ex);
            }

            return experience;
        }

        // public WorkExperience UpdateWorkExperience(int experienceId, WorkExperience experience)
        // {
        //     if (string.IsNullOrEmpty(experience.PositionTitle))
        //     {
        //         throw new ArgumentException("Position Title is required to update a Work Experience.");
        //     }

        //     if (string.IsNullOrEmpty(experience.CompanyName))
        //     {
        //         throw new ArgumentException("Company Name is required to update a Work Experience.");
        //     }

        //     if (string.IsNullOrEmpty(experience.Location))
        //     {
        //         throw new ArgumentException("Location is required to update a Work Experience.");
        //     }

        //     if (experience.StartDate == DateTime.MinValue || experience.StartDate > DateTime.Now)
        //     {
        //         throw new ArgumentException("Start Date must be a valid date in the past or present to update a Work Experience.");
        //     }

        //     string sql = "UPDATE work_experiences " +
        //                  "SET position_title = @positionTitle, company_name = @companyName, location = @location, " +
        //                  "description = @description, start_date = @startDate, end_date = @endDate " +
        //                  "WHERE id = @experienceId;";

        //     try
        //     {
        //         using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        //         {
        //             connection.Open();

        //             using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
        //             {
        //                 cmd.Parameters.AddWithValue("@positionTitle", experience.PositionTitle);
        //                 cmd.Parameters.AddWithValue("@companyName", experience.CompanyName);
        //                 cmd.Parameters.AddWithValue("@location", experience.Location);
        //                 cmd.Parameters.AddWithValue("@description", experience.Description);
        //                 cmd.Parameters.AddWithValue("@startDate", experience.StartDate);
        //                 cmd.Parameters.AddWithValue("@endDate", experience.EndDate);
        //                 cmd.Parameters.AddWithValue("@experienceId", experienceId);

        //                 int count = cmd.ExecuteNonQuery();

        //                 if (count == 1)
        //                 {
        //                     return experience;
        //                 }
        //                 else
        //                 {
        //                     return null;
        //                 }
        //             }
        //         }
        //     }
        //     catch (NpgsqlException ex)
        //     {
        //         throw new DaoException("An error occurred while updating the Work Experience.", ex);
        //     }
        // }

        // public int DeleteWorkExperience(int experienceId)
        // {
        //     if (experienceId <= 0)
        //     {
        //         throw new ArgumentException("Experience ID must be greater than zero.");
        //     }

        //     string sql = "DELETE FROM work_experiences WHERE id = @experienceId;";

        //     try
        //     {
        //         using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        //         {
        //             connection.Open();

        //             using (NpgsqlTransaction transaction = connection.BeginTransaction())
        //             {
        //                 try
        //                 {
        //                     int rowsAffected;

        //                     int? companyLogoId = GetCompanyLogoIdByWorkExperienceId(experienceId);
        //                     int? companyWebsiteId = GetCompanyWebsiteIdByWorkExperienceId(experienceId);
        //                     int? mainImageId = GetMainImageIdByWorkExperienceId(experienceId);

        //                     if (companyLogoId.HasValue)
        //                     {
        //                         _imageDao.DeleteImageByWorkExperienceId(experienceId, companyLogoId.Value);
        //                     }

        //                     if (companyWebsiteId.HasValue)
        //                     {
        //                         _websiteDao.DeleteWebsiteByWorkExperienceId(experienceId, companyWebsiteId.Value);
        //                     }

        //                     if (mainImageId.HasValue)
        //                     {
        //                         _imageDao.DeleteImageByWorkExperienceId(experienceId, mainImageId.Value);
        //                     }

        //                     DeleteResponsibilitiesAndAchievementsByWorkExperienceId(experienceId);
        //                     DeleteSkillsUsedOrObtainedByWorkExperienceId(experienceId);
        //                     DeleteAdditionalImagesByWorkExperienceId(experienceId);

        //                     using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
        //                     {
        //                         cmd.Transaction = transaction;
        //                         cmd.Parameters.AddWithValue("@experienceId", experienceId);

        //                         rowsAffected = cmd.ExecuteNonQuery();
        //                     }

        //                     transaction.Commit();

        //                     return rowsAffected;
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     Console.WriteLine(ex.ToString());

        //                     transaction.Rollback();

        //                     throw new DaoException("An error occurred while deleting the Work Experience.", ex);
        //                 }

        //             }
        //         }
        //     }
        //     catch (NpgsqlException ex)
        //     {
        //         throw new DaoException("An error occurred while connecting to the database.", ex);
        //     }
        // }

        /*  
            **********************************************************************************************
                                           PORTFOLIO WORK EXPERIENCE CRUD
            **********************************************************************************************
        */

        public WorkExperience CreateWorkExperienceByPortfolioId(int portfolioId, WorkExperience experience)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(experience.PositionTitle))
            {
                throw new ArgumentException("Position Title is required to create a Work Experience.");
            }

            if (string.IsNullOrEmpty(experience.CompanyName))
            {
                throw new ArgumentException("Company Name is required to create a Work Experience.");
            }

            if (string.IsNullOrEmpty(experience.Location))
            {
                throw new ArgumentException("Location is required to create a Work Experience.");
            }

            if (experience.StartDate == DateTime.MinValue || experience.StartDate > DateTime.Now)
            {
                throw new ArgumentException("Start Date must be a valid date in the past or present to create a Work Experience.");
            }

            string insertExperienceSql = "INSERT INTO work_experiences (position_title, company_name, location, description, " +
                                         "start_date, end_date) " +
                                         "VALUES (@positionTitle, @companyName, @location, @description, @startDate, @endDate) " +
                                         "RETURNING id;";

            string insertPortfolioEducationSql = "INSERT INTO portfolio_work_experiences (portfolio_id, experience_id) " +
                                                 "VALUES (@portfolioId, @experienceId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int experienceId;

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertExperienceSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@positionTitle", experience.PositionTitle);
                                cmd.Parameters.AddWithValue("@companyName", experience.CompanyName);
                                cmd.Parameters.AddWithValue("@location", experience.Location);
                                cmd.Parameters.AddWithValue("@description", experience.Description);
                                cmd.Parameters.AddWithValue("@startDate", experience.StartDate);
                                cmd.Parameters.AddWithValue("@endDate", experience.EndDate);
                                cmd.Transaction = transaction;

                                experienceId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            using (NpgsqlCommand portfolioCmd = new NpgsqlCommand(insertPortfolioEducationSql, connection))
                            {
                                portfolioCmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                portfolioCmd.Parameters.AddWithValue("@experienceId", experienceId);
                                portfolioCmd.Transaction = transaction;
                                portfolioCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            experience.Id = experienceId;

                            return experience;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the Work Experience for the Portfolio.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);

            }
        }

        public List<WorkExperience> GetWorkExperiencesByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            List<WorkExperience> experiences = new List<WorkExperience>();

            string sql = "SELECT we.id, we.position_title, we.company_name, we.company_logo_id, we.company_website_id, " +
                         "we.location, we.description, we.start_date, we.end_date, we.main_image_id " +
                         "FROM work_experiences we " +
                         "JOIN portfolio_work_experiences pwe ON we.id = pwe.experience_id " +
                         "WHERE pwe.portfolio_id = @portfolioId;";

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
                            while (reader.Read())
                            {
                                WorkExperience experience = MapRowToWorkExperience(reader);

                                experiences.Add(experience);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DaoException("An error occurred while retrieving Work Experiences for the Portfolio.", ex);
            }

            return experiences;
        }

        public WorkExperience GetWorkExperienceByPortfolioId(int portfolioId, int experienceId)
        {
            if (portfolioId <= 0 || experienceId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Experience ID must be greater than zero.");
            }

            WorkExperience experience = null;

            string sql = "SELECT we.id, we.position_title, we.company_name, we.company_logo_id, we.company_website_id, " +
                         "we.location, we.description, we.start_date, we.end_date, we.main_image_id " +
                         "FROM work_experiences we " +
                         "JOIN portfolio_work_experiences pwe ON we.id = pwe.experience_id " +
                         "WHERE pwe.portfolio_id = @portfolioId AND we.id = @experienceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                experience = MapRowToWorkExperience(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DaoException("An error occurred while retrieving the Work Experience for the Portfolio.", ex);
            }

            return experience;
        }

        public WorkExperience UpdateWorkExperienceByPortfolioId(int portfolioId, int experienceId, WorkExperience experience)
        {
            if (portfolioId <= 0 || experienceId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Experience ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(experience.PositionTitle))
            {
                throw new ArgumentException("Position Title is required to update a Work Experience.");
            }

            if (string.IsNullOrEmpty(experience.CompanyName))
            {
                throw new ArgumentException("Company Name is required to update a Work Experience.");
            }

            if (string.IsNullOrEmpty(experience.Location))
            {
                throw new ArgumentException("Location is required to update a Work Experience.");
            }

            if (experience.StartDate == DateTime.MinValue || experience.StartDate > DateTime.Now)
            {
                throw new ArgumentException("Start Date must be a valid date in the past or present to update a Work Experience.");
            }

            string sql = "UPDATE work_experiences " +
                         "SET position_title = @positionTitle, company_name = @companyName, location = @location, " +
                         "description = @description, start_date = @startDate, end_date = @endDate " +
                         "FROM portfolio_work_experiences " +
                         "WHERE work_experiences.id = portfolio_work_experiences.experience_id " +
                         "AND portfolio_work_experiences.portfolio_id = @portfolioId " +
                         "AND work_experiences.id = @experienceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@positionTitle", experience.PositionTitle);
                        cmd.Parameters.AddWithValue("@companyName", experience.CompanyName);
                        cmd.Parameters.AddWithValue("@location", experience.Location);
                        cmd.Parameters.AddWithValue("@description", experience.Description);
                        cmd.Parameters.AddWithValue("@startDate", experience.StartDate);
                        cmd.Parameters.AddWithValue("@endDate", experience.EndDate);
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return experience;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the work experience by portfolio ID.", ex);
            }

            return null;
        }

        public int DeleteWorkExperienceByPortfolioId(int portfolioId, int experienceId)
        {
            if (portfolioId <= 0 || experienceId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Experience ID must be greater than zero.");
            }

            string deletePortfolioExperienceSql = "DELETE FROM portfolio_work_experiences WHERE portfolio_id = @portfolioId " +
                                                  "AND experience_id = @experienceId;";

            string deleteExperienceSql = "DELETE FROM work_experiences WHERE id = @experienceId;";

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

                            int? companyLogoId = GetCompanyLogoIdByWorkExperienceId(experienceId);
                            int? companyWebsiteId = GetCompanyWebsiteIdByWorkExperienceId(experienceId);
                            int? mainImageId = GetMainImageIdByWorkExperienceId(experienceId);

                            if (companyLogoId.HasValue)
                            {
                                _imageDao.DeleteImageByWorkExperienceId(experienceId, companyLogoId.Value);
                            }

                            if (companyWebsiteId.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByWorkExperienceId(experienceId, companyWebsiteId.Value);
                            }

                            if (mainImageId.HasValue)
                            {
                                _imageDao.DeleteImageByWorkExperienceId(experienceId, mainImageId.Value);
                            }

                            DeleteResponsibilitiesAndAchievementsByWorkExperienceId(experienceId);
                            DeleteSkillsUsedOrObtainedByWorkExperienceId(experienceId);
                            DeleteAdditionalImagesByWorkExperienceId(experienceId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deletePortfolioExperienceSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@experienceId", experienceId);
                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteExperienceSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@experienceId", experienceId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the Work Experience by Portfolio ID.", ex);
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
                                          WORK EXPERIENCE HELPER METHODS
            **********************************************************************************************
        */

        private int? GetMainImageIdByWorkExperienceId(int experienceId)
        {
            string sql = "SELECT main_image_id FROM work_experiences WHERE id = @experienceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);

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
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving MainImage ID by Work Experience ID: " + ex.Message);
                return null;
            }
        }

        private int? GetCompanyLogoIdByWorkExperienceId(int experienceId)
        {
            string sql = "SELECT company_logo_id FROM work_experiences WHERE id = @experienceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);

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
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving CompanyLogo ID by Work Experience ID: " + ex.Message);
                return null;
            }
        }

        private int? GetCompanyWebsiteIdByWorkExperienceId(int experienceId)
        {
            string sql = "SELECT company_website_id FROM work_experiences WHERE id = @experienceId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@experienceId", experienceId);

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
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving CompanyWebsite ID by Work Experience ID: " + ex.Message);
                return null;
            }
        }

        private int DeleteResponsibilitiesAndAchievementsByWorkExperienceId(int experienceId)
        {
            List<Achievement> achievements = _achievementDao.GetAchievementsByWorkExperienceId(experienceId);

            int achievementsDeletedCount = 0;

            foreach (Achievement achievement in achievements)
            {
                int achievementId = achievement.Id;

                try
                {
                    _achievementDao.DeleteAchievementByWorkExperienceId(experienceId, achievementId);

                    achievementsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting Achievement by Work Experience ID: " + ex.Message);
                }
            }

            return achievementsDeletedCount;
        }

        private int DeleteSkillsUsedOrObtainedByWorkExperienceId(int experienceId)
        {
            List<Skill> skills = _skillDao.GetSkillsByWorkExperienceId(experienceId);

            int skillsDeletedCount = 0;

            foreach (Skill skill in skills)
            {
                int skillId = skill.Id;

                try
                {
                    _skillDao.DeleteSkillByWorkExperienceId(experienceId, skillId);

                    skillsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting Skill by Work Experience ID: " + ex.Message);
                }
            }

            return skillsDeletedCount;
        }

        private int DeleteAdditionalImagesByWorkExperienceId(int experienceId)
        {
            List<Image> additionalImages = _imageDao.GetAdditionalImagesByWorkExperienceId(experienceId);

            int imagesDeletedCount = 0;

            foreach (Image image in additionalImages)
            {
                int imageId = image.Id;

                try
                {
                    _imageDao.DeleteImageByWorkExperienceId(experienceId, imageId);

                    imagesDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting Additional Image by Work Experience ID: " + ex.Message);
                }
            }

            return imagesDeletedCount;
        }

        /*  
            **********************************************************************************************
                                               WORK EXPERIENCE MAP ROW
            **********************************************************************************************
        */

        private WorkExperience MapRowToWorkExperience(NpgsqlDataReader reader)
        {
            WorkExperience experience = new WorkExperience
            {
                Id = Convert.ToInt32(reader["id"]),
                PositionTitle = Convert.ToString(reader["position_title"]),
                CompanyName = Convert.ToString(reader["company_name"]),
                Location = Convert.ToString(reader["location"]),
                Description = Convert.ToString(reader["description"]),
                StartDate = Convert.ToDateTime(reader["start_date"]),
                EndDate = Convert.ToDateTime(reader["end_date"])
            };

            int experienceId = experience.Id;

            SetWorkExperienceCompanyLogoIdProperties(reader, experience, experienceId);
            SetWorkExperienceCompanyWebsiteIdProperties(reader, experience, experienceId);
            SetWorkExperienceMainImageIdProperties(reader, experience, experienceId);

            experience.ResponsibilitiesAndAchievements = _achievementDao.GetAchievementsByWorkExperienceId(experienceId);
            experience.SkillsUsedOrObtained = _skillDao.GetSkillsByWorkExperienceId(experienceId);
            experience.AdditionalImages = _imageDao.GetAdditionalImagesByWorkExperienceId(experienceId);

            return experience;
        }

        private void SetWorkExperienceCompanyLogoIdProperties(NpgsqlDataReader reader, WorkExperience experience, int experienceId)
        {
            if (reader["company_logo_id"] != DBNull.Value)
            {
                experience.CompanyLogoId = Convert.ToInt32(reader["company_logo_id"]);

                experience.CompanyLogo = _imageDao.GetImageByWorkExperienceId(experienceId, experience.CompanyLogoId);
            }
            else
            {
                experience.CompanyLogoId = 0;
            }
        }

        private void SetWorkExperienceCompanyWebsiteIdProperties(NpgsqlDataReader reader, WorkExperience experience, int experienceId)
        {
            if (reader["company_website_id"] != DBNull.Value)
            {
                experience.CompanyWebsiteId = Convert.ToInt32(reader["company_website_id"]);

                experience.CompanyWebsite = _websiteDao.GetWebsiteByWorkExperienceId(experienceId);
            }
            else
            {
                experience.CompanyWebsiteId = 0;
            }
        }

        private void SetWorkExperienceMainImageIdProperties(NpgsqlDataReader reader, WorkExperience experience, int experienceId)
        {
            if (reader["main_image_id"] != DBNull.Value)
            {
                experience.MainImageId = Convert.ToInt32(reader["main_image_id"]);

                experience.MainImage = _imageDao.GetImageByWorkExperienceId(experienceId, experience.MainImageId);
            }
            else
            {
                experience.MainImageId = 0;
            }
        }


    }
}