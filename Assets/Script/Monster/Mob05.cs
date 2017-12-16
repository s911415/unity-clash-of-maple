using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Mob05 : Monster
    {
        [SerializeField]
        protected AudioClip _attack2AudioClip;
        public Mob05() : base(5)
        {
        }
    }
}
