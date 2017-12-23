using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Component.Numbers;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Player
{
    public class Player : HurtableObject
    {
        public enum UniqueSkill { Attack, Defense }
        public delegate void ValueChangedEventHandler<T>(T value);
        public delegate void ModelChangedEventHandler();
        public delegate void MonsterKilledEventHandler(int monsterID, IReadOnlyDictionary<int, int> monsterKilledSummary);
        public delegate void HouseCreatedEventHandler(HouseInfo house);
        private const int INIT_HOUSES = 6;
        [SerializeField]
        protected int _playerID;
        protected bool _godMode;

        [SerializeField]
        protected Info _info;

        [SerializeField, Range(0, Config.PLAYER_MAX_HP)]
        protected int _hp;

        [SerializeField, Range(0, Config.PLAYER_MAX_MONEY)]
        protected int _money;

        [SerializeField]
        protected List<Tower> _towers;

        [SerializeField]
        protected HashSet<Honors.Honor> _honors;

        private FightSceneLogic _scene;

        private bool _died, _uniqueSkillUsed;
        protected bool _protectedBySeaFood;

        [SerializeField]
        protected AudioSource _audio;

        [SerializeField]
        protected Sprite _protectBySeafoodImage;
        [SerializeField]
        protected AudioClip _protectBySeafoodClip;

        public event ValueChangedEventHandler<int> OnHPChanged;
        public event ValueChangedEventHandler<int> OnMoneyChanged;
        public event ModelChangedEventHandler OnHonorsChanged;
        public event ModelChangedEventHandler OnDied;
        public event HouseCreatedEventHandler OnHouseCreated;
        public event MonsterKilledEventHandler OnKilledMonster;

        [SerializeField]
        private Dictionary<int, int> _beenKilledMonsterCount;
        [SerializeField]
        private int _houseDestroyedCount, _builtHouseCount;

        public override int MAX_HP => Config.PLAYER_MAX_HP;
        protected override void Awake()
        {
            base.Awake();
            _towers = new List<Tower>();
            _honors = new HashSet<Honors.Honor>();
            _hp = MAX_HP;
            _info = Manager.GetPlayerAt(_playerID);
            _scene = GetSceneLogic<FightSceneLogic>();
            _godMode = false;
            _died = false;
            _uniqueSkillUsed = false;
        }

        protected override void Start()
        {
            base.Start();
            InitHouses();
            ResetCounter();
            _protectedBySeaFood = false;
            Attached();
        }

        public void Attached()
        {
            //Call event
            OnHPChanged?.Invoke(_hp);
            OnMoneyChanged?.Invoke(_money);
            OnHonorsChanged?.Invoke();
        }

        public void KilledMonster(int monsterID)
        {
            OnKilledMonster?.Invoke(monsterID, this.MyKilledMonsterInfo);
        }

        public Info Info
        {
            get
            {
                if (_info == null) _info = Manager.GetPlayerAt(_playerID);

                return _info;
            }
        }

        public Result Result
        {
            get
            {
                return new Result(
                           _playerID,
                           _money,
                           _hp,
                           _builtHouseCount,
                           _houseDestroyedCount,
                           this.MyKilledMonsterInfo,
                           _honors
                       );
            }
        }

        public override int HP => _hp;
        public int Money => _money;

        public override bool Alive => _hp > 0;

        public Player SetMoney(int m)
        {
            _money = m;
            OnMoneyChanged?.Invoke(_money);
            return this;
        }

        public Player AddMoney(int m)
        {
            _money += m;

            if (_money > Config.PLAYER_MAX_MONEY) _money = Config.PLAYER_MAX_MONEY;

            OnMoneyChanged?.Invoke(m);
            return this;
        }

        public bool CostMoney(int m)
        {
            if (_money - m < 0)
                return false;

            _money -= m;
            OnMoneyChanged?.Invoke(-m);
            return true;
        }

        public Player AddHonor(Honors.Honor h)
        {
            this._honors.Add(h);
            OnHonorsChanged?.Invoke();
            return this;
        }

        public void DoUniqueSkill(UniqueSkill type)
        {
            if (_uniqueSkillUsed)
                throw new System.Exception("無法使用兩次大絕");

            if ((float)_hp / MAX_HP >= Config.PLAYER_UNIQUE_REQUIRE_HP)
                throw new System.Exception($"HP少於{Config.PLAYER_UNIQUE_REQUIRE_HP:P0}才能使用大絕");

            _uniqueSkillUsed = true;

            if (type == UniqueSkill.Attack)
            {
                TerroristAttack();
            }
            else if (type == UniqueSkill.Defense)
            {
                SeaFoodBless();
            }
        }

        public void TerroristAttack()
        {
            var rivalHouseArray = _scene.HouseGenerator.GetAllHouseInfo().Where(h => h.PlayerID != _playerID).ToArray();

            foreach (var h in rivalHouseArray)
                h.TerroristAttack(Config.PLAYER_UNIQUE_SKILL_TIME);

            _scene.SetTerroristAttack(true);
            SetTimeout(() =>
            {
                _scene.SetTerroristAttack(false);
            }, Config.PLAYER_UNIQUE_SKILL_TIME);
        }

        public void SeaFoodBless()
        {
            var sprite = GetComponent<SpriteRenderer>();
            Sprite currentSprite = sprite.sprite;
            sprite.sprite = _protectBySeafoodImage;
            _protectedBySeaFood = true;
            var audioInterval = SetInterval(() =>
            {
                _audio.PlayOneShot(_protectBySeafoodClip);
            }, 175);
            SetTimeout(() =>
            {
                sprite.sprite = currentSprite;
                _protectedBySeaFood = false;
                ClearInterval(audioInterval);
            }, Config.PLAYER_UNIQUE_SKILL_TIME);
        }

        public override void Damage(int damage)
        {
            if (IsGodMode || _protectedBySeaFood) damage = 0;

            _hp -= damage;

            if (_hp < 0) _hp = 0;

            _scene.NumberCollection.ShowNumber(
                this.gameObject,
                _playerID == 0 ? NumberCollection.Type.Violet : NumberCollection.Type.Red,
                (uint)damage
            );

            if (_hp == 0) Die();

            OnHPChanged?.Invoke(damage);
        }

        public override void Recovery(int recover)
        {
            _hp += recover;

            if (_hp > MAX_HP) _hp = MAX_HP;

            OnHPChanged?.Invoke(recover);
        }

        public void Die()
        {
            if (_died) return;

            _died = true;
            OnDied?.Invoke();
        }

        public HouseInfo BuyHouse(Point p, bool free = false)
        {
            var houseGen = _scene.HouseGenerator;

            if (free || CostMoney(Config.HOUSE_PRICE))
            {
                var houseInfo = houseGen.AddHouse(p.Row, p.Column, this._playerID);
                _builtHouseCount++;
                houseInfo.OnHouseDestroy += this.OnHouseDestroyed;
                OnHouseCreated?.Invoke(houseInfo);
                return houseInfo;
            }
            else
            {
                return null;
            }
        }

        private void OnHouseDestroyed(Point p)
        {
            _houseDestroyedCount++;
        }

        public void OnMonsterKilled(int monsterID)
        {
            _beenKilledMonsterCount[monsterID]++;
            var originalBouns = this.Manager.MonsterInfoCollection[monsterID].Bonus;
            int offset = (int)(originalBouns * .2f);
            var bouns = Random.Range(originalBouns - offset, originalBouns + offset);
            AddMoney(bouns);
        }

        public HouseInfo SetHouseMonster(Point p, int cardSelectIndex)
        {
            var houseGen = _scene.HouseGenerator;
            var monsterID = Info.GetCardIds()[cardSelectIndex];
            var monsterInfo = this.Manager.MonsterInfoCollection[monsterID];

            if (CostMoney(monsterInfo.Cost))
            {
                return houseGen.SetHouseMonster(p.Row, p.Column, monsterID);
            }
            else
            {
                return null;
            }
        }

        public HouseInfo UpgradeHouse(Point p, HouseInfo.UpgradeType type)
        {
            var scene = GetSceneLogic<FightSceneLogic>();
            var house = scene.HouseGenerator[p.Row, p.Column];
            var price = CalcUpgradePrice(type, house.GetUpgradeCount(type));

            if (CostMoney(price))
            {
                scene.HouseGenerator.UpgradeHouse(type, p.Row, p.Column);
                return house;
            }
            else
            {
                return null;
            }
        }

        public bool IsGodMode
        {
            get
            {
                return _godMode;
            }
            set
            {
                _godMode = value;
            }
        }

        public void InitHouses()
        {
            Point[,] defaultPoint =
            {
                { new Point(1, 4), new Point(4, 4), new Point(7, 4), new Point(1, 7), new Point(4, 7), new Point(7, 7), },
                { new Point(1, 12), new Point(4, 12), new Point(7, 12), new Point(1, 15), new Point(4, 15), new Point(7, 15), },
            };

            for (int i = 0; i < INIT_HOUSES; i++)
            {
                BuyHouse(defaultPoint[_playerID, i], true);
            }
        }

        public HouseInfo DiscardHouseMonster(Point p)
        {
            var houseGen = _scene.HouseGenerator;

            if (CostMoney(Config.DISCARD_MONSTER_PUNISH))
            {
                return houseGen.DiscardMonster(p.Row, p.Column);
            }
            else
            {
                return null;
            }
        }


        public void ResetCounter()
        {
            _beenKilledMonsterCount = new Dictionary<int, int>();
            _houseDestroyedCount = 0;
            _builtHouseCount = 0;

            foreach (var m in this.Manager.MonsterInfoCollection.GetAllMonsterId())
            {
                _beenKilledMonsterCount.Add(m, 0);
            }
        }

        public int CalcUpgradePrice(HouseInfo.UpgradeType type, int currentCount)
        {
            var basis = 0;

            switch (type)
            {
                case HouseInfo.UpgradeType.Attack:
                    basis = Config.UPGRADE_ATTACK_BASIS;
                    break;

                case HouseInfo.UpgradeType.HP:
                    basis = Config.UPGRADE_HP_BASIS;
                    break;

                case HouseInfo.UpgradeType.Speed:
                    basis = Config.UPGRADE_SPEED_BASIS;
                    break;
            }

            return basis * (currentCount + 1);
        }

        public IReadOnlyDictionary<int, int> BeenKilledByRivalMonsterCount => _beenKilledMonsterCount;

        public IReadOnlyCollection<Honors.Honor> Honors => _honors;

        private IReadOnlyDictionary<int, int> _rivalMonsterKilled;
        public IReadOnlyDictionary<int, int> MyKilledMonsterInfo
        {
            get
            {
                if (_rivalMonsterKilled == null)
                {
                    _rivalMonsterKilled = _scene.GetPlayerAt(1 - _playerID)._beenKilledMonsterCount;
                }

                return _rivalMonsterKilled;
            }
        }
    }
}
