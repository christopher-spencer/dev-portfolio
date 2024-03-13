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
        Skill CreateSkill(Skill skill);
        Skill GetSkillById(int skillId);
        List<Skill> GetAllSkills();
        Skill UpdateSkill(int skillId, Skill skill);
        int DeleteSkill(int skillId);

        /*  
            **********************************************************************************************
                                            SIDE PROJECT SKILL CRUD
            **********************************************************************************************
        */      
        Skill CreateSkillBySideProjectId(int projectId, Skill skill);
        List<Skill> GetSkillsBySideProjectId(int projectId);
        Skill GetSkillBySideProjectId(int projectId, int skillId);
        Skill UpdateSkillBySideProjectId(int projectId, Skill skill);
        int DeleteSkillBySideProjectId(int projectId, int skillId);
    }
}