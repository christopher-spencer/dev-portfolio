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

        // public Credential CreateCredential(Credential credential)
        // {
        //     if (string.IsNullOrEmpty(credential.Name))
        //     {
        //         throw new ArgumentException("Credential name is required to created a Credential.");
        //     }

        //     if (string.IsNullOrEmpty(credential.IssuingOrganization))
        //     {
        //         throw new ArgumentException("Issuing Organization is required to created a Credential.");

        //     }

        //     string sql = "INSERT INTO credentials (name, issuing_organization, description, issue_date, " +
        //                  "expiration_date, credential_id_number) " +
        //                  "VALUES (@name, @issuingOrganization, @description, @issueDate, @expirationDate, " +
        //                  "@credentialIdNumber) " +
        //                  "RETURNING id;";
        //     try
        //     {
        //         using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        //         {
        //             connection.Open();

        //             using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
        //             {
        //                 cmd.Parameters.AddWithValue("@name", credential.Name);
        //                 cmd.Parameters.AddWithValue("@issuingOrganization", credential.IssuingOrganization);
        //                 cmd.Parameters.AddWithValue("@description", credential.Description);
        //                 cmd.Parameters.AddWithValue("@issueDate", credential.IssueDate);
        //                 cmd.Parameters.AddWithValue("@expirationDate", credential.ExpirationDate);
        //                 cmd.Parameters.AddWithValue("@credentialIdNumber", credential.CredentialIdNumber);

        //                 int credentialId = Convert.ToInt32(cmd.ExecuteScalar());
        //                 credential.Id = credentialId;
        //             }
        //         }
        //     }
        //     catch (NpgsqlException ex)
        //     {
        //         throw new DaoException("An error occurred while creating the Credential.", ex);
        //     }

        //     return credential;
        // }

        public List<Credential> GetCredentials()
        {
            List<Credential> credentials = new List<Credential>();

            string sql = "SELECT id, name, issuing_organization, description, issue_date, " +
                         "expiration_date, credential_id_number, organization_logo_id, " +
                         "organization_website_id, credential_website_id, main_image_id " +
                          "FROM credentials;";

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
                                Credential credential = MapRowToCredential(reader);
                                credentials.Add(credential);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving all Credentials.", ex);
            }

            return credentials;
        }

        public Credential GetCredential(int credentialId)
        {
            if (credentialId <= 0)
            {
                throw new ArgumentException("Credential ID must be greater than zero.");
            }

            Credential credential = null;

            string sql = "SELECT id, name, issuing_organization, description, issue_date, " +
                         "expiration_date, credential_id_number, organization_logo_id, " +
                         "organization_website_id, credential_website_id, main_image_id " +
                         "FROM credentials WHERE id = @credentialId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                credential = MapRowToCredential(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Credential.", ex);
            }

            return credential;
        }

        // public Credential UpdateCredential(int credentialId, Credential credential)
        // {
        //     if (credentialId <= 0)
        //     {
        //         throw new ArgumentException("Credential ID must be greater than zero.");
        //     }

        //     if (string.IsNullOrEmpty(credential.Name))
        //     {
        //         throw new ArgumentException("Credential name is required to update a Credential.");
        //     }

        //     if (string.IsNullOrEmpty(credential.IssuingOrganization))
        //     {
        //         throw new ArgumentException("Issuing Organization is required to update a Credential.");
        //     }

        //     string sql = "UPDATE credentials SET name = @name, issuing_organization = @issuingOrganization, " +
        //                  "description = @description, issue_date = @issueDate, expiration_date = @expirationDate, " +
        //                  "credential_id_number = @credentialIdNumber WHERE id = @credentialId;";

        //     try
        //     {
        //         using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        //         {
        //             connection.Open();

        //             using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
        //             {
        //                 cmd.Parameters.AddWithValue("@name", credential.Name);
        //                 cmd.Parameters.AddWithValue("@issuingOrganization", credential.IssuingOrganization);
        //                 cmd.Parameters.AddWithValue("@description", credential.Description);
        //                 cmd.Parameters.AddWithValue("@issueDate", credential.IssueDate);
        //                 cmd.Parameters.AddWithValue("@expirationDate", credential.ExpirationDate);
        //                 cmd.Parameters.AddWithValue("@credentialIdNumber", credential.CredentialIdNumber);
        //                 cmd.Parameters.AddWithValue("@credentialId", credentialId);

        //                 int count = cmd.ExecuteNonQuery();

        //                 if (count == 1)
        //                 {
        //                     return credential;
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
        //         throw new DaoException("An error occurred while updating the Credential.", ex);
        //     }
        // }

        // public int DeleteCredential(int credentialId)
        // {
        //     if (credentialId <= 0)
        //     {
        //         throw new ArgumentException("Credential ID must be greater than zero.");
        //     }

        //     string sql = "DELETE FROM credentials WHERE id = @credentialId;";

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

        //                     int? organizationLogoId = GetOrganizationLogoIdByCredentialId(credentialId);
        //                     int? organizationWebsiteId = GetOrganizationWebsiteIdByCredentialId(credentialId);
        //                     int? credentialWebsiteId = GetCredentialWebsiteIdByCredentialId(credentialId);
        //                     int? mainImageId = GetMainImageIdByCredentialId(credentialId);

        //                     if (organizationLogoId.HasValue)
        //                     {
        //                         _imageDao.DeleteImageByCredentialId(credentialId, organizationLogoId.Value);
        //                     }

        //                     if (organizationWebsiteId.HasValue)
        //                     {
        //                         _websiteDao.DeleteWebsiteByCredentialId(credentialId, organizationWebsiteId.Value);
        //                     }

        //                     if (credentialWebsiteId.HasValue)
        //                     {
        //                         _websiteDao.DeleteWebsiteByCredentialId(credentialId, credentialWebsiteId.Value);
        //                     }

        //                     if (mainImageId.HasValue)
        //                     {
        //                         _imageDao.DeleteImageByCredentialId(credentialId, mainImageId.Value);
        //                     }

        //                     DeleteAssociatedSkillsByCredentialId(credentialId);

        //                     using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
        //                     {
        //                         cmd.Transaction = transaction;
        //                         cmd.Parameters.AddWithValue("@credentialId", credentialId);
        //                         rowsAffected = cmd.ExecuteNonQuery();
        //                     }

        //                     transaction.Commit();

        //                     return rowsAffected;

        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     Console.WriteLine(ex.ToString());

        //                     transaction.Rollback();

        //                     throw new DaoException("An error occurred while deleting the Credential.", ex);
        //                 }
        //             }
        //         }
        //     }
        //     catch (NpgsqlException ex)
        //     {
        //         throw new DaoException("An error occurred while deleting the Credential.", ex);
        //     }
        // }

        /*  
            **********************************************************************************************
                                            PORTFOLIO CREDENTIAL CRUD
            **********************************************************************************************
        */

        public Credential CreateCredentialByPortfolioId(int portfolioId, Credential credential)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(credential.Name))
            {
                throw new ArgumentException("Credential name is required to created a Credential.");
            }

            if (string.IsNullOrEmpty(credential.IssuingOrganization))
            {
                throw new ArgumentException("Issuing Organization is required to created a Credential.");
            }

            string insertCredentialSql = "INSERT INTO credentials (name, issuing_organization, description, issue_date, " +
                                         "expiration_date, credential_id_number) " +
                                         "VALUES (@name, @issuingOrganization, @description, @issueDate, @expirationDate, " +
                                         "@credentialIdNumber) " +
                                         "RETURNING id;";

            string insertPortfolioCredentialSql = "INSERT INTO portfolio_credentials (portfolio_id, credential_id) " +
                                                  "VALUES (@portfolioId, @credentialId);";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int credentialId;

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertCredentialSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@name", credential.Name);
                                cmd.Parameters.AddWithValue("@issuingOrganization", credential.IssuingOrganization);
                                cmd.Parameters.AddWithValue("@description", credential.Description);
                                cmd.Parameters.AddWithValue("@issueDate", credential.IssueDate);

