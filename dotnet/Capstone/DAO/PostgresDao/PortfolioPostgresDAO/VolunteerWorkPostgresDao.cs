using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class VolunteerWorkPostgresDao : IVolunteerWorkDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;
        private readonly IWebsiteDao _websiteDao;
        private readonly IAchievementDao _achievementDao;
        private readonly ISkillDao _skillDao;


        public VolunteerWorkPostgresDao(string dbConnectionString, IImageDao imageDao, IWebsiteDao websiteDao, IAchievementDao achievementDao, ISkillDao skillDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
            this._websiteDao = websiteDao;
            this._achievementDao = achievementDao;
            this._skillDao = skillDao;
        }

        const string MainImage = "main image";
        const string AdditionalImage = "additional image";
        const string Logo = "logo";

        /*  
            **********************************************************************************************
                                            VOLUNTEER WORK CRUD
            **********************************************************************************************
        */

        /*  
            **********************************************************************************************
                                        PORTFOLIO VOLUNTEER WORK CRUD
            **********************************************************************************************
        */

        /*  
            **********************************************************************************************
                                        VOLUNTEER WORK HELPER METHODS
            **********************************************************************************************
        */

        private int? GetMainImageIdByVolunteerWorkId(int volunteerWorkdId)
        {
            string sql = "SELECT main_image_id FROM volunteer_works WHERE id = @volunteerWorkId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkdId);

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
                Console.WriteLine("Error retrieving MainImage ID by Volunteer Work ID: " + ex.Message);
                return null;
            }
        }

        private int? GetOrganizationLogoIdByVolunteerWorkId(int volunteerWorkId)
        {
            string sql = "SELECT organization_logo_id FROM volunteer_works WHERE id = @volunteerWorkId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);

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
                Console.WriteLine("Error retrieving Organization Logo ID by Volunteer Work ID: " + ex.Message);
                return null;
            }
        }

        private int? GetOrganizationWebsiteIdByVolunteerWorkId(int volunteerWorkId)
        {
            string sql = "SELECT organization_website_id FROM volunteer_works WHERE id = @volunteerWorkId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);

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
                Console.WriteLine("Error retrieving Organization Website ID by Volunteer Work ID: " + ex.Message);
                return null;
            }
        }

        private int DeleteResponsibilitiesAndAchievementsByVolunteerWorkId(int volunteerWorkId)
        {
            List<Achievement> achievements = _achievementDao.GetAchievementsByVolunteerWorkId(volunteerWorkId);

            int achievementsDeletedCount = 0;

            foreach (Achievement achievement in achievements)
            {
                int achievementId = achievement.Id;

                try
                {
                    _achievementDao.DeleteAchievementByVolunteerWorkId(achievementId, volunteerWorkId);
                    achievementsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting Achievement by Volunteer Work ID: " + ex.Message);
                }
            }

            return achievementsDeletedCount;
        }

        private int DeleteSkillsUsedAndObtainedByVolunteerWorkId(int volunteerWorkId)
        {
            List<Skill> skills = _skillDao.GetSkillsByVolunteerWorkId(volunteerWorkId);

            int skillsDeletedCount = 0;

            foreach (Skill skill in skills)
            {
                int skillId = skill.Id;

                try
                {
                    _skillDao.DeleteSkillByVolunteerWorkId(skillId, volunteerWorkId);
                    skillsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting Skill by Volunteer Work ID: " + ex.Message);
                }
            }

            return skillsDeletedCount;
        }

        private int DeleteAdditionalImagesByVolunteerWorkId(int volunteerWorkId)
        {
            List<Image> additionalImages = _imageDao.GetAdditionalImagesByVolunteerWorkId(volunteerWorkId);

            int additionalImagesDeletedCount = 0;

            foreach (Image image in additionalImages)
            {
                int imageId = image.Id;

                try
                {
                    _imageDao.DeleteImageByVolunteerWorkId(volunteerWorkId, imageId);
                    additionalImagesDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting Additional Image by Volunteer Work ID: " + ex.Message);
                }
            }

            return additionalImagesDeletedCount;
        }

        /*  
            **********************************************************************************************
                                            VOLUNTEER WORK MAP ROW
            **********************************************************************************************
        */

        private VolunteerWork MapRowToVolunteerWork(NpgsqlDataReader reader)
        {
            VolunteerWork volunteerWork = new VolunteerWork
            {
                Id = Convert.ToInt32(reader["id"]),
                OrganizationName = Convert.ToString(reader["organization_name"]),
                Location = Convert.ToString(reader["location"]),
                OrganizationDescription = Convert.ToString(reader["organization_description"]),
                PositionTitle = Convert.ToString(reader["position_title"]),
                StartDate = Convert.ToDateTime(reader["start_date"]),
                EndDate = Convert.ToDateTime(reader["end_date"])
            };

            int volunteerWorkId = volunteerWork.Id;

            SetVolunteerWorkOrganizationLogoIdProperties(reader, volunteerWork, volunteerWorkId);
            SetVolunteerWorkOrganizationWebsiteIdProperties(reader, volunteerWork, volunteerWorkId);
            SetVolunteerWorkMainImageIdProperties(reader, volunteerWork, volunteerWorkId);

            volunteerWork.ResponsibilitiesAndAchievements = _achievementDao.GetAchievementsByVolunteerWorkId(volunteerWorkId);
            volunteerWork.SkillsUsedAndObtained = _skillDao.GetSkillsByVolunteerWorkId(volunteerWorkId);
            volunteerWork.AdditionalImages = _imageDao.GetAdditionalImagesByVolunteerWorkId(volunteerWorkId);
            
            return volunteerWork;
        }

        private void SetVolunteerWorkOrganizationLogoIdProperties(NpgsqlDataReader reader, VolunteerWork volunteerWork, int volunteerWorkId)
        {
            if (reader["organization_logo_id"] != DBNull.Value)
            {
                volunteerWork.OrganizationLogoId = Convert.ToInt32(reader["organization_logo_id"]);

                volunteerWork.OrganizationLogo = _imageDao.GetMainImageOrOrganizationLogoByVolunteerWorkId(volunteerWorkId, Logo);
            }
            else
            {
                volunteerWork.OrganizationLogoId = 0;
            }
        }

        private void SetVolunteerWorkOrganizationWebsiteIdProperties(NpgsqlDataReader reader, VolunteerWork volunteerWork, int volunteerWorkId)
        {
            if (reader["organization_website_id"] != DBNull.Value)
            {
                volunteerWork.OrganizationWebsiteId = Convert.ToInt32(reader["organization_website_id"]);

                volunteerWork.OrganizationWebsite = _websiteDao.GetWebsiteByVolunteerWorkId(volunteerWorkId);
            }
            else
            {
                volunteerWork.OrganizationWebsiteId = 0;
            }
        }

        private void SetVolunteerWorkMainImageIdProperties(NpgsqlDataReader reader, VolunteerWork volunteerWork, int volunteerWorkId)
        {
            if (reader["main_image_id"] != DBNull.Value)
            {
                volunteerWork.MainImageId = Convert.ToInt32(reader["main_image_id"]);

                volunteerWork.MainImage = _imageDao.GetMainImageOrOrganizationLogoByVolunteerWorkId(volunteerWorkId, MainImage);
            }
            else
            {
                volunteerWork.MainImageId = 0;
            }
        }
    }
}