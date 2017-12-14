using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Scene;
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
        private float _attackCount = 0f;
        [SerializeField]
        private int _playerID = -1;
        private Monster _target = null;

        // finalTartget 敵方主堡
        private Vector3 _finalTarget;

        protected virtual void Start()
        {
            // 開始先鎖定主堡位置
            var gen = GetSceneLogic<FightSceneLogic>().MapGridGenerator;
            _finalTarget = (_playerID == 0) ? new Vector3(190.0f, 0.0f, 50.0f) : new Vector3(15.0f, 0.0f, 50.0f);
            animator = GetComponent<Animator>();
            _sprite = transform.Find("Image").GetComponent<SpriteRenderer>();
            _body = GetComponent<Rigidbody>();
            Debug.Assert(_sprite != null);
            Debug.Assert(_body != null);
        }

        protected virtual void FixedUpdate()
        {
            Walk();
            Find();
            Attack();
        }
        public virtual void Walk()
        {
            if (action == Action.Walk)
            {
                Vector3 v3 = _finalTarget - transform.position;
                v3 = v3.normalized * _speed * 0.1f;
                transform.Translate(v3);
            }
        }
        protected virtual void Find()
        {
            if (!_target || _target._hp<=0)
            {
                Monster[] monsterList = GetSceneLogic<FightSceneLogic>().GetAllMonsterInfo();
                float[] distanceArray = new float[monsterList.Length];

                for (int i = 0; i < monsterList.Length; i++)
                {
                    var m = monsterList[i];
                    float distance = float.MaxValue;

                    if (m._playerID != _playerID)
                    {
                        Vector3 myPoint = gameObject.transform.position;
                        Vector3 targetPoint = m.gameObject.transform.position;
                        distance = Vector3.Distance(myPoint, targetPoint);
                    }

                    distanceArray[i] = distance;
                }

                int minDistanceIndex = Helper.GetMinIndex(distanceArray);

                if (minDistanceIndex < 0 || distanceArray[minDistanceIndex] > _info.AttackRange) return;

                if (monsterList[minDistanceIndex] != this)
                {
                    _target = monsterList[minDistanceIndex];
                }
            }
        }
        public virtual void Attack()
        {
            if (_attackCount > 0)
            {
                _attackCount -= Time.deltaTime;
                return;
            }

            if (!_target)
            {
                action = Action.Walk;
                return;
            }

            action = Action.Attack;
            _target.Damage(_info.Attack);
            _attackCount = _info.AttackSpeed;
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

        public Monster SetInfo(int playerID, int maxHP, int attack, int speed)
        {
            this._playerID = playerID;
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
            _hp -= damage;

            //TODO: 彈數字
            if (_hp < 0) Die();
        }

        public virtual void Recovery(int recover)
        {
            _hp += recover;
        }



        public virtual void Die()
        {
            action = Action.Die;
            var container = GameObject.Find("PendingRemoveMonster");
            this.transform.parent = container.transform;
        }

        public void Remove()
        {
            Debug.Log("RRR");
            Destroy(gameObject);
        }

        public void AfterAttack()
        {
            action = Action.Walk;
        }

        public virtual void Skill1()
        {
        }

        public virtual void Skill2()
        {
        }



        public virtual void Idle()
        {
        }

        public void ShowHpChangedNumber(int damage)
        {
            throw new NotImplementedException();
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