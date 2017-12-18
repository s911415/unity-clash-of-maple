using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Mob05 : Monster
    {
        protected uint _recoverTimer;
        [SerializeField]
        protected AudioClip _attack2AudioClip;
        public Mob05() : base(5)
        {
        }

        protected override void Start()
        {
            base.Start();
            _recoverTimer = SetInterval(() =>
            {
                var friends = GetFriends(5);

                foreach (var m in friends)
                {
                    m.Recovery(20);
                }
            }, 1000);
        }
    }
}
