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
        Achievement GetAchievement(int achievementId);
        List<Achievement> GetAchievements();
    
        /*  
            **********************************************************************************************
                                          WORK EXPERIENCE ACHIEVEMENT CRUD
            **********************************************************************************************
        */
        Achievement CreateAchievementByWorkExperienceId(int experienceId, Achievement achievement);
        List<Achievement> GetAchievementsByWorkExperienceId(int experienceId);
        Achievement GetAchievementByWorkExperienceId(int experienceId, int achievementId);
        Achievement UpdateAchievementByWorkExperienceId(int experienceId, int achievementId, Achievement achievement);
        int DeleteAchievementByWorkExperienceId(int experienceId, int achievementId);

        /*  
            **********************************************************************************************
                                          EDUCATION ACHIEVEMENT CRUD
            **********************************************************************************************
        */
        Achievement CreateAchievementByEducationId(int educationId, Achievement achievement);
        List<Achievement> GetAchievementsByEducationId(int educationId);    
        Achievement GetAchievementByEducationId(int educationId, int achievementId);
        Achievement UpdateAchievementByEducationId(int educationId, int achievementId, Achievement achievement);
        int DeleteAchievementByEducationId(int educationId, int achievementId);

        /*  
            **********************************************************************************************
                                    OPEN SOURCE CONTRIBUTION ACHIEVEMENT CRUD
            **********************************************************************************************
        */
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
        Achievement CreateAchievementByVolunteerWorkId(int volunteerId, Achievement achievement);
        List<Achievement> GetAchievementsByVolunteerWorkId(int volunteerId);
        Achievement GetAchievementByVolunteerWorkId(int volunteerId, int achievementId);
        Achievement UpdateAchievementByVolunteerWorkId(int volunteerId, int achievementId, Achievement achievement);
        int DeleteAchievementByVolunteerWorkId(int volunteerId, int achievementId);
    }
}