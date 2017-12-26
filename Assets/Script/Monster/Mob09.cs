using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Mob09 : Monster
    {
        public Mob09() : base(9)
        {
        }

        protected override void DropItem()
        {
            _scene.ItemGenerator.DropItem(this.transform.localPosition, 5, _playerID);
        }
    }
}
