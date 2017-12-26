using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using UnityEngine;
using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Component.Message;
using NTUT.CSIE.GameDev.Skills;

namespace NTUT.CSIE.GameDev.Component
{
    public class SkillGenerator : CommonObject
    {
        [SerializeField]
        protected GameObject[] _skills;

        public void UseSkill(Vector3 position, int skillID, int playerID)
        {
            var obj = Instantiate(_skills[skillID], this.transform);
            var skill = obj.GetComponent<Skill>();
            position.y = 0;
            obj.transform.localPosition = position;
            skill.Init(skillID, playerID);
        }
    }
}
