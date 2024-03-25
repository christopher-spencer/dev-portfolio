using System;
using System.Collections.Generic;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class WorkExperiencePostgresDao: IWorkExperienceDao
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

        /*  
            **********************************************************************************************
                                           PORTFOLIO WORK EXPERIENCE CRUD
            **********************************************************************************************
        */

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