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
        Goal CreateGoal(Goal goal);
        Goal GetGoalById(int goalId);
        List<Goal> GetAllGoals();
        Goal UpdateGoal(int goalId, Goal goal);
        int DeleteGoalById(int goalId);
        
        /*  
            **********************************************************************************************
                                            SIDE PROJECT GOAL CRUD
            **********************************************************************************************
        */       
        Goal CreateGoalBySideProjectId(int projectId, Goal goal);
        List<Goal> GetGoalsBySideProjectId(int projectId);
        Goal GetGoalBySideProjectId(int projectId, int goalId);
        Goal UpdateGoalBySideProjectId(int projectId, Goal goal);
        int DeleteGoalBySideProjectId(int projectId, int goalId);

    }
}