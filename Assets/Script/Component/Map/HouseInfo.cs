using NTUT.CSIE.GameDev.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NTUT.CSIE.GameDev.Game;
using UnityEngine.Serialization;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.UI;
using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Component.Numbers;

namespace NTUT.CSIE.GameDev.Component.Map
{
    public class HouseInfo : HurtableObject
    {
        private const int NONE = -1;
        public int scale = 1;
        [SerializeField]
        private ulong _houseId;
        [SerializeField]
        private SpriteRenderer _houseRenderer;
        // 房屋基本資訊
        [SerializeField]
        private HouseType type;
        [SerializeField]
        protected int _hp;
        public string houseName;
        private Sprite[][] _houseImages;
        [SerializeField]
        private Sprite[] _houseImage = null, _houseImage2 = null;
        // 出產怪物資訊
        private int _monsterNum;
        [SerializeField]
        private int _extraAttack, _extraHp, _extraSpeed;
        [FormerlySerializedAs("Remaining Next Spawn Time (Readonly)")]
        public int RemainingNextSpawnTime;
        private float _lastSpawnTime = 0f;
        [SerializeField]
        private Point _position;
        [SerializeField]
        private Direction _direction = Direction.Right;
        private int _playerID;
        private int _upgAttackCnt, _upgHpCnt, _upgSpeedCnt;
        private NumberCollection _numberCollection;
        private bool _died = false;
        private FightSceneLogic _scene;
        [SerializeField]
        private GameObject _explosionPrefab;
        [SerializeField]
        private bool _inTerroristAttack;

        public delegate void HouseDestroyEvent(Point p);
        public event HouseDestroyEvent OnHouseDestroy;

        public enum HouseType {Empty, Building, Summon, Master}

        public enum UpgradeType {Attack, HP, Speed}

        public HouseInfo()
        {
            _hp = 0;
            houseName = "空地";
            _extraAttack = _extraHp = _extraSpeed = 0;
            _upgAttackCnt = _upgHpCnt = _upgSpeedCnt = 0;
        }

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(_houseImage != null && _houseImage.Length == 4);
            Debug.Assert(_houseImage2 != null && _houseImage2.Length == 4);
            _houseImages = new Sprite[][] { _houseImage, _houseImage2 };
            _scene = GetSceneLogic<FightSceneLogic>();
        }

        public HouseType Type
        {
            set
            {
                type = value;

                if (type == HouseType.Empty)
                {
                    _hp = 0;
                    houseName = "空地";
                }
                else if (type == HouseType.Building)
                {
                    _hp = MAX_HP;
                    houseName = "建築";
                }
                else if (type == HouseType.Summon)
                {
                    _lastSpawnTime = Time.time;
                }
                else if (type == HouseType.Master)
                {
                    houseName = "主塔";
                }

                _houseRenderer.sprite = _houseImages[_playerID][(int)type];
            }

            get
            {
                return type;
            }
        }

        public override bool Alive => _hp > 0 && !_died;

        public HouseInfo SetPosition(int row, int col)
        {
            this._position = new Point(row, col);
            return this;
        }

        public HouseInfo SetId(ulong id)
        {
            this._houseId = id;
            return this;
        }

        public HouseInfo SetType(HouseType type)
        {
            this.Type = type;
            return this;
        }

        public void TerroristAttack(uint time)
        {
            _inTerroristAttack = true;
            SetTimeout(() => _inTerroristAttack = false, time);
        }

        public void Initialize()
        {
            this.name = string.Format("House #{0} ({1}, {2})", _houseId, _position.Row, _position.Column);
            var gen = _scene.MapGridGenerator;
            var pos = Helper.Clone(gen[_position.Row, _position.Column].gameObject.transform.localPosition);
            pos.y = 0;
            gameObject.transform.localPosition = pos;
            _houseRenderer.sprite = _houseImages[_playerID][(int)type];

            if (_direction == Direction.Left)
                _houseRenderer.flipX = true;
            else
                _houseRenderer.flipX = false;

            _died = false;
        }

        public int MonsterNumber
        {
            set
            {
                _monsterNum = value;
            }
        }

