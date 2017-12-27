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
        private static int _instanceCount = 0;
        private const float MAX_SOUND_COUNT = 10;
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

        protected int GetAttackByDifficult()
        {
            switch (this.Manager.Difficulty)
            {
                case Difficulty.Level.Easy:
                    return 1000;

                case Difficulty.Level.Normal:
                    return 2200;

                case Difficulty.Level.Hard:
                    return 3300;
            }

            return 3000;
        }

        public override void SkillTime()
        {
            var mList = GetNearMobs(ATTACK_RANGE);

            foreach (var m in mList)
            {
                if (_instanceCount < MAX_SOUND_COUNT && _hitClip)
                    _audio.PlayOneShot(_hitClip);

                var dmg = Helper.GetRandomValueBaseOnValue(GetAttackByDifficult(), 0.25f);
                m.Damage(dmg);
            }
        }
    }
}
