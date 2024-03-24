using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface IAchievementDao
    {
        /*  
            **********************************************************************************************
                                                ACHIEVEMENT CRUD
            **********************************************************************************************
        */
// TODO ACHIEVEMENT Controllers****
        Achievement GetAchievement(int achievementId);
        List<Achievement> GetAchievements();
    
        /*  
            **********************************************************************************************
                                          WORK EXPERIENCE ACHIEVEMENT CRUD
            **********************************************************************************************
        */
// TODO ACHIEVEMENT Experience Controllers****
        Achievement CreateAchievementByWorkExperienceId(int experienceId, Achievement achievement);
        List<Achievement> GetAchievementsByWorkExperienceId(int experienceId);
        Achievement GetAchievementByWorkExperienceId(int experienceId, int achievementId);
        Achievement UpdateAchievementByWorkExperienceId(int experienceId, int achievementId, Achievement achievement);
        int DeleteAchievementByWorkExperienceId(int experienceId, int achievementId);

        /*  
            **********************************************************************************************
                                    OPEN SOURCE CONTRIBUTION ACHIEVEMENT CRUD
            **********************************************************************************************
        */
// TODO ACHIEVEMENT Contribution Controllers****
        Achievement CreateAchievementByOpenSourceContributionId(int contributionId, Achievement achievement);
        List<Achievement> GetAchievementsByOpenSourceContributionId(int contributionId);
        Achievement GetAchievementByOpenSourceContributionId(int contributionId, int achievementId);
        Achievement UpdateAchievementByOpenSourceContributionId(int contributionId, int achievementId, Achievement achievement);
        int DeleteAchievementByOpenSourceContributionId(int contributionId, int achievementId);
        /*  
            **********************************************************************************************
                                            VOLUNTEER WORK ACHIEVEMENT CRUD
            **********************************************************************************************
        */
// TODO ACHIEVEMENT Volunteer Controllers****
        Achievement CreateAchievementByVolunteerWorkId(int volunteerId, Achievement achievement);
        List<Achievement> GetAchievementsByVolunteerWorkId(int volunteerId);
        Achievement GetAchievementByVolunteerWorkId(int volunteerId, int achievementId);
        Achievement UpdateAchievementByVolunteerWorkId(int volunteerId, int achievementId, Achievement achievement);
        int DeleteAchievementByVolunteerWorkId(int volunteerId, int achievementId);
    }
}