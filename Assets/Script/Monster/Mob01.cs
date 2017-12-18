using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Mob01 : Monster
    {
        protected bool _isClone = false;
        public Mob01() : base(1)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_isClone) return;

            /*
            var obj = Instantiate(this, this.transform.parent);
            var mob = obj.GetComponent<Mob01>();
            mob._isClone = true;
            obj.SetInfo(_playerID, _maxHP, _attack, _speed).Initialize();
            var offset = Random.insideUnitSphere;
            offset.y = 0;
            obj.transform.localPosition = this.transform.localPosition + offset;
            */
        }
    }
}
