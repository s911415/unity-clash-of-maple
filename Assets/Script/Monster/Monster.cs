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
        protected Info _info;
        [SerializeField]
        protected int _hp, _maxHP, _attack, _speed;


        public enum Action
        {
            Idle, Walk, Attack, Die
        }

        public Action action = Action.Idle;

        public int MAX_HP => _maxHP;

        protected override void Awake()
        {
            base.Awake();
            _info = Manager.MonsterInfoCollection[id];
        }

        public Monster SetInfo(int maxHP, int attack, int speed)
        {
            this._maxHP = maxHP;
            this._attack = attack;
            this._speed = speed;
            return this;
        }

        public void Initialize()
        {
            this._hp = _maxHP;
        }

        public virtual void Damage(int damage)
        {
            throw new NotImplementedException();
        }

        public virtual void Recovery(int recover)
        {
            throw new NotImplementedException();
        }

        public virtual void Attack()
        {
        }

        public virtual void Die()
        {
        }

        public virtual void Skill1()
        {
        }

        public virtual void Skill2()
        {
        }

        public virtual void Walk()
        {
        }

        public virtual void Idle()
        {
        }
    }
}