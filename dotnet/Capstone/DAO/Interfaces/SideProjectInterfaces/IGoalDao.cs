using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IGoalDao
    {
        /*  
            **********************************************************************************************
                                                    GOAL CRUD
            **********************************************************************************************
        */    

        Goal GetGoal(int goalId);
        List<Goal> GetGoals();
        
        /*  
            **********************************************************************************************
                                            SIDE PROJECT GOAL CRUD
            **********************************************************************************************
        */       
        Goal CreateGoalBySideProjectId(int projectId, Goal goal);
        List<Goal> GetGoalsBySideProjectId(int projectId);
        Goal GetGoalBySideProjectId(int projectId, int goalId);
        Goal UpdateGoalBySideProjectId(int projectId, int goalId, Goal goal);
        int DeleteGoalBySideProjectId(int projectId, int goalId);

    }
}