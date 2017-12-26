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
    public class MasterSkill : Skill
    {
        private const float ATTACK_RANGE = 25;
        public override void SkillFinish()
        {
            base.SkillFinish();
        }

        public override void SkillTime()
        {
            var mList = GetNearMobs(ATTACK_RANGE);

            foreach (var m in mList)
            {
                if (_hitClip)
                    _audio.PlayOneShot(_hitClip);

                var dmg = Helper.GetRandomValueBaseOnValue(3000, 0.25f);
                m.Damage(dmg);
            }
        }
    }
}