        public Monster.Info MonsterInfo
        {
            get
            {
                if (_monsterNum == NONE)
                    return null;

                return this.Manager.MonsterInfoCollection[_monsterNum];
            }
        }

        public HouseInfo SetPlayerID(int id)
        {
            this.PlayerID = id;
            return this;
        }

        public int PlayerID
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

        private void Update()
        {
            RemainingNextSpawnTime = -1;

            if (type == HouseInfo.HouseType.Summon)
            {
                RemainingNextSpawnTime = System.Convert.ToInt32(_lastSpawnTime + MonsterInfo.SpawnInterval - Time.time);

                if (Time.time - _lastSpawnTime > MonsterInfo.SpawnInterval)
                {
                    Spawn();
                    _lastSpawnTime = Time.time;
                }
            }
        }

        private void Spawn()
        {
            if (_inTerroristAttack)
                return;

            Debug.Log(string.Format("召喚: {0}", MonsterInfo.Name));
            _scene.SpawnMonster(MonsterInfo, _playerID, this);
        }

        private void Die()
        {
            if (_died) return;

            _died = true;
            GameObject explosion = Instantiate(_explosionPrefab);
            explosion.transform.position = transform.position + new Vector3(0f, 0f, -0.7f);
            _scene.HouseGenerator.DestroyHouse(this._position);
            Debug.Log("eee");
            OnHouseDestroy?.Invoke(_position);
        }

        public HouseInfo SetDirection(Direction dir)
        {
            this.Direction = dir;
            return this;
        }

        public Direction Direction
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
        internal void _ResetSpawnCounter()
        {
            if (type == HouseInfo.HouseType.Summon)
            {
                _lastSpawnTime = -MonsterInfo.SpawnInterval - 1;
            }
        }

        protected override void Start()
        {
            base.Start();
            _numberCollection = _scene.NumberCollection;
        }

        public override void Damage(int attack)
        {
            _hp -= attack;
            _numberCollection.ShowNumber(
                this.gameObject,
                _playerID == 0 ? NumberCollection.Type.Violet : NumberCollection.Type.Red,
                (uint)attack
            );

            if (_hp <= 0)
            {
                Die();
            }
        }

        public override void Recovery(int recovery)
        {
            _hp += recovery;
            _numberCollection.ShowNumber(this.gameObject, NumberCollection.Type.Blue, (uint)recovery);
        }

        public void ResetMonster()
        {
            Type = HouseInfo.HouseType.Building;
            _monsterNum = NONE;
            _extraAttack = 0;
            _extraHp = 0;
            _extraSpeed = 0;
        }

        public void UpgradeAttack()
        {
            _extraAttack++;
            _upgAttackCnt++;
        }

        public void UpgradeSpeed()
        {
            _extraSpeed++;
            _upgSpeedCnt++;
        }

        public void UpgradeHP()
        {
            _extraHp += 10;
            _upgHpCnt++;
        }

        public int RealAttack => MonsterInfo == null ? 0 : (MonsterInfo.Attack + _extraAttack);
        public int RealHP => MonsterInfo == null ? 0 : (MonsterInfo.MaxHP + _extraHp);
        public int RealSpeed => MonsterInfo == null ? 0 : (MonsterInfo.Speed + _extraSpeed);

        public override int HP => _hp;
        public override int MAX_HP => Config.HOUSE_MAX_HP;

        public int UpgradeAttackCount => _upgAttackCnt;
        public int UpgradeSpeedCount => _upgSpeedCnt;
        public int UpgradeHpCount => _upgHpCnt;

        public int GetUpgradeCount(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.Attack:
                    return _upgAttackCnt;

                case UpgradeType.HP:
                    return _upgHpCnt;

                case UpgradeType.Speed:
                    return _upgSpeedCnt;
            }

            return 0;
        }

        public void UpgradeAttribute(UpgradeType type, int value)
        {
            switch (type)
            {
                case UpgradeType.Attack:
                    _extraAttack = value;
                    break;

                case UpgradeType.HP:
                    _extraHp = value;
                    break;

                case UpgradeType.Speed:
                    _extraSpeed = value;
                    break;
            }
        }

        public ulong ID => _houseId;

        public Point Position => _position;
    }
}