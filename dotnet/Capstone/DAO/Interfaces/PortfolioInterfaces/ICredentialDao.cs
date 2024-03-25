using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface ICredentialDao
    {
        /*  
            **********************************************************************************************
                                                CREDENTIAL CRUD
            **********************************************************************************************
        */
// TODO CREDENTIAL Controllers****
        Credential CreateCredential(Credential credential);
        List<Credential> GetCredentials();
        Credential GetCredential(int credentialId);
        Credential UpdateCredential(int credentialId, Credential credential);
        int DeleteCredential(int credentialId);
        
        /*  
            **********************************************************************************************
                                            PORTFOLIO CREDENTIAL CRUD
            **********************************************************************************************
        */
// TODO create PORTFOLIO Credential Controllers
        Credential CreateCredentialByPortfolioId(int portfolioId, Credential credential);
        List<Credential> GetCredentialsByPortfolioId(int portfolioId);
        Credential GetCredentialByPortfolioId(int portfolioId, int credentialId);
        Credential UpdateCredentialByPortfolioId(int portfolioId, int credentialId, Credential credential);
        int DeleteCredentialByPortfolioId(int portfolioId, int credentialId);
    }
}