//FIXME switched up Parameters.AddWithValue here for null, do elsewhere***********
                               // cmd.Parameters.AddWithValue("@expirationDate", credential.ExpirationDate);
                               // NOTE two ways below ???
                                //cmd.Parameters.AddWithValue("@expirationDate", (object)credential.ExpirationDate ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@expirationDate", credential.ExpirationDate.HasValue ? (object)credential.ExpirationDate : DBNull.Value);

                                //cmd.Parameters.AddWithValue("@credentialIdNumber", credential.CredentialIdNumber);
                                cmd.Parameters.AddWithValue("@credentialIdNumber", (object)credential.CredentialIdNumber ?? DBNull.Value);

                                cmd.Transaction = transaction;

                                credentialId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertPortfolioCredentialSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@credentialId", credentialId);
                                cmd.Transaction = transaction;
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            credential.Id = credentialId;

                            return credential;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while creating the Credential for the Portfolio.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while connecting to the database.", ex);
            }
        }

        public List<Credential> GetCredentialsByPortfolioId(int portfolioId)
        {
            if (portfolioId <= 0)
            {
                throw new ArgumentException("Portfolio ID must be greater than zero.");
            }

            List<Credential> credentials = new List<Credential>();

            string sql = "SELECT c.id, c.name, c.issuing_organization, c.description, c.issue_date, " +
                         "c.expiration_date, c.credential_id_number, c.organization_logo_id, " +
                         "c.organization_website_id, c.credential_website_id, c.main_image_id " +
                         "FROM credentials c " +
                         "JOIN portfolio_credentials pc ON c.id = pc.credential_id " +
                         "WHERE pc.portfolio_id = @portfolioId;";

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
                                Credential credential = MapRowToCredential(reader);
                                credentials.Add(credential);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving all Credentials for the Portfolio.", ex);
            }

            return credentials;
        }

        public Credential GetCredentialByPortfolioId(int portfolioId, int credentialId)
        {
            if (portfolioId <= 0 || credentialId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Credential ID must be greater than zero.");
            }

            Credential credential = null;

            string sql = "SELECT c.id, c.name, c.issuing_organization, c.description, c.issue_date, " +
                         "c.expiration_date, c.credential_id_number, c.organization_logo_id, " +
                         "c.organization_website_id, c.credential_website_id, c.main_image_id " +
                         "FROM credentials c " +
                         "JOIN portfolio_credentials pc ON c.id = pc.credential_id " +
                         "WHERE pc.portfolio_id = @portfolioId AND c.id = @credentialId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                credential = MapRowToCredential(reader);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the Credential for the Portfolio.", ex);
            }

            return credential;
        }

        public Credential UpdateCredentialByPortfolioId(int portfolioId, int credentialId, Credential credential)
        {
            if (portfolioId <= 0 || credentialId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Credential ID must be greater than zero.");
            }

            if (string.IsNullOrEmpty(credential.Name))
            {
                throw new ArgumentException("Credential name is required to update a Credential.");
            }

            if (string.IsNullOrEmpty(credential.IssuingOrganization))
            {
                throw new ArgumentException("Issuing Organization is required to update a Credential.");
            }

            string sql = "UPDATE credentials SET name = @name, issuing_organization = @issuingOrganization, " +
                         "description = @description, issue_date = @issueDate, expiration_date = @expirationDate, " +
                         "credential_id_number = @credentialIdNumber " +
                         "FROM portfolio_credentials pc " +
                         "WHERE pc.portfolio_id = @portfolioId AND pc.credential_id = @credentialId " +
                         "AND pc.credential_id = credentials.id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", credential.Name);
                        cmd.Parameters.AddWithValue("@issuingOrganization", credential.IssuingOrganization);
                        cmd.Parameters.AddWithValue("@description", credential.Description);
                        cmd.Parameters.AddWithValue("@issueDate", credential.IssueDate);
                        cmd.Parameters.AddWithValue("@expirationDate", credential.ExpirationDate);
                        cmd.Parameters.AddWithValue("@credentialIdNumber", credential.CredentialIdNumber);
                        cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return credential;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the credential by portfolio ID.", ex);
            }

            return null;
        }

        public int DeleteCredentialByPortfolioId(int portfolioId, int credentialId)
        {
            if (portfolioId <= 0 || credentialId <= 0)
            {
                throw new ArgumentException("Portfolio ID and Credential ID must be greater than zero.");
            }

            string deletePortfolioCredentialSql = "DELETE FROM portfolio_credentials " +
                                                  "WHERE portfolio_id = @portfolioId " +
                                                  "AND credential_id = @credentialId;";

            string deleteCredentialSql = "DELETE FROM credentials " +
                                         "WHERE id = @credentialId;";

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

                            int? organizationLogoId = GetOrganizationLogoIdByCredentialId(credentialId);
                            int? organizationWebsiteId = GetOrganizationWebsiteIdByCredentialId(credentialId);
                            int? credentialWebsiteId = GetCredentialWebsiteIdByCredentialId(credentialId);
                            int? mainImageId = GetMainImageIdByCredentialId(credentialId);

                            if (organizationLogoId.HasValue)
                            {
                                _imageDao.DeleteImageByCredentialId(credentialId, organizationLogoId.Value);
                            }

                            if (organizationWebsiteId.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByCredentialId(credentialId, organizationWebsiteId.Value);
                            }

                            if (credentialWebsiteId.HasValue)
                            {
                                _websiteDao.DeleteWebsiteByCredentialId(credentialId, credentialWebsiteId.Value);
                            }

                            if (mainImageId.HasValue)
                            {
                                _imageDao.DeleteImageByCredentialId(credentialId, mainImageId.Value);
                            }

                            DeleteAssociatedSkillsByCredentialId(credentialId);

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deletePortfolioCredentialSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@portfolioId", portfolioId);
                                cmd.Parameters.AddWithValue("@credentialId", credentialId);
                                cmd.ExecuteNonQuery();
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteCredentialSql, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@credentialId", credentialId);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }


                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                            transaction.Rollback();

                            throw new DaoException("An error occurred while deleting the Credential by Portfolio ID.", ex);
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
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);

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
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);

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
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);

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
                        cmd.Parameters.AddWithValue("@credentialId", credentialId);

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
                // ExpirationDate = Convert.ToDateTime(reader["expiration_date"]),
                // CredentialIdNumber = Convert.ToInt32(reader["credential_id_number"])
            };
// FIXME switched up MAPROW here for null
                // Handle nullable properties like ExpirationDate and CredentialIdNumber
                credential.ExpirationDate = reader["expiration_date"] == DBNull.Value ? null : (DateTime?)reader["expiration_date"];
                credential.CredentialIdNumber = reader["credential_id_number"] == DBNull.Value ? null : (int?)reader["credential_id_number"];

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