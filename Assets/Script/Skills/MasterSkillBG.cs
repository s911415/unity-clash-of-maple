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
    public class MasterSkillBG : Skill
    {
        public override void SkillTime()
        {
            Vector3 pos = this.transform.localPosition;
            pos.x += _render.flipX ? 10 : -10;
            _scene.SkillGenerator.UseSkill(pos, 1, _playerID);
        }
    }
}
