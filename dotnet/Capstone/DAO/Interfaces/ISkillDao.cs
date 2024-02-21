using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface ISkillDao
    {
        Skill CreateSkill(Skill skill);
        List<Skill> GetSkillsByProjectId(int projectId);
        Skill GetSkillById(int skillId);
        List<Skill> GetAllSkills();
        Skill UpdateSkill(Skill skill);
        int DeleteSkillById(int skillId);
    }
}