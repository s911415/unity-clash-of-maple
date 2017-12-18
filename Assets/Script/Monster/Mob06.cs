using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Mob06 : Monster
    {
        protected const float 侵蝕機率 = 0.25f;
        public Mob06() : base(6)
        {
        }

        protected override void DamageTarget<T>(IEnumerable<T> targetList)
        {
            base.DamageTarget(targetList);

            foreach (var m in targetList)
            {
                if (Random.value < 侵蝕機率)
                {
                    Monster mConv = m as Monster;

                    if (mConv != null)
                    {
                        mConv.ReduceAttackInTime(20, 3);
                    }
                }
            }
        }
    }
}
