using NTUT.CSIE.GameDev.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Component
{
    public abstract class HurtableObject : CommonObject
    {
        protected Collider _collider;

        protected override void Awake()
        {
            base.Awake();
            _collider = this.GetComponent<Collider>();
        }

        protected virtual void Start()
        {
            _collider.enabled = false;
        }

        public abstract int MAX_HP
        {
            get;
        }

        public abstract bool Alive
        {
            get;
        }
        public abstract int HP
        {
            get;
        }

        public abstract void Damage(int damage);
        public abstract void Recovery(int recover);

        public Bounds Bound => _collider.bounds;

    }
}
