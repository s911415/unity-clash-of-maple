using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Player
{
    public class Player : CommonObject, IHurtable
    {
        public delegate void ValueChangedEventHandler<T>(T value);
        public const int MAX_HP = 50000;
        public const int MAX_MONEY = 2000000000;
        [SerializeField]
        protected int _playerID;

        [SerializeField]
        protected Info _info;

        [SerializeField, Range(0, MAX_HP)]
        protected int _hp;

        [SerializeField, Range(0, MAX_MONEY)]
        protected int _money;

        [SerializeField]
        protected List<Tower> _towers;

        [SerializeField]
        protected HashSet<Honors.Honor> _honors;

        public event ValueChangedEventHandler<int> OnHPChanged;
        public event ValueChangedEventHandler<int> OnMoneyChanged;
        public event ValueChangedEventHandler<ICollection<Tower>> OnTowersChanged;
        public event ValueChangedEventHandler<ICollection<Honors.Honor>> OnHonorsChanged;
        protected override void Awake()
        {
            base.Awake();
            _towers = new List<Tower>();
            _honors = new HashSet<Honors.Honor>();
            _hp = MAX_HP;
            _info = Manager.GetPlayerAt(_playerID);
        }
        protected void Start()
        {
        }
        public void ShowHpChangedNumber(int damage)
        {
        }

        public void Attached()
        {
            //Call event
            OnHPChanged?.Invoke(_hp);
            OnMoneyChanged?.Invoke(_money);
            OnTowersChanged?.Invoke(_towers);
            OnHonorsChanged?.Invoke(_honors);
        }

        public Info Info
        {
            get
            {
                if (_info == null) _info = Manager.GetPlayerAt(_playerID);

                return _info;
            }
        }

        public int HP => _hp;
        public int Money => _money;

        int IHurtable.MAX_HP => MAX_HP;

        public Player SetMoney(int m)
        {
            _money = m;
            OnMoneyChanged?.Invoke(_money);
            return this;
        }

        public Player AddMoney(int m)
        {
            _money += m;

            if (_money > MAX_MONEY) _money = MAX_MONEY;

            OnMoneyChanged?.Invoke(_money);
            return this;
        }

        public bool CostMoney(int m)
        {
            if (_money - m < 0)
                return false;

            _money -= m;
            OnMoneyChanged?.Invoke(_money);
            return true;
        }

        public Player AddHonor(Honors.Honor h)
        {
            this._honors.Add(h);
            OnHonorsChanged?.Invoke(_honors);
            return this;
        }

        public void Damage(int damage)
        {
            _hp -= damage;

            if (_hp < 0) _hp = 0;

            OnHPChanged?.Invoke(_hp);
        }

        public void Recovery(int recover)
        {
            _hp += recover;

            if (_hp > MAX_HP) _hp = MAX_HP;

            OnHPChanged?.Invoke(_hp);
        }

        public HouseInfo BuyHouse(Point p)
        {
            var scene = GetSceneLogic<FightSceneLogic>();
            var houseGen = scene.HouseGenerator;

            if (CostMoney(Config.HOUSE_PRICE))
            {
                var houseInfo = houseGen.AddHouse(p.Row, p.Column, this._playerID);
                return houseInfo;
            }
            else
            {
                return null;
            }
        }

        public HouseInfo SetHouseMonster(Point p, int cardIndex)
        {
            var scene = GetSceneLogic<FightSceneLogic>();
            var houseGen = scene.HouseGenerator;
            var monsterID = Info.GetCardIds()[cardIndex];
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

        public HouseInfo DiscardHouseMonster(Point p)
        {
            var scene = GetSceneLogic<FightSceneLogic>();
            var houseGen = scene.HouseGenerator;

            if (CostMoney(Config.DISCARD_MONSTER_PUNISH))
            {
                return houseGen.DiscardMonster(p.Row, p.Column);
            }
            else
            {
                return null;
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
    }
}
