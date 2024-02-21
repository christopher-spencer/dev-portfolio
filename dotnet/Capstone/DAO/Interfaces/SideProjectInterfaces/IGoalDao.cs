using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IGoalDao
    {
        Goal CreateGoal(Goal goal);
        Goal GetGoalById(int goalId);
        List<Goal> GetAllGoals();
        Goal UpdateGoal(Goal goal);
        int DeleteGoalById(int goalId);

    }
}