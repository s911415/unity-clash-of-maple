using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Mob02 : Monster
    {
        protected uint _recoverTimer;
        public Mob02() : base(2)
        {
        }

        protected override void Start()
        {
            base.Start();
            _recoverTimer = SetInterval(() =>
            {
                Recovery(20);
            }, 1000);
        }

        protected override void DropItem()
        {
            _scene.ItemGenerator.DropItem(this.transform.localPosition, 3, _playerID);
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            ClearInterval(_recoverTimer);
        }
    }
}
