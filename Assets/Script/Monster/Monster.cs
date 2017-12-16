﻿using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Numbers;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NTUT.CSIE.GameDev.Monster
{
    [Serializable]
    public class Monster : CommonObject, IHurtable
    {
        protected static ulong _monsterCounter = 0;

        [SerializeField]
        protected AudioClip _attackAudioClip, _damageAudioClip, _dieAudioClip;
        protected AudioSource _audio;
        [SerializeField]
        protected ulong _id;
        [SerializeField]
        protected readonly int _monsterID;
        [SerializeField]
        protected Info _info;
        [SerializeField]
        protected int _hp, _maxHP, _attack, _speed;
        [SerializeField]
        protected Animator _animator;
        [SerializeField]
        protected SpriteRenderer _sprite;
        [SerializeField]
        protected Direction _direction = Direction.Left;
        protected Rigidbody _body;
        [SerializeField]
        protected float _attackCount = 0f;
        [SerializeField]
        protected int _playerID = -1;
        protected Monster _target = null;
        protected NumberCollection _numberCollection;

        // finalTartget 敵方主堡
        private Vector3 _finalTarget;

        protected Monster(int mobID)
        {
            this._monsterID = mobID;
        }

        protected virtual void Start()
        {
            // 開始先鎖定主堡位置
            var gen = GetSceneLogic<FightSceneLogic>().MapGridGenerator;
            _finalTarget = (_playerID == 0) ? gen[4, 18].transform.localPosition : gen[4, 1].transform.localPosition;
            _finalTarget += new Vector3(-2.5f * VectorOffset, 0, -5);
            _animator = GetComponent<Animator>();
            _sprite = transform.Find("Image").GetComponent<SpriteRenderer>();
            _audio = GetComponent<AudioSource>();
            _body = GetComponent<Rigidbody>();
            _numberCollection = GetSceneLogic<FightSceneLogic>().NumberCollection;
            Debug.Assert(_sprite != null);
            Debug.Assert(_body != null);
            this.GetComponent<Collider>().enabled = false;
        }

        protected virtual void FixedUpdate()
        {
            Walk();
            FindNearestTarget();
            Attack();
        }

        public virtual void Walk()
        {
            if (action == Action.Walk)
            {
                Vector3 v3 = _finalTarget - transform.position;
                v3 = v3.normalized * _speed * 0.1f;
                v3.y = 0;
                transform.Translate(v3);
            }
        }

        protected virtual void FindNearestTarget()
        {
            if (!_target || _target._hp <= 0)
            {
                _target = null;
                Monster[] enemiesList = GetEnemies(_info.AttackRange);
                float[] distanceArray = enemiesList.Select(
                                            m => Vector3.Distance(m.transform.position, this.transform.position)
                                        ).ToArray();
                int minDistanceIndex = Helper.GetMinIndex(distanceArray);

                if (minDistanceIndex < 0 || distanceArray[minDistanceIndex] > _info.AttackRange) return;

                if (enemiesList[minDistanceIndex] != this)
                {
                    _target = enemiesList[minDistanceIndex];
                }
            }
            else
            {
                _target = null;
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
            _attackCount = _info.AttackSpeed;
        }

        public virtual void DamageTarget()
        {
            if (action != Action.Attack || _target == null || !_target.gameObject) return;

            var nearMonsters = GetEnemies(this._info.AttackRange);

            foreach (var m in nearMonsters)
            {
                if (!m) continue;

                Debug.Log(string.Format("DamageTarget: {0}", m.name));

                if (_attackAudioClip != null)
                    _audio.PlayOneShot(_attackAudioClip);

                m.Damage(CalcDamageValue());
            }
        }

        protected virtual void Update()
        {
            _animator.SetInteger("action", (int)action);

            if (_direction == Direction.Left)
                _sprite.flipX = false;
            else
                _sprite.flipX = true;

            if (CheckOutOfMap())
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual int CalcDamageValue()
        {
            var offset = _attack * 0.25f;
            return (int)(_attack + Random.Range(-offset, offset));
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
            _info = Manager.MonsterInfoCollection[_monsterID];
        }

        public Monster SetInfo(int playerID, int maxHP, int attack, int speed)
        {
            this._playerID = playerID;
            this._maxHP = maxHP;
            this._attack = attack;
            this._speed = speed;
            this._id = _monsterCounter++;
            this.name = string.Format("Mob #{0}", _id);
            return this;
        }

        public void Initialize()
        {
            this._hp = _maxHP;
        }

        public virtual void Damage(int damage)
        {
            _hp -= damage;
            _numberCollection.ShowNumber(
                this.gameObject,
                _playerID == 0 ? NumberCollection.Type.Violet : NumberCollection.Type.Red,
                (uint)damage
            );

            if (_hp < 0)
            {
                if (_dieAudioClip != null)
                    _audio.PlayOneShot(_dieAudioClip);

                Die();
            }
            else
            {
                if (_damageAudioClip != null)
                    _audio.PlayOneShot(_damageAudioClip);
            }
        }

        public virtual void Recovery(int recover)
        {
            _hp += recover;
            _numberCollection.ShowNumber(
                this.gameObject,
                NumberCollection.Type.Blue,
                (uint)recover
            );
        }

        public virtual void Die()
        {
            action = Action.Die;
            var container = GameObject.Find("PendingRemoveMonster");
            this.transform.parent = container.transform;
        }

        public void Remove()
        {
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

        /// <summary>   Return 1 if player, 0 if robot. </summary>
        protected int VectorOffset
        {
            get
            {
                if (_playerID == Manager.DEFAULT_PLAYER_ID) return 1;

                if (_playerID == Manager.ROBOT_PLAYER_ID) return -1;

                Debug.LogWarning("0 returned");
                return 0;
            }
        }

        protected Monster[] GetEnemies(float range = float.MaxValue)
        {
            Monster[] monsterList = GetSceneLogic<FightSceneLogic>().GetAllMonsterInfo();
            var query = monsterList.Where(m => m._playerID != this._playerID);

            if (range < float.MaxValue)
            {
                query = query.Where(
                            m => Vector3.Distance(m.transform.position, this.transform.position) <= range
                        );
            }

            return query.ToArray();
        }

        protected Monster[] GetFriends(float range = float.MaxValue)
        {
            Monster[] monsterList = GetSceneLogic<FightSceneLogic>().GetAllMonsterInfo();
            var query = monsterList.Where(m => m._playerID == this._playerID);

            if (range < float.MaxValue)
            {
                query = query.Where(
                            m => Vector3.Distance(m.transform.position, this.transform.position) <= range
                        );
            }

            return query.ToArray();
        }

        public ulong ID => _id;
    }
}