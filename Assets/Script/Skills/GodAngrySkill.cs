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
    public class GodAngrySkill : Skill
    {
        private const float ATTACK_RANGE = 10;
        private static int _instanceCount = 0;
        private const float MAX_SOUND_COUNT = 15;
        public override void SkillFinish()
        {
            _instanceCount--;
            base.SkillFinish();
        }

        public override void SkillStart()
        {
            base.SkillStart();
            _instanceCount++;
        }

        public override void SkillTime()
        {
            var mList = GetNearMobs(ATTACK_RANGE);

            foreach (var m in mList)
            {
                if (_instanceCount < MAX_SOUND_COUNT && _hitClip)
                    _audio.PlayOneShot(_hitClip);

                var dmg = Helper.GetRandomValueBaseOnValue(5000, 0.2f);
                m.Damage(dmg);
            }
        }
    }
}
