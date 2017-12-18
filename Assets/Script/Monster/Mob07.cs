using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Mob07 : Monster
    {

        protected uint _recoverTimer;
        public Mob07() : base(7)
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
                    Recovery(100);
                }
            }, 1000);
        }
    }
}
