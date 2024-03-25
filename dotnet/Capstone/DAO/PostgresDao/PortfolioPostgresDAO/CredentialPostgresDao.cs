using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class CredentialPostgresDao : ICredentialDao
    {
        private readonly string connectionString;
        private readonly IImageDao _imageDao;
        private readonly IWebsiteDao _websiteDao;
        private readonly ISkillDao _skillDao;


        public CredentialPostgresDao(string dbConnectionString, IImageDao imageDao, IWebsiteDao websiteDao, ISkillDao skillDao)
        {
            connectionString = dbConnectionString;
            this._imageDao = imageDao;
            this._websiteDao = websiteDao;
            this._skillDao = skillDao;
        }

        const string MainImage = "main image";
        const string AdditionalImage = "additional image";
        const string Logo = "logo";

        /*  
            **********************************************************************************************
                                                CREDENTIAL CRUD
            **********************************************************************************************
        */
        
        /*  
            **********************************************************************************************
                                            PORTFOLIO CREDENTIAL CRUD
            **********************************************************************************************
        */

        /*  
            **********************************************************************************************
                                            CREDENTIAL HELPER METHODS
            **********************************************************************************************
        */

        private int? GetOrganizationLogoIdByCredentialId(int credentialId)
        {
            string sql = "SELECT organization_logo_id FROM credentials WHERE id = @credentialId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("credentialId", credentialId);

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
                Console.WriteLine("Error retrieving Organization Logo Id by Credential ID: " + ex.Message);
                return null;
            }
        }

        private int? GetOrganizationWebsiteIdByCredentialId(int credentialId)
        {
            string sql = "SELECT organization_website_id FROM credentials WHERE id = @credentialId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("credentialId", credentialId);

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
                Console.WriteLine("Error retrieving Organization Website Id by Credential ID: " + ex.Message);
                return null;
            }
        }

        private int? GetCredentialWebsiteIdByCredentialId(int credentialId)
        {
            string sql = "SELECT credential_website_id FROM credentials WHERE id = @credentialId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("credentialId", credentialId);

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
                Console.WriteLine("Error retrieving Credential Website Id by Credential ID: " + ex.Message);
                return null;
            }
        }

        private int? GetMainImageIdByCredentialId(int credentialId)
        {
            string sql = "SELECT main_image_id FROM credentials WHERE id = @credentialId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("credentialId", credentialId);

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
                Console.WriteLine("Error retrieving Main Image Id by Credential ID: " + ex.Message);
                return null;
            }
        }

        private int DeleteAssociatedSkillsByCredentialId(int credentialId)
        {
            List<Skill> skills = _skillDao.GetSkillsByCredentialId(credentialId);

            int skillsDeletedCount = 0;

            foreach (Skill skill in skills)
            {
                int skillId = skill.Id;

                try
                {
                    _skillDao.DeleteSkillByCredentialId(credentialId, skillId);
                    skillsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting Skill by Credential ID: " + ex.Message);
                }
            }

            return skillsDeletedCount;
        }
        
        /*  
            **********************************************************************************************
                                                CREDENTIAL MAP ROW
            **********************************************************************************************
        */

        private Credential MapRowToCredential(NpgsqlDataReader reader)
        {
            Credential credential = new Credential
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                IssuingOrganization = Convert.ToString(reader["issuing_organization"]),
                Description = Convert.ToString(reader["description"]),
                IssueDate = Convert.ToDateTime(reader["issue_date"]),
                ExpirationDate = Convert.ToDateTime(reader["expiration_date"]),
                CredentialIdNumber = Convert.ToInt32(reader["credential_id_number"])
            };

            int credentialId = credential.Id;

            SetCredentialMainImageIdProperties(reader, credential, credentialId);
            SetCredentialWebsiteIdProperties(reader, credential, credentialId);
            SetCredentialOrganizationWebsiteIdProperties(reader, credential, credentialId);
            SetCredentialOrganizationLogoIdProperties(reader, credential, credentialId);

            credential.AssociatedSkills = _skillDao.GetSkillsByCredentialId(credentialId);

            return credential;
        }

        private void SetCredentialOrganizationLogoIdProperties(NpgsqlDataReader reader, Credential credential, int credentialId)
        {
            if (reader["organization_logo_id"] != DBNull.Value)
            {
                credential.OrganizationLogoId = Convert.ToInt32(reader["organization_logo_id"]);

                credential.OrganizationLogo = _imageDao.GetMainImageOrOrganizationLogoByCredentialId(credentialId, Logo);
            }
            else
            {
                credential.OrganizationLogoId = 0;
            }
        }

        private void SetCredentialOrganizationWebsiteIdProperties(NpgsqlDataReader reader, Credential credential, int credentialId)
        {
            if (reader["organization_website_id"] != DBNull.Value)
            {
                credential.OrganizationWebsiteId = Convert.ToInt32(reader["organization_website_id"]);

                credential.OrganizationWebsite = _websiteDao.GetWebsiteByCredentialId(credentialId, credential.OrganizationWebsiteId);
            }
            else
            {
                credential.OrganizationWebsiteId = 0;
            }
        }

        private void SetCredentialWebsiteIdProperties(NpgsqlDataReader reader, Credential credential, int credentialId)
        {
            if (reader["credential_website_id"] != DBNull.Value)
            {
                credential.CredentialWebsiteId = Convert.ToInt32(reader["credential_website_id"]);

                credential.CredentialWebsite = _websiteDao.GetWebsiteByCredentialId(credentialId, credential.CredentialWebsiteId);
            }
            else
            {
                credential.CredentialWebsiteId = 0;
            }
        }
        
        private void SetCredentialMainImageIdProperties(NpgsqlDataReader reader, Credential credential, int credentialId)
        {
            if (reader["main_image_id"] != DBNull.Value)
            {
                credential.MainImageId = Convert.ToInt32(reader["main_image_id"]);

                credential.MainImage = _imageDao.GetMainImageOrOrganizationLogoByCredentialId(credentialId, MainImage);
            }
            else
            {
                credential.MainImageId = 0;
            }
        }
    }
}