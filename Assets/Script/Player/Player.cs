using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Component.Numbers;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Player.Honors;
using NTUT.CSIE.GameDev.Scene;
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
        // public delegate void MonsterKilledEventHandler(int monsterID, IReadOnlyDictionary<int, int> monsterKilledSummary);
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

        private float _lastAttackTime;

        public event ValueChangedEventHandler<int> OnHPChanged;
        public event ValueChangedEventHandler<int> OnMoneyChanged;
        public event ValueChangedEventHandler<Honor> OnHonorAdded;
        public event ModelChangedEventHandler OnHonorsChanged;
        public event ModelChangedEventHandler OnDied;
        public event HouseCreatedEventHandler OnHouseCreated;
        public event ValueChangedEventHandler<int> OnItemCollected;
        private Dictionary<int, int> _itemCollectedCount;

        [SerializeField]
        private Dictionary<int, int> _killedMonsterCount;
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
            _lastAttackTime = 0;
            Attached();
        }

        public void Attached()
        {
            //Call event
            OnHPChanged?.Invoke(_hp);
            OnMoneyChanged?.Invoke(_money);
            OnHonorsChanged?.Invoke();
        }

        /*
        public void KilledMonster(int monsterID)
        {
            OnKilledMonster?.Invoke(monsterID, this.MyKilledMonsterInfo);
        }
        */

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
            if (m > Config.PLAYER_MAX_MONEY) m = Config.PLAYER_MAX_MONEY;

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
            if (!_honors.Contains(h))
            {
                this._honors.Add(h);
                OnHonorAdded?.Invoke(h);
            }

            OnHonorsChanged?.Invoke();
            return this;
        }

        public void OnCollectItem(int itemID)
        {
            if (!_itemCollectedCount.ContainsKey(itemID))
                _itemCollectedCount.Add(itemID, 1);
            else
                _itemCollectedCount[itemID]++;

            OnItemCollected?.Invoke(itemID);
            CheckItems();
        }

        protected void FixedUpdate()
        {
            float now = Time.time;

            if (this.Alive && now  - _lastAttackTime > Config.PLAYER_ATTACK_INTERVAL)
            {
                var mobCount = GetNearMobCount();

                if (mobCount > 0)
                {
                    _scene.SkillGenerator.UseSkill(this.transform.localPosition, 0, _playerID);
                    _lastAttackTime = now;
                }
            }
        }

        protected int GetNearMobCount()
        {
            return _scene
                   .GetAllMonsterInfo()
                   .Where(m => m.PlayerID != _playerID && InRange(m.transform.localPosition, 25)).Count();
        }

        public void DoUniqueSkill(UniqueSkill type)
        {
            if (_uniqueSkillUsed)
                throw new System.Exception("無法使用兩次大絕");

            if ((float)_hp / MAX_HP >= Config.PLAYER_UNIQUE_REQUIRE_HP/* || CurrentBuildingCount > 0*/)
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

            System.Action killAll = () =>
            {
                var rivalMonster = _scene.GetAllMonsterInfo().Where(h => h.PlayerID != _playerID).ToArray();

                foreach (var m in rivalMonster)
                    m.Die();
            };
            SetTimeout(killAll, 3500);
            SetTimeout(() =>
            {
                foreach (var h in rivalHouseArray)
                {
                    if (h)
                    {
                        h.Damage(h.HP - 10);
                    }
                }
            }, 3500);
            killAll();
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
            uint audioInterval = 0;
            audioInterval = SetInterval(() =>
            {
                if (!_audio)
                {
                    ClearInterval(audioInterval);
                }
                else
                {
                    _audio.PlayOneShot(_protectBySeafoodClip);
                }
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

        /// <summary>   當對手的怪物被殺掉時 </summary>
        ///
        /// <param name="monsterID">    Identifier for the monster. </param>
        public void OnRivalMonsterKilled(int monsterID)
        {
            _killedMonsterCount[monsterID]++;
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

        protected void CheckItems()
        {
            System.Func<int, int> GetNumber = (itemID) =>
            {
                if (!_itemCollectedCount.ContainsKey(itemID))
                    _itemCollectedCount.Add(itemID, 0);

                return _itemCollectedCount[itemID];
            };

            if (GetNumber(1) > 20)
                AddHonor(Honor.松果大帝);

            if (GetNumber(2) > 10)
                AddHonor(Honor.衛生股長);

            if (GetNumber(3) > 30)
                AddHonor(Honor.燈泡大師);

            if (GetNumber(5) > 50)
                AddHonor(Honor.除錯大師);
        }


        public void ResetCounter()
        {
            _killedMonsterCount = new Dictionary<int, int>();
            _itemCollectedCount = new Dictionary<int, int>();
            _houseDestroyedCount = 0;
            _builtHouseCount = 0;

            foreach (var m in this.Manager.MonsterInfoCollection.GetAllMonsterId())
            {
                _killedMonsterCount.Add(m, 0);
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

        public int CurrentBuildingCount
        {
            get
            {
                return _scene.HouseGenerator
                       .GetAllHouseInfo()
                       .Where(h => h.PlayerID == _playerID)
                       .Select(h => h.Type)
                       .Where(t => t != HouseInfo.HouseType.Master)
                       .Count();
            }
        }

        public bool IsUniqueSkillUsed => _uniqueSkillUsed;

        public IReadOnlyCollection<Honors.Honor> Honors => _honors;

        public IReadOnlyDictionary<int, int> MyKilledMonsterInfo => _killedMonsterCount;
    }
}
