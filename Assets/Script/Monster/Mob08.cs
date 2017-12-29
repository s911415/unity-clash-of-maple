using NTUT.CSIE.GameDev.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Mob08 : Monster
    {
        protected const float 中毒機率 = 0.5f;

        public Mob08() : base(8)
        {
        }

        protected override void DamageTargets(IEnumerable<HurtableObject> targetList)
        {
            base.DamageTargets(targetList);

            foreach (var m in targetList)
            {
                if (Random.value < 中毒機率)
                {
                    Monster mConv = m as Monster;

                    if (mConv != null)
                    {
                        mConv.SetPoisoning(35, 10000, 1000);
                    }
                }
            }
        }
    }
}
