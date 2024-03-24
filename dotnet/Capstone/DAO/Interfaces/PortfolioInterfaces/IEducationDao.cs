using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IEducationDao
    {
        /*  
            **********************************************************************************************
                                                EDUCATION CRUD
            **********************************************************************************************
        */
// TODO EDUCATION Controllers****
        Education CreateEducation(Education education);
        List<Education> GetEducations();
        Education GetEducation(int educationId);
        Education UpdateEducation(int educationId, Education education);
        int DeleteEducation(int educationId);
        
    }
}