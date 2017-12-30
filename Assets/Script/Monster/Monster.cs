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
    public class Monster : HurtableObject
    {
        private const float FIND_TARGET_DELTA = 1f;
        protected static ulong _monsterCounter = 0;

        protected static GameObject _statusDizzy = null, _statusPoison = null;
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
        protected HurtableObject _target = null;
        protected NumberCollection _numberCollection;
        [SerializeField]
        protected bool _died;
        [SerializeField]
        protected bool _freeze, _poisoning;
        [SerializeField]
        protected GameObject _statusObj;
        protected uint _freezeTimer = 0;
        protected uint _poisonTimerInterval, _poisonTimerTimeout;
        protected int _poisonValue;
        protected FightSceneLogic _scene;
        public delegate void MonsterKilledEvent(int monsterID);
        public event MonsterKilledEvent OnMonsterKilled;
        public delegate void StatusChangeEventHandler();
        public event StatusChangeEventHandler OnStatusChanged;
        private uint _failAttackCount = 0;
        private float _lastFindTargetTime = 0;

        // finalTartget 敵方主堡
        private Vector3 _finalTarget;
        private bool _isArrival = false;

        protected Monster(int mobID)
        {
            this._monsterID = mobID;
        }

        protected override void Start()
        {
            base.Start();
            _scene = GetSceneLogic<FightSceneLogic>();
            // 開始先鎖定主堡位置
            var gen = _scene.MapGridGenerator;
            _finalTarget = (_playerID == 0) ? gen[4, 18].transform.localPosition : gen[4, 1].transform.localPosition;
            _finalTarget += new Vector3(-2.5f * VectorOffset, 0, -5);
            _animator = GetComponent<Animator>();
            _sprite = transform.Find("Image").GetComponent<SpriteRenderer>();
            _audio = GetComponent<AudioSource>();
            _body = GetComponent<Rigidbody>();
            _numberCollection = _scene.NumberCollection;
            Debug.Assert(_sprite != null);
            Debug.Assert(_body != null);
            this.GetComponent<Collider>().enabled = false;
            _audio.spatialBlend = 0.75f;
        }

        protected virtual void FixedUpdate()
        {
            if (_died || _hp < 0)
            {
                Die();
                return;
            }

            if (_freeze)
            {
                _action = Action.Walk;
                return;
            }

            if (_action == Action.Walk)
            {
                CollectItem();
                FindNearestTarget();
                Walk();
            }

            if (_action == Action.Walk || _action == Action.Attack)
            {
                if (!_target) _action = Action.Walk;

                if (InRange(_finalTarget, 1))
                    _isArrival = true;
                else
                    _isArrival = false;
            }

            if (_target && IsAllowAttack(_target)) Attack();
        }

        public override int HP => _hp;

        public virtual void Walk()
        {
            _action = Action.Walk;
            Vector3 v3 = (
                             _target && _target.Alive ?
                             _target.transform.localPosition :
                             _finalTarget
                         ) - transform.localPosition;
            v3.z -= 5;
            v3.y = 0;
            v3 = v3.normalized * (_speed * 0.1f);
            v3.y = 0;

            if (!_freeze)
            {
                if (v3.x > 0)
                {
                    _direction = Direction.Right;
                }
                else if (v3.x < 0)
                {
                    _direction = Direction.Left;
                }

                transform.Translate(v3, Space.Self);
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
                (!_target || !_target.Alive || Time.time - _lastFindTargetTime > FIND_TARGET_DELTA)
            )
            {
                _target = null;
                var enemiesList = GetAttackableObject();
                var currentPos = this.transform.localPosition;
                float[] distanceArray = enemiesList.Select(
                                            m => Vector3.Distance(m.transform.localPosition, currentPos)
                                        ).ToArray();
                int minDistanceIndex = Helper.GetMinIndex(distanceArray);

                if (minDistanceIndex < 0) return;

                if (enemiesList[minDistanceIndex] != this)
                {
                    _target = enemiesList[minDistanceIndex];
                    _lastFindTargetTime = Time.time;
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

        public override bool Alive => this&& !_died&& _hp > 0;

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
            if (_action != Action.Attack) return;

            const uint FAIL_COUNT = 2;

            if (_attackAudioClip != null)
                _audio.PlayOneShot(_attackAudioClip);

            var nearAttackableObject = GetAttackableObject(this._info.AttackRange);

            if (nearAttackableObject.Count == 0)
                _failAttackCount++;
            else
                _failAttackCount = 0;

            if (_failAttackCount > FAIL_COUNT)
            {
                _target = null;
                _action = Action.Walk;
            }

            DamageTargets(nearAttackableObject);
        }
        protected virtual void DamageOneTarget(HurtableObject target)
        {
            DamageTargets(new HurtableObject[] { target });
        }

        protected virtual void DamageTargets(IEnumerable<HurtableObject> targetList)
        {
            if (_action != Action.Attack || _target == null || !_target.gameObject) return;

            foreach (var m in targetList)
            {
                if (!m) continue;

                // Debug.Log(string.Format("DamageTarget: {0}", m.name));
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

            return Helper.GetRandomValueBaseOnValue(_attack, 0.15f);
        }

        public virtual void SetPoisoning(int hurt, uint timeout, uint interval)
        {
            ClearTimeout(_poisonTimerTimeout);

            if (!IsGodMode)
            {
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
                    OnStatusChanged?.Invoke();
                }, timeout);
            }

            OnStatusChanged?.Invoke();
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

        public override int MAX_HP => _maxHP;

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
            this.OnStatusChanged += StatusChanged;
        }

        public virtual void Freeze(uint ms)
        {
            if (!IsGodMode)
            {
                this._freeze = true;
                OnStatusChanged?.Invoke();
                ClearTimeout(_freezeTimer);
                _freezeTimer = SetTimeout(() =>
                {
                    this._freeze = false;
                    OnStatusChanged?.Invoke();
                }, ms);
            }
        }

        public virtual void Freeze()
        {
            this._freeze = true;
            OnStatusChanged?.Invoke();
            _statusObj.SetActive(false);
        }

        public virtual void Damage(int damage, bool silence)
        {
            if (IsGodMode) damage = 0;

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

        public override void Recovery(int recover)
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

        private static GameObject _pendingRemoveMonsterContainer;
        public virtual void Die()
        {
            _action = Action.Die;

            if (_died) return;

            _died = true;

            if (_dieAudioClip != null)
                _audio.PlayOneShot(_dieAudioClip);

            if (_pendingRemoveMonsterContainer == null || !_pendingRemoveMonsterContainer)
                _pendingRemoveMonsterContainer = GameObject.Find("PendingRemoveMonster");

            this.transform.parent = _pendingRemoveMonsterContainer.transform;
            DropItem();
            OnMonsterKilled?.Invoke(this._monsterID);
            // var rival = _scene.GetPlayerAt(1 - _playerID);
            // rival.KilledMonster(_monsterID);
        }

        protected virtual void DropItem()
        {
        }

        public bool IsGodMode => _scene.GetPlayerAt(_playerID).IsGodMode;

        public void Remove()
        {
            Destroy(gameObject);
        }

        public void AfterAttack()
        {
            _action = Action.Walk;
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

        protected Monster[] GetEnemies(float range = float.PositiveInfinity)
        {
            Monster[] monsterList = _scene.GetAllMonsterInfo();
            var query = monsterList.Where(m => m._playerID != this._playerID);

            if (!float.IsInfinity(range))
            {
                var myPos = this.transform.localPosition;
                query = query.Where(
                            m =>
                            m &&
                            this &&
                            InRange(m.transform.localPosition, range) &&
                            m.Alive
                        );
            }

            return query.ToArray();
        }

        protected HouseInfo[] GetEnemyHouses(float range = float.PositiveInfinity)
        {
            HouseInfo[] houseList = _scene.HouseGenerator.GetAllHouseInfo();
            var query = houseList.Where(m => m.PlayerID != this._playerID);

            if (!float.IsInfinity(range))
            {
                var myPos = this.transform.localPosition;
                query = query.Where(
                            h =>
                            h &&
                            this &&
                            InRange(h.transform.position, range) &&
                            h.Alive &&
                            h.Type != HouseInfo.HouseType.Master
                        )
                        ;
            }

            return query.ToArray();
        }

        protected IReadOnlyList<HurtableObject> GetAttackableObject(float range = float.PositiveInfinity)
        {
            List<HurtableObject> list = new List<HurtableObject>();
            var houseList = GetEnemyHouses(range);
            var monsterList = GetEnemies(range);
            list.AddRange(houseList);
            list.AddRange(monsterList);
            var p = _scene.GetPlayerAt(1 - _playerID);

            if (_isArrival || InRange(p.gameObject.transform.localPosition, range))
                list.Add(p);

            return list;
        }

        protected void CollectItem()
        {
            var items = _scene.ItemGenerator.AllItems
                        .Where(i => i.PlayerID != _playerID)
                        .Where(i => InRange(i.gameObject.transform.localPosition, _info.AttackRange))
                        .ToArray();

            foreach (var i in items)
                i.Collect();
        }

        public bool IsAllowAttack(HurtableObject m)
        {
            return InRange(m.transform.localPosition, _info.AttackRange);
        }

        protected Monster[] GetFriends(float range = float.PositiveInfinity)
        {
            Monster[] monsterList = _scene.GetAllMonsterInfo();
            var query = monsterList.Where(m => m._playerID == this._playerID);

            if (!float.IsInfinity(range))
            {
                query = query.Where(
                            m => m && this && InRange(m.transform.localPosition, range)
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

        public override void Damage(int damage)
        {
            Damage(damage, false);
        }

        protected virtual void StatusChanged()
        {
            InitStatus();
            var dizzy = _statusObj.transform.Find("StatusDizzy");
            var poison = _statusObj.transform.Find("StatusPoison");
            Action<GameObject> setStatusObj = (obj) =>
            {
                Vector3 v = obj.transform.localPosition;
                v.x = 0;
                v.y = .0625f;
                obj.transform.localPosition = v;
                obj.name = obj.name.Substring(0, obj.name.LastIndexOf("(Clone)")).Trim();
            };

            if (_poisoning)
            {
                if (!poison)
                    setStatusObj(GameObject.Instantiate(_statusPoison, _statusObj.transform));
            }
            else
            {
                if (poison)
                    Destroy(poison.gameObject);
            }

            if (_freeze)
            {
                _animator.enabled = false;

                if (!dizzy)
                    setStatusObj(GameObject.Instantiate(_statusDizzy, _statusObj.transform));
            }
            else
            {
                _animator.enabled = true;

                if (dizzy)
                    Destroy(dizzy.gameObject);
            }
        }

        protected static void InitStatus()
        {
            if (!_statusDizzy)
                _statusDizzy = Resources.Load<GameObject>("Prefab/Monster/StatusDizzy");

            if (!_statusPoison)
                _statusPoison = Resources.Load<GameObject>("Prefab/Monster/StatusPoison");
        }

        protected bool IsInRange(HurtableObject obj)
        {
            var myPos = this.transform.localPosition;
            var objPos = obj.transform.localPosition;
            var rect1Center = myPos + _collider.bounds.center;
            var rect1Size = _collider.bounds.size;
            var rect2Center = objPos + obj.Bound.center;
            var rect2Size = obj.Bound.size;
            var rect1 = new
            {
                x = rect1Center.x  - rect1Size.x / 2, y = rect1Center.z - rect1Size.z / 2,
                width = rect1Size.x, height = rect1Size.z
            };
            var rect2 = new
            {
                x = rect2Center.x - rect2Size.x / 2, y = rect2Center.z - rect2Size.z / 2,
                width = rect2Size.x, height = rect2Size.z
            };
            return (rect1.x < rect2.x + rect2.width &&
                    rect1.x + rect1.width > rect2.x &&
                    rect1.y < rect2.y + rect2.height &&
                    rect1.height + rect1.y > rect2.y);
        }

        public ulong ID => _id;
    }
}
