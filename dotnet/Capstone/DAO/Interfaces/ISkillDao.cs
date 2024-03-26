using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface ISkillDao
    {

        /*  
            **********************************************************************************************
                                                    SKILL CRUD
            **********************************************************************************************
        */
        // Skill CreateSkill(Skill skill);
        Skill GetSkill(int skillId);
        List<Skill> GetSkills();
        // Skill UpdateSkill(int skillId, Skill skill);
        // int DeleteSkill(int skillId);

        /*  
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
                                            PORTFOLIO SKILL CRUD
            **********************************************************************************************
            **********************************************************************************************
            **********************************************************************************************
        */

        Skill CreateSkillByPortfolioId(int portfolioId, Skill skill);
        List<Skill> GetSkillsByPortfolioId(int portfolioId);
        Skill GetSkillByPortfolioId(int portfolioId, int skillId);
        Skill UpdateSkillByPortfolioId(int portfolioId, int skillId, Skill skill);
        int DeleteSkillByPortfolioId(int portfolioId, int skillId);
        /*  
            **********************************************************************************************
                                            SIDE PROJECT SKILL CRUD
            **********************************************************************************************
        */      
        Skill CreateSkillBySideProjectId(int projectId, Skill skill);
        List<Skill> GetSkillsBySideProjectId(int projectId);
        Skill GetSkillBySideProjectId(int projectId, int skillId);
        Skill UpdateSkillBySideProjectId(int projectId, int skillId, Skill skill);
        int DeleteSkillBySideProjectId(int projectId, int skillId);


    /*   
        **********************************************************************************************
                                        WORK EXPERIENCE SKILL CRUD
        **********************************************************************************************
    */

        Skill CreateSkillByWorkExperienceId(int experienceId, Skill skill); 
        List<Skill> GetSkillsByWorkExperienceId(int experienceId);
        Skill GetSkillByWorkExperienceId(int experienceId, int skillId);
        Skill UpdateSkillByWorkExperienceId(int experienceId, int skillId, Skill skill);
        int DeleteSkillByWorkExperienceId(int experienceId, int skillId);
    
        /*  
            **********************************************************************************************
                                            CREDENTIAL SKILL CRUD
            **********************************************************************************************
        */

        Skill CreateSkillByCredentialId(int credentialId, Skill skill);
        List<Skill> GetSkillsByCredentialId(int credentialId);
        Skill GetSkillByCredentialId(int credentialId, int skillId);
        Skill UpdateSkillByCredentialId(int credentialId, int skillId, Skill skill);
        int DeleteSkillByCredentialId(int credentialId, int skillId);

        /*  
            **********************************************************************************************
                                     OPEN SOURCE CONTRIBUTION SKILL CRUD
            **********************************************************************************************
        */
// TODO SKILL OpenSourceContribution Controllers****
        Skill CreateSkillByOpenSourceContributionId(int openSourceContributionId, Skill skill);
        List<Skill> GetSkillsByOpenSourceContributionId(int openSourceContributionId);
        Skill GetSkillByOpenSourceContributionId(int openSourceContributionId, int skillId);
        Skill UpdateSkillByOpenSourceContributionId(int openSourceContributionId, int skillId, Skill skill);
        int DeleteSkillByOpenSourceContributionId(int openSourceContributionId, int skillId);

        /*  
            **********************************************************************************************
                                        VOLUNTEER WORK SKILL CRUD
            **********************************************************************************************
        */
// TODO SKILL VolunteerWork Controllers****
        Skill CreateSkillByVolunteerWorkId(int volunteerWorkId, Skill skill);
        List<Skill> GetSkillsByVolunteerWorkId(int volunteerWorkId);
        Skill GetSkillByVolunteerWorkId(int volunteerWorkId, int skillId);
        Skill UpdateSkillByVolunteerWorkId(int volunteerWorkId, int skillId, Skill skill);
        int DeleteSkillByVolunteerWorkId(int volunteerWorkId, int skillId);
    
    }
}