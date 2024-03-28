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

        // public VolunteerWork CreateVolunteerWork(VolunteerWork volunteerWork)
        // {
        //     if (string.IsNullOrEmpty(volunteerWork.OrganizationName))
        //     {
        //         throw new ArgumentException("Organization Name is required to create a Volunteer Work.");
        //     }

        //     if (string.IsNullOrEmpty(volunteerWork.PositionTitle))
        //     {
        //         throw new ArgumentException("Position Title is required to create a Volunteer Work.");
        //     }

        //     if (volunteerWork.StartDate == DateTime.MinValue || volunteerWork.StartDate > DateTime.Now)
        //     {
        //         throw new ArgumentException("Start Date must be a valid date in the past or present to create a Volunteer Work.");
        //     }

        //     string sql = "INSERT INTO volunteer_works (organization_name, location, organization_description, " +
        //                  "position_title, start_date, end_date) " +
        //                  "VALUES (@organizationName, @location, @organizationDescription, @positionTitle, @startDate, " +
        //                  "@endDate) " +
        //                  "RETURNING id;";

        //     try
        //     {
        //         using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        //         {
        //             connection.Open();

        //             using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
        //             {
        //                 cmd.Parameters.AddWithValue("@organizationName", volunteerWork.OrganizationName);
        //                 cmd.Parameters.AddWithValue("@location", volunteerWork.Location);
        //                 cmd.Parameters.AddWithValue("@organizationDescription", volunteerWork.OrganizationDescription);
        //                 cmd.Parameters.AddWithValue("@positionTitle", volunteerWork.PositionTitle);
        //                 cmd.Parameters.AddWithValue("@startDate", volunteerWork.StartDate);
        //                 cmd.Parameters.AddWithValue("@endDate", volunteerWork.EndDate);

        //                 int volunteerWorkId = Convert.ToInt32(cmd.ExecuteScalar());
        //                 volunteerWork.Id = volunteerWorkId;
        //             }
        //         }
        //     }
        //     catch (NpgsqlException ex)
        //     {
        //         throw new DaoException("An error occurred while creating the Volunteer Work.", ex);
        //     }

        //     return volunteerWork;
        // }

        public List<VolunteerWork> GetVolunteerWorks()
        {
            List<VolunteerWork> volunteerWorks = new List<VolunteerWork>();

            string sql = "SELECT organization_name, location, organization_description, " +
                         "position_title, start_date, end_date FROM volunteer_works;";

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
                                VolunteerWork volunteerWork = MapRowToVolunteerWork(reader);
                                volunteerWorks.Add(volunteerWork);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving all Volunteer Works.", ex);
            }

            return volunteerWorks;
        }

        public VolunteerWork GetVolunteerWork(int volunteerWorkId)
        {
            if (volunteerWorkId <= 0)
            {
                throw new ArgumentException("Volunteer Work ID must be greater than zero.");
            }

            VolunteerWork volunteerWork = null;

            string sql = "SELECT organization_name, location, organization_description, " +
                         "position_title, start_date, end_date " +
                         "FROM volunteer_works WHERE id = @volunteerWorkId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                volunteerWork = MapRowToVolunteerWork(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Volunteer Work.", ex);
            }

            return volunteerWork;
        }

        // public VolunteerWork UpdateVolunteerWork(int volunteerWorkId, VolunteerWork volunteerWork)
        // {
        //     if (volunteerWorkId <= 0)
        //     {
        //         throw new ArgumentException("Volunteer Work ID must be greater than zero.");
        //     }

        //     if (string.IsNullOrEmpty(volunteerWork.OrganizationName))
        //     {
        //         throw new ArgumentException("Organization Name is required to update a Volunteer Work.");
        //     }

        //     if (string.IsNullOrEmpty(volunteerWork.PositionTitle))
        //     {
        //         throw new ArgumentException("Position Title is required to update a Volunteer Work.");
        //     }

        //     if (volunteerWork.StartDate == DateTime.MinValue || volunteerWork.StartDate > DateTime.Now)
        //     {
        //         throw new ArgumentException("Start Date must be a valid date in the past or present to update a Volunteer Work.");
        //     }

        //     string sql = "UPDATE volunteer_works SET organization_name = @organizationName, location = @location, " +
        //                  "organization_description = @organizationDescription, position_title = @positionTitle, " +
        //                  "start_date = @startDate, end_date = @endDate " +
        //                  "WHERE id = @volunteerWorkId;";

        //     try
        //     {
        //         using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        //         {
        //             connection.Open();

        //             using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
        //             {
        //                 cmd.Parameters.AddWithValue("@organizationName", volunteerWork.OrganizationName);
        //                 cmd.Parameters.AddWithValue("@location", volunteerWork.Location);
        //                 cmd.Parameters.AddWithValue("@organizationDescription", volunteerWork.OrganizationDescription);
        //                 cmd.Parameters.AddWithValue("@positionTitle", volunteerWork.PositionTitle);
        //                 cmd.Parameters.AddWithValue("@startDate", volunteerWork.StartDate);
        //                 cmd.Parameters.AddWithValue("@endDate", volunteerWork.EndDate);
        //                 cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);

        //                 int count = cmd.ExecuteNonQuery();

        //                 if (count == 1)
        //                 {
        //                     return volunteerWork;
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
        //         throw new DaoException("An error occurred while updating the Volunteer Work.", ex);
        //     }
        // }

        // public int DeleteVolunteerWork(int volunteerWorkId)
        // {
        //     if (volunteerWorkId <= 0)
        //     {
        //         throw new ArgumentException("Volunteer Work ID must be greater than zero.");
        //     }

        //     string sql = "DELETE FROM volunteer_works WHERE id = @volunteerWorkId;";

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

        //                     int? mainImageId = GetMainImageIdByVolunteerWorkId(volunteerWorkId);
        //                     int? organizationLogoId = GetOrganizationLogoIdByVolunteerWorkId(volunteerWorkId);
        //                     int? organizationWebsiteId = GetOrganizationWebsiteIdByVolunteerWorkId(volunteerWorkId);

        //                     if (mainImageId.HasValue)
        //                     {
        //                         _imageDao.DeleteImageByVolunteerWorkId(volunteerWorkId, mainImageId.Value);
        //                     }

        //                     if (organizationLogoId.HasValue)
        //                     {
        //                         _imageDao.DeleteImageByVolunteerWorkId(volunteerWorkId, organizationLogoId.Value);
        //                     }

        //                     if (organizationWebsiteId.HasValue)
        //                     {
        //                         _websiteDao.DeleteWebsiteByVolunteerWorkId(volunteerWorkId, organizationWebsiteId.Value);
        //                     }

        //                     DeleteResponsibilitiesAndAchievementsByVolunteerWorkId(volunteerWorkId);
        //                     DeleteSkillsUsedAndObtainedByVolunteerWorkId(volunteerWorkId);
        //                     DeleteAdditionalImagesByVolunteerWorkId(volunteerWorkId);

        //                     using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
        //                     {
        //                         cmd.Transaction = transaction;
        //                         cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
        //                         rowsAffected = cmd.ExecuteNonQuery();
        //                     }

        //                     transaction.Commit();

        //                     return rowsAffected;
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     Console.WriteLine(ex.ToString());

        //                     transaction.Rollback();

        //                     throw new DaoException("An error occurred while deleting the Volunteer Work.", ex);
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
                                        PORTFOLIO VOLUNTEER WORK CRUD
            **********************************************************************************************
        */

        public VolunteerWork CreateVolunteerWorkByPortfolioId(int portfolioId, VolunteerWork volunteerWork)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(volunteerWork.OrganizationName))
            {
                throw new ArgumentException("Organization Name is required to create a Volunteer Work.");
            }

            if (string.IsNullOrEmpty(volunteerWork.PositionTitle))
            {
                throw new ArgumentException("Position Title is required to create a Volunteer Work.");
            }

            if (volunteerWork.StartDate == DateTime.MinValue || volunteerWork.StartDate > DateTime.Now)
            {
                throw new ArgumentException("Start Date must be a valid date in the past or present to create a Volunteer Work.");
            }

            string insertVolunteerWorkSql = "INSERT INTO volunteer_works (organization_name, location, " +
                                             "organization_description, position_title, start_date, end_date) " +
                                             "VALUES (@organizationName, @location, @organizationDescription, " +
                                             "@positionTitle, @startDate, @endDate) " +
                                             "RETURNING id;";

            string insertPortfolioVolunteerWorkSql = "INSERT INTO portfolio_volunteer_works (portfolio_id, " +
                                                     "volunteer_id) " +
                                                     "VALUES (@portfolioId, @volunteerWorkId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int volunteerWorkId;

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertVolunteerWorkSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@organizationName", volunteerWork.OrganizationName);
                                cmd.Parameters.AddWithValue("@location", volunteerWork.Location);
                                cmd.Parameters.AddWithValue("@organizationDescription", volunteerWork.OrganizationDescription);
                                cmd.Parameters.AddWithValue("@positionTitle", volunteerWork.PositionTitle);
                                cmd.Parameters.AddWithValue("@startDate", volunteerWork.StartDate);
                                cmd.Parameters.AddWithValue("@endDate", volunteerWork.EndDate);
                                cmd.Transaction = transaction;

                                volunteerWorkId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertPortfolioVolunteerWorkSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
                                cmd.Transaction = transaction;
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            volunteerWork.Id = volunteerWorkId;

                            return volunteerWork;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the Volunteer Work for the Portfolio.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<VolunteerWork> GetVolunteerWorksByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            List<VolunteerWork> volunteerWorks = new List<VolunteerWork>();

            string sql = "SELECT vw.id, vw.organization_name, vw.location, vw.organization_description, " +
                         "vw.position_title, vw.start_date, vw.end_date " +
                         "FROM volunteer_works vw " +
                         "JOIN portfolio_volunteer_works pvw ON vw.id = pvw.volunteer_id " +
                         "WHERE pvw.portfolio_id = @portfolioId;";

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
                                VolunteerWork volunteerWork = MapRowToVolunteerWork(reader);
                                volunteerWorks.Add(volunteerWork);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving all Volunteer Works for the Portfolio.", ex);
            }

            return volunteerWorks;
        }

        public VolunteerWork GetVolunteerWorkByPortfolioId(int portfolioId, int volunteerWorkId)
        {
            if (portfolioId <= 0 || volunteerWorkId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Volunteer Work ID must be greater than zero.");
            }

            VolunteerWork volunteerWork = null;

            string sql = "SELECT vw.id, vw.organization_name, vw.location, vw.organization_description, " +
                         "vw.position_title, vw.start_date, vw.end_date " +
                         "FROM volunteer_works vw " +
                         "JOIN portfolio_volunteer_works pvw ON vw.id = pvw.volunteer_id " +
                         "WHERE pvw.portfolio_id = @portfolioId AND vw.id = @volunteerWorkId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                volunteerWork = MapRowToVolunteerWork(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Volunteer Work for the Portfolio.", ex);
            }

            return volunteerWork;
        }

        public VolunteerWork UpdateVolunteerWorkByPortfolioId(int portfolioId, int volunteerWorkId, VolunteerWork volunteerWork)
        {
            if (portfolioId <= 0 || volunteerWorkId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Volunteer Work ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(volunteerWork.OrganizationName))
            {
                throw new ArgumentException("Organization Name is required to update a Volunteer Work.");
            }

            if (string.IsNullOrEmpty(volunteerWork.PositionTitle))
            {
                throw new ArgumentException("Position Title is required to update a Volunteer Work.");
            }

            if (volunteerWork.StartDate == DateTime.MinValue || volunteerWork.StartDate > DateTime.Now)
            {
                throw new ArgumentException("Start Date must be a valid date in the past or present to update a Volunteer Work.");
            }

            string sql = "UPDATE volunteer_works SET organization_name = @organizationName, location = @location, " +
                         "organization_description = @organizationDescription, position_title = @positionTitle, " +
                         "start_date = @startDate, end_date = @endDate " +
                         "FROM portfolio_volunteer_works pvw " +
                         "WHERE pvw.portfolio_id = @portfolioId AND pvw.volunteer_id = @volunteerWorkId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
                        cmd.Parameters.AddWithValue("@organizationName", volunteerWork.OrganizationName);
                        cmd.Parameters.AddWithValue("@location", volunteerWork.Location);
                        cmd.Parameters.AddWithValue("@organizationDescription", volunteerWork.OrganizationDescription);
                        cmd.Parameters.AddWithValue("@positionTitle", volunteerWork.PositionTitle);
                        cmd.Parameters.AddWithValue("@startDate", volunteerWork.StartDate);
                        cmd.Parameters.AddWithValue("@endDate", volunteerWork.EndDate);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return volunteerWork;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the Volunteer Work for the Portfolio.", ex);
            }

            return null;
        }

        public int DeleteVolunteerWorkByPortfolioId(int portfolioId, int volunteerWorkId)
        {
            if (portfolioId <= 0 || volunteerWorkId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Volunteer Work ID must be greater than zero.");
            }

            string deletePortfolioVolunteerWorkSql = "DELETE FROM portfolio_volunteer_works " +
                                                     "WHERE portfolio_id = @portfolioId " +
                                                     "AND volunteer_id = @volunteerWorkId;";

            string deleteVolunteerWorkSql = "DELETE FROM volunteer_works " +
                                             "WHERE id = @volunteerWorkId;";

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

                            int? mainImageId = GetMainImageIdByVolunteerWorkId(volunteerWorkId);
                            int? organizationLogoId = GetOrganizationLogoIdByVolunteerWorkId(volunteerWorkId);
                            int? organizationWebsiteId = GetOrganizationWebsiteIdByVolunteerWorkId(volunteerWorkId);

                            if (mainImageId.HasValue)
                            {
                                _imageDao.DeleteImageByVolunteerWorkId(volunteerWorkId, mainImageId.Value);
                            }

                            if (organizationLogoId.HasValue)
                            {
                                _imageDao.DeleteImageByVolunteerWorkId(volunteerWorkId, organizationLogoId.Value);
                            }

                            if (organizationWebsiteId.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByVolunteerWorkId(volunteerWorkId, organizationWebsiteId.Value);
                            }

                            DeleteResponsibilitiesAndAchievementsByVolunteerWorkId(volunteerWorkId);
                            DeleteSkillsUsedAndObtainedByVolunteerWorkId(volunteerWorkId);
                            DeleteAdditionalImagesByVolunteerWorkId(volunteerWorkId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deletePortfolioVolunteerWorkSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);
                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteVolunteerWorkSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@volunteerWorkId", volunteerWorkId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the Volunteer Work for the Portfolio.", ex);
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