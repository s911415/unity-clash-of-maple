using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Mob04 : Monster
    {
        [SerializeField]
        protected AudioClip _attack2AudioClip;

        public Mob04() : base(4)
        {
        }
    }
}
