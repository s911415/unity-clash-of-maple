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
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private SpriteRenderer _sprite;
        [SerializeField]
        private Direction _direction = Direction.Left;
        private Rigidbody _body;

        [SerializeField]
        private int _playerID = -1;


        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
            _sprite = transform.Find("Image").GetComponent<SpriteRenderer>();
            _body = GetComponent<Rigidbody>();
            Debug.Assert(_sprite != null);
            Debug.Assert(_body != null);
        }

        protected virtual void FixedUpdate()
        {
            if (action == Action.Walk)
            {
                var dir = _direction == Direction.Left ? -1 : 1;
                _body.AddForce(_speed * new Vector3(1, 0, 0) * dir, ForceMode.Acceleration);
            }
        }

        protected virtual void Update()
        {
            animator.SetInteger("action", (int)action);

            if (_direction == Direction.Left)
                _sprite.flipX = false;
            else
                _sprite.flipX = true;

            if (CheckOutOfMap())
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual bool CheckOutOfMap()
        {
            var x = transform.localPosition.x;
            return x < 0 || x > 200;
        }

        public enum Action
        {
            Walk, Attack, Die
        }

        public Action action = Action.Walk;

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

        public virtual int PlayerID
        {
            get
            {
                return _playerID;
            }
            set
            {
                _playerID = value;
            }
        }

        public virtual Direction Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        protected int VectorOffset
        {
            get
            {
                if (_playerID == Manager.DEFAULT_PLAYER_ID) return 1;

                if (_playerID == Manager.ROBOT_PLAYER_ID) return -1;

                return 0;
            }
        }
    }
}