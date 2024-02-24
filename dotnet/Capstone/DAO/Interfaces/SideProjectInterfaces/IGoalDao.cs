using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IGoalDao
    {
        Goal CreateGoalByProjectId(int projectId, Goal goal);
        Goal CreateGoal(Goal goal);
        List<Goal> GetGoalsByProjectId(int projectId);
        Goal GetGoalByProjectId(int projectId, int goalId);
        Goal GetGoalById(int goalId);
        List<Goal> GetAllGoals();
        Goal UpdateGoalByProjectId(int projectId, Goal updatedGoal);
        Goal UpdateGoal(Goal goal);
        int DeleteGoalByProjectId(int projectId, int goalId);
        int DeleteGoalById(int goalId);

    }
}