using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.DAO.Interfaces
{
    public interface ISkillDao
    {
        Skill CreateSkill(Skill skill);
        Skill GetSkillById(int skillId);
        List<Skill> GetAllSkills();
        Skill UpdateSkill(Skill skill);
        int DeleteSkillById(int skillId);
    }
}