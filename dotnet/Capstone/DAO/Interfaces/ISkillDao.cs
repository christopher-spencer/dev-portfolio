using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface ISkillDao
    {
        Skill CreateSkill(Skill skill);
        Skill CreateSkillByProjectId(int projectId, Skill skill);
        List<Skill> GetSkillsByProjectId(int projectId);
        Skill GetSkillByProjectId(int projectId, int skillId);
        Skill GetSkillById(int skillId);
        List<Skill> GetAllSkills();
        Skill UpdateSkillByProjectId(int projectId, Skill skill);
        Skill UpdateSkill(Skill skill);
        int DeleteSkillByProjectId(int projectId, int skillId);
        int DeleteSkill(int skillId);
    }
}