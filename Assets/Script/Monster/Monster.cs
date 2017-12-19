using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
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
        protected Action _action = Action.Walk;
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
        [SerializeField]
        protected Monster _target = null;
        private HouseInfo _houseTarget = null;
        protected NumberCollection _numberCollection;
        [SerializeField]
        protected bool _died;
        [SerializeField]
        protected bool _freeze, _poisoning;
        protected uint _freezeTimer = 0;
        protected uint _poisonTimerInterval, _poisonTimerTimeout;
        protected int _poisonValue;

        // finalTartget 敵方主堡
        private Vector3 _finalTarget;
        private bool _isArrival = false;
        private bool _isFindHouse = false;

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
            if (_isArrival)
            {
                _action = Action.Attack;
                return;
            }

            if (_action == Action.Walk)
            {
                Walk();
                FindNearestTarget();
                FindHouse();
            }

            if (_action == Action.Walk || _action == Action.Attack)
            {
                if (!_target && !_isFindHouse) _action = Action.Walk;
                if (Vector3.Distance(_finalTarget, transform.localPosition) < _info.AttackRange)
                    _isArrival = true;
                else
                    _isArrival = false;

            }

            if (_target && IsAllowAttack(_target)) Attack();

            if (_died || _hp < 0)
            {
                Die();
            }
        }

        public virtual void Walk()
        {
            _action = Action.Walk;
            Vector3 v3 = _finalTarget - transform.localPosition;
            v3 = v3.normalized * _speed * 0.1f;
            v3.y = 0;

            if (!_freeze) transform.Translate(v3);
        }

        protected virtual void FindHouse()
        {
            HouseInfo[] houseList = GetSceneLogic<FightSceneLogic>().HouseGenerator.GetAllHouseInfo();
            foreach (HouseInfo h in houseList)
            {
                if(Vector3.Distance(transform.position, h.transform.position) < _info.AttackRange + 1.5f &&
                    (h.PlayerID != _playerID))
                {
                    _isFindHouse = true;
                    _houseTarget = h;
                    _action = Action.Attack;
                    return;
                }
                _isFindHouse = false;
                _houseTarget = null;
            }
        }

        protected virtual void FindNearestTarget()
        {
            if (_freeze)
            {
                _target = null;
                _action = Action.Walk;
                return;
            }

            if (
                (!_target || _target._hp <= 0)
            )
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
                if (!IsAllowAttack(_target))
                {
                    _action = Action.Walk;
                }
            }
        }

        public bool Alive => this&& !_died&& _hp > 0;

        public virtual void Attack()
        {
            if (_attackCount > 0)
            {
                _attackCount -= Time.deltaTime;
                return;
            }

            _action = Action.Attack;
            _attackCount = _info.AttackSpeed;
        }

        /// <summary>   Damage target, Called From Animator event </summary>
        public virtual void DamageTarget()
        {
            if (_isArrival)
            {
                int opponentID = Math.Abs(_playerID - 1);
                if (_playerID == 0) Debug.Log("Damage house" + opponentID);
                else Debug.Log("Robot: Damage house" + opponentID);
                GetSceneLogic<FightSceneLogic>().GetPlayerAt(opponentID).Damage(CalcDamageValue());
                if (_attackAudioClip != null)
                    _audio.PlayOneShot(_attackAudioClip);
            }
            else if (_isFindHouse)
            {
                if (_houseTarget == null)
                {
                    _action = Action.Walk;
                    return;
                }                    
                _houseTarget.Damage(CalcDamageValue());
                if (_attackAudioClip != null)
                    _audio.PlayOneShot(_attackAudioClip);
                // 打一棟房子
                List<HouseInfo> houseList = new List<HouseInfo>();
                houseList.Add(_houseTarget);
                DamageTarget(houseList);
            }
            else
            {
                var nearMonsters = GetEnemies(this._info.AttackRange);
                DamageTarget(nearMonsters);
            }
        }

        protected virtual void DamageTarget<T>(IEnumerable<T> targetList) where T: CommonObject, IHurtable
        {
            if (_action != Action.Attack || _target == null || !_target.gameObject) return;

            foreach (var m in targetList)
            {
                if (!m) continue;

                Debug.Log(string.Format("DamageTarget: {0}", m.name));

                m.Damage(CalcDamageValue());
            }
        }

        protected virtual void Update()
        {
            _animator.SetInteger("action", (int)_action);

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
            const float MISS_RATE = 0.0625f;

            if (Random.value < MISS_RATE)
                return 0;

            var offset = _attack * 0.15f;
            return (int)(_attack + Random.Range(-offset, offset));
        }

        public virtual void SetPoisoning(int hurt, uint timeout, uint interval)
        {
            ClearTimeout(_poisonTimerTimeout);

            if (!_poisoning)
            {
                _poisoning = true;
                _poisonTimerInterval = SetInterval(() =>
                {
                    this.Damage(hurt, true);
                }, interval);
            }

            _poisonTimerTimeout = SetTimeout(() =>
            {
                _poisoning = false;
                ClearInterval(_poisonTimerInterval);
            }, timeout);
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

        public virtual void Initialize()
        {
            this._hp = _maxHP;
            this._died = false;
            this._freeze = false;
        }

        public virtual void Freeze(uint ms)
        {
            this._freeze = true;
            ClearTimeout(_freezeTimer);
            _freezeTimer = SetTimeout(() => this._freeze = false, ms);
            Debug.Log(string.Format("{0}被暈眩了", this.name));
        }

        public virtual void Damage(int damage, bool silence)
        {
            _hp -= damage;
            _numberCollection.ShowNumber(
                this.gameObject,
                _playerID == 0 ? NumberCollection.Type.Violet : NumberCollection.Type.Red,
                (uint)damage
            );

            if (_hp > 0)
            {
                if (!silence && _damageAudioClip != null)
                    _audio.PlayOneShot(_damageAudioClip);
            }
        }

        public virtual void Recovery(int recover)
        {
            var oldHP = _hp;
            _hp += recover;

            if (_hp > _maxHP) _hp = _maxHP;

            var offset = _hp - oldHP;

            if (offset > 0)
            {
                _numberCollection.ShowNumber(
                    this.gameObject,
                    NumberCollection.Type.Blue,
                    (uint)offset
                );
            }
        }

        public virtual void Die()
        {
            _action = Action.Die;

            if (_died) return;

            _died = true;
            this.Manager.GetPlayerAt(_playerID).AddMonsterKillCount(this._monsterID);

            if (_dieAudioClip != null)
                _audio.PlayOneShot(_dieAudioClip);

            var container = GameObject.Find("PendingRemoveMonster");
            this.transform.parent = container.transform;
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        public void AfterAttack()
        {
            _action = Action.Walk;
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

        protected Monster[]  GetEnemies(float range = float.MaxValue)
        {
            Monster[] monsterList = GetSceneLogic<FightSceneLogic>().GetAllMonsterInfo();
            var query = monsterList.Where(m => m._playerID != this._playerID);

            if (range < float.MaxValue)
            {
                query = query.Where(
                            m => Vector3.Distance(m.transform.position, this.transform.position) <= range
                        )
                        .Where(m => m.Alive)
                        ;
            }

            return query.ToArray();
        }

        public bool IsAllowAttack<T>(T m) where T: CommonObject, IHurtable
        {
            return Vector3.Distance(m.transform.position, this.transform.position) <= _info.AttackRange;
        }

        protected Monster[] GetFriends(float range = float.MaxValue)
        {
            Monster[] monsterList = GetSceneLogic<FightSceneLogic>().GetAllMonsterInfo();
            var query = monsterList.Where(m => m._playerID == this._playerID);

            if (range < float.MaxValue)
            {
                query = query.Where(
                            m => m && this && Vector3.Distance(m.transform.position, this.transform.position) <= range
                        );
            }

            return query.ToArray();
        }

        protected virtual void OnDestroy()
        {
            CleanUp();
        }

        public virtual void ReduceAttackInTime(int reduce, uint ms)
        {
            int curAtt = _attack;
            _attack -= reduce;
            SetTimeout(() => _attack = curAtt, ms);
        }

        protected virtual void CleanUp()
        {
            ClearInterval(_poisonTimerInterval);
            ClearTimeout(_poisonTimerTimeout);
        }

        public void Damage(int damage)
        {
            Damage(damage, false);
        }

        public ulong ID => _id;
    }
}