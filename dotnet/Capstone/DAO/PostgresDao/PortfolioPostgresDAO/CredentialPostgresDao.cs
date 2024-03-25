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