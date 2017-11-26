using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    [Serializable]
    public class Monster : CommonObject, IHurtable
    {
        public int id;
        [SerializeField]
        private Info _info;
        [SerializeField]
        private int _hp;

        public enum Action
        {
            Idle, Walk, Attack, Die
        }

        public Action action = Action.Idle;

        public int MAX_HP => _info.MaxHP;

        protected override void Awake()
        {
            base.Awake();
            _info = Manager.MonsterInfoCollection[id];
            _hp = MAX_HP;
        }

        public void Damage(int damage)
        {
            throw new NotImplementedException();
        }

        public void Recovery(int recover)
        {
            throw new NotImplementedException();
        }
    }
}