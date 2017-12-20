using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Mob00 : Monster
    {
        private const float 暈眩機率 = 0.25f;

        public Mob00() : base(0)
        {
        }

        protected override void DamageTargets(IEnumerable<HurtableObject> targetList)
        {
            base.DamageTargets(targetList);

            foreach (var m in targetList)
            {
                if (Random.value < 暈眩機率)
                {
                    var mConv = m as Monster;

                    if (mConv != null)
                    {
                        mConv.Freeze(3000);
                    }
                }
            }
        }
    }
}
