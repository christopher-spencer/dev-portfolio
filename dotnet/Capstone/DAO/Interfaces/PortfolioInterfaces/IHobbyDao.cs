using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IHobbyDao
    {
        /*  
            **********************************************************************************************
                                                HOBBY CRUD
            **********************************************************************************************
        */
// TODO HOBBY Controllers****
        Hobby GetHobby(int hobbyId);
        List<Hobby> GetHobbies();
    
        /*  
            **********************************************************************************************
                                          PORTFOLIO HOBBY CRUD
            **********************************************************************************************
        */
// TODO HOBBY Portfolio Controllers****
        Hobby CreateHobbyByPortfolioId(int portfolioId, Hobby hobby);
        List<Hobby> GetHobbiesByPortfolioId(int portfolioId);    
        Hobby GetHobbyByPortfolioId(int portfolioId, int hobbyId);
        Hobby UpdateHobbyByPortfolioId(int portfolioId, int hobbyId, Hobby hobby);
        int DeleteHobbyByPortfolioId(int portfolioId, int hobbyId);

    }
}