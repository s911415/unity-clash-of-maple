using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Mob04 : Monster
    {
        [SerializeField]
        protected AudioClip _attack2AudioClip;
        protected int _attackIdx = 0;

        public Mob04() : base(4)
        {
        }

        protected override void Update()
        {
            base.Update();
            _animator.SetInteger("attackIndex", _attackIdx);
        }

        public override void Attack()
        {
            base.Attack();
            _attackIdx = 1 - _attackIdx;
        }
    }
}
