using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class EducationPostgresDao : IEducationDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;
        private readonly IWebsiteDao _websiteDao;
        private readonly IAchievementDao _achievementDao;


        public EducationPostgresDao(string dbConnectionString, IImageDao imageDao, IWebsiteDao websiteDao, IAchievementDao achievementDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
            this._websiteDao = websiteDao;
            this._achievementDao = achievementDao;
        }

        const string MainImage = "main image";
        const string AdditionalImage = "additional image";
        const string Logo = "logo";

        /*  
            **********************************************************************************************
                                                EDUCATION CRUD
            **********************************************************************************************
        */

        public List<Education> GetEducations()
        {
            List<Education> educations = new List<Education>();

            string sql = "SELECT id, institution_name, institution_logo_id, institution_website_id, location, description, field_of_study, " +
                         "major, minor, degree_obtained, gpa_overall, gpa_in_major, start_date, graduation_date, main_image_id " +
                         "FROM educations;";

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
                                Education education = MapRowToEducation(reader);
                                educations.Add(education);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving all Educations.", ex);
            }

            return educations;
        }

        public Education GetEducation(int educationId)
        {
            if (educationId <= 0)
            {
                throw new ArgumentException("Education ID must be greater than zero.");
            }

            Education education = null;

            string sql = "SELECT id, institution_name, institution_logo_id, institution_website_id, location, description, field_of_study, " +
                         "major, minor, degree_obtained, gpa_overall, gpa_in_major, start_date, graduation_date, main_image_id " +
                         "FROM educations WHERE id = @educationId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                education = MapRowToEducation(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the education.", ex);
            }

            return education;
        }

        /*  
            **********************************************************************************************
                                            PORTFOLIO EDUCATION CRUD
            **********************************************************************************************
        */

        public Education CreateEducationByPortfolioId(int portfolioId, Education education)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            CheckEducationNameLocationAndStartDateAreNotNullOrEmpty(education);

            string insertEducationSql = "INSERT INTO educations (institution_name, location, description, field_of_study, major, minor, " +
                         "degree_obtained, gpa_overall, gpa_in_major, start_date, graduation_date) " +
                         "VALUES (@institutionName, @location, @description, @fieldOfStudy, @major, @minor, @degreeObtained, " +
                         "@gpaOverall, @gpaInMajor, @startDate, @graduationDate) " +
                         "RETURNING id;";

            string insertPortfolioEducationSql = "INSERT INTO portfolio_educations (portfolio_id, education_id) VALUES (@portfolioId, @educationId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int educationId;

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertEducationSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@institutionName", education.InstitutionName);
                                cmd.Parameters.AddWithValue("@location", education.Location);
                                cmd.Parameters.AddWithValue("@description", education.Description);
                                cmd.Parameters.AddWithValue("@fieldOfStudy", education.FieldOfStudy);
                                cmd.Parameters.AddWithValue("@major", (object)education.Major ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@minor", (object)education.Minor ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@degreeObtained", education.DegreeObtained);
                                cmd.Parameters.AddWithValue("@gpaOverall", education.GPAOverall);
                                cmd.Parameters.AddWithValue("@gpaInMajor", education.GPAInMajor);
                                cmd.Parameters.AddWithValue("@startDate", education.StartDate);
                                cmd.Parameters.AddWithValue("@graduationDate", education.GraduationDate);
                                cmd.Transaction = transaction;

                                educationId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertPortfolioEducationSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@educationId", educationId);
                                cmd.Transaction = transaction;
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            education.Id = educationId;

                            return education;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the Education for the Portfolio.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Education> GetEducationsByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            List<Education> educations = new List<Education>();

            string sql = "SELECT e.id, e.institution_name, e.institution_logo_id, e.institution_website_id, e.location, e.description, e.field_of_study, " +
                         "e.major, e.minor, e.degree_obtained, e.gpa_overall, e.gpa_in_major, e.start_date, e.graduation_date, e.main_image_id " +
                         "FROM educations e " +
                         "JOIN portfolio_educations pe ON e.id = pe.education_id " +
                         "WHERE pe.portfolio_id = @portfolioId;";

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
                                Education education = MapRowToEducation(reader);
                                educations.Add(education);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving all Educations for the Portfolio.", ex);
            }

            return educations;
        }

        public Education GetEducationByPortfolioId(int portfolioId, int educationId)
        {
            if (portfolioId <= 0 || educationId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Education ID must be greater than zero.");
            }

            Education education = null;

            string sql = "SELECT e.id, e.institution_name, e.institution_logo_id, e.institution_website_id, e.location, e.description, e.field_of_study, " +
                         "e.major, e.minor, e.degree_obtained, e.gpa_overall, e.gpa_in_major, e.start_date, e.graduation_date, e.main_image_id " +
                         "FROM educations e " +
                         "JOIN portfolio_educations pe ON e.id = pe.education_id " +
                         "WHERE pe.portfolio_id = @portfolioId AND e.id = @educationId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@educationId", educationId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                education = MapRowToEducation(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the education for the Portfolio.", ex);
            }

            return education;
        }

        public Education UpdateEducationByPortfolioId(int portfolioId, int educationId, Education education)
        {
            if (portfolioId <= 0 || educationId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Education ID must be greater than zero.");
            }

            CheckEducationNameLocationAndStartDateAreNotNullOrEmpty(education);

            string sql = "UPDATE educations SET institution_name = @institutionName, location = @location, description = @description, " +
                         "field_of_study = @fieldOfStudy, major = @major, minor = @minor, degree_obtained = @degreeObtained, " +
                         "gpa_overall = @gpaOverall, gpa_in_major = @gpaInMajor, start_date = @startDate, graduation_date = @graduationDate " +
                         "FROM portfolio_educations pe " +
                         "WHERE pe.portfolio_id = @portfolioId AND pe.education_id = @educationId AND pe.education_id = educations.id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@educationId", educationId);
                        cmd.Parameters.AddWithValue("@institutionName", education.InstitutionName);
                        cmd.Parameters.AddWithValue("@location", education.Location);
                        cmd.Parameters.AddWithValue("@description", education.Description);
                        cmd.Parameters.AddWithValue("@fieldOfStudy", education.FieldOfStudy);
                        cmd.Parameters.AddWithValue("@major", education.Major);
                        cmd.Parameters.AddWithValue("@minor", education.Minor);
                        cmd.Parameters.AddWithValue("@degreeObtained", education.DegreeObtained);
                        cmd.Parameters.AddWithValue("@gpaOverall", education.GPAOverall);
                        cmd.Parameters.AddWithValue("@gpaInMajor", education.GPAInMajor);
                        cmd.Parameters.AddWithValue("@startDate", education.StartDate);
                        cmd.Parameters.AddWithValue("@graduationDate", education.GraduationDate);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return education;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the education by portfolio ID.", ex);
            }

            return null;
        }

        public int DeleteEducationByPortfolioId(int portfolioId, int educationId)
        {
            if (portfolioId <= 0 || educationId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Education ID must be greater than zero.");
            }
            
            string deletePortfolioEducationSql = "DELETE FROM portfolio_educations " +
                                                 "WHERE portfolio_id = @portfolioId " +
                                                 "AND education_id = @educationId;";
                                                 
            string deleteEducationSql = "DELETE FROM educations WHERE id = @educationId;";

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

                            int? institutionLogoId = GetInstitutionLogoIdByEducationId(educationId);
                            int? mainImageId = GetMainImageIdByEducationId(educationId);
                            int? institutionWebsiteId = GetWebsiteIdByEducationId(educationId);

                            if (institutionLogoId.HasValue)
                            {
                                _imageDao.DeleteImageByEducationId(educationId, institutionLogoId.Value);
                            }

                            if (mainImageId.HasValue)
                            {
                                _imageDao.DeleteImageByEducationId(educationId, mainImageId.Value);
                            }

                            if (institutionWebsiteId.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByEducationId(educationId, institutionWebsiteId.Value);
                            }

                            DeleteHonorsAndAwardsByEducationId(educationId);
                            DeleteAdditionalImagesByEducationId(educationId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deletePortfolioEducationSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@educationId", educationId);
                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteEducationSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@educationId", educationId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the Education by Portfolio ID.", ex);
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
                                          EDUCATION HELPER METHODS
            **********************************************************************************************
        */

        private int? GetMainImageIdByEducationId(int educationId)
        {
            string sql = "SELECT main_image_id FROM educations WHERE id = @educationId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);

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
                Console.WriteLine("Error retrieving MainImage ID by Education ID: " + ex.Message);
                return null;
            }
        }

        private int? GetInstitutionLogoIdByEducationId(int educationId)
        {
            string sql = "SELECT institution_logo_id FROM educations WHERE id = @educationId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);

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
                Console.WriteLine("Error retrieving Institution Logo ID by Education ID: " + ex.Message);
                return null;
            }
        }

        private int? GetWebsiteIdByEducationId(int educationId)
        {
            string sql = "SELECT institution_website_id FROM educations WHERE id = @educationId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@educationId", educationId);

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
                Console.WriteLine("Error retrieving Institution Website ID by Education ID: " + ex.Message);
                return null;
            }
        }

        private int DeleteHonorsAndAwardsByEducationId(int educationId)
        {
            List<Achievement> honorsAndAwards = _achievementDao.GetAchievementsByEducationId(educationId);

            int achievementsDeletedCount = 0;

            foreach (Achievement achievement in honorsAndAwards)
            {
                int achievementId = achievement.Id;

                try
                {
                    _achievementDao.DeleteAchievementByEducationId(educationId, achievementId);
                    achievementsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting Honor/Award (Achievement) from Education with Education ID:{educationId} and achievement ID:{achievementId}: {ex.Message}");
                }
            }

            return achievementsDeletedCount;
        }

        private int DeleteAdditionalImagesByEducationId(int educationId)
        {
            List<Image> additionalImages = _imageDao.GetAdditionalImagesByEducationId(educationId);

            int imagesDeletedCount = 0;

            foreach (Image image in additionalImages)
            {
                int imageId = image.Id;

                try
                {
                    _imageDao.DeleteImageByEducationId(educationId, imageId);
                    imagesDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting additional image from Education with education ID:{educationId} and image ID:{imageId}: {ex.Message}");
                }
            }

            return imagesDeletedCount;
        }

        /*  
            **********************************************************************************************
                                               EDUCATION MAP ROW
            **********************************************************************************************
        */

        private void CheckEducationNameLocationAndStartDateAreNotNullOrEmpty(Education education)
        {
            if (string.IsNullOrEmpty(education.InstitutionName))
            {
                throw new ArgumentException("Institution Name is required to create an Education.");
            }

            if (string.IsNullOrEmpty(education.Location))
            {
                throw new ArgumentException("Location is required to create an Education.");
            }

            if (education.StartDate == DateTime.MinValue || education.StartDate > DateTime.Now)
            {
                throw new ArgumentException("Start Date must be a valid date in the past or present to create an Education.");
            }
        }

        private Education MapRowToEducation(NpgsqlDataReader reader)
        {
            Education education = new Education
            {
                Id = Convert.ToInt32(reader["id"]),
                InstitutionName = Convert.ToString(reader["institution_name"]),
                Location = Convert.ToString(reader["location"]),
                Description = Convert.ToString(reader["description"]),
                FieldOfStudy = Convert.ToString(reader["field_of_study"]),
                Major = Convert.ToString(reader["major"]),
                Minor = Convert.ToString(reader["minor"]),
                DegreeObtained = Convert.ToString(reader["degree_obtained"]),
                GPAOverall = Convert.ToDecimal(reader["gpa_overall"]),
                GPAInMajor = Convert.ToDecimal(reader["gpa_in_major"]),
                StartDate = Convert.ToDateTime(reader["start_date"]),
                GraduationDate = Convert.ToDateTime(reader["graduation_date"])
            };

            int educationId = education.Id;

            SetEducationInstitutionLogoIdProperties(reader, education, educationId);
            SetEducationMainImageIdProperties(reader, education, educationId);
            SetEducationInstitutionWebsiteIdProperties(reader, education, educationId);

            education.HonorsAndAwards = _achievementDao.GetAchievementsByEducationId(educationId);
            education.AdditionalImages = _imageDao.GetAdditionalImagesByEducationId(educationId);

            return education;
        }

        private void SetEducationInstitutionLogoIdProperties(NpgsqlDataReader reader, Education education, int educationId)
        {
            if (reader["institution_logo_id"] != DBNull.Value)
            {
                education.InstitutionLogoId = Convert.ToInt32(reader["institution_logo_id"]);

                education.InstitutionLogo = _imageDao.GetMainImageOrInstitutionLogoByEducationId(educationId, Logo);
            }
            else
            {
                education.InstitutionLogoId = 0;
            }
        }

        private void SetEducationMainImageIdProperties(NpgsqlDataReader reader, Education education, int educationId)
        {
            if (reader["main_image_id"] != DBNull.Value)
            {
                education.MainImageId = Convert.ToInt32(reader["main_image_id"]);

                education.MainImage = _imageDao.GetMainImageOrInstitutionLogoByEducationId(educationId, MainImage);
            }
            else
            {
                education.MainImageId = 0;
            }
        }

        private void SetEducationInstitutionWebsiteIdProperties(NpgsqlDataReader reader, Education education, int educationId)
        {
            if (reader["institution_website_id"] != DBNull.Value)
            {
                education.InstitutionWebsiteId = Convert.ToInt32(reader["institution_website_id"]);

                education.InstitutionWebsite = _websiteDao.GetWebsiteByEducationId(educationId);
            }
            else
            {
                education.InstitutionWebsiteId = 0;
            }
        }



    }
}