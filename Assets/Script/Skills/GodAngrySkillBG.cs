using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Skills
{
    public class GodAngrySkillBG : Skill
    {
        private Monster.Monster[] _mobList;

        public override void SkillStart()
        {
            base.SkillStart();
            _mobList = GetNearMobs(float.PositiveInfinity);
        }

        public override void SkillTime()
        {
            foreach (var m in _mobList)
            {
                if (!m) continue;

                m.Freeze(4 * 1000 / 2);
                _scene.SkillGenerator.UseSkill(m.transform.localPosition, 3, _playerID);
            }
        }
    }
}
