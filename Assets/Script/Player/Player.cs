using NTUT.CSIE.GameDev.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Player
{
    public class Player : CommonObject
    {
        public const int MAX_HP = 50000;
        [SerializeField]
        protected int _playerID;

        [SerializeField]
        protected Info _info;

        [SerializeField]
        protected int _hp, _money;

        [SerializeField]
        protected List<Tower> _towers;

        [SerializeField]
        protected HashSet<Honors.Honor> _honors;

        protected void Start()
        {
            _info = Manager.GetPlayerAt(_playerID);
            _towers = new List<Tower>();
            _honors = new HashSet<Honors.Honor>();
            _hp = MAX_HP;
        }

        public int HP => _hp;
        public int Money => _money;

        public Player SetMoney(int m)
        {
            _money = m;
            return this;
        }

        public Player AddMoney(int m)
        {
            _money += m;
            return this;
        }

        public bool CostMoney(int m)
        {
            if (_money - m < 0)
                return false;

            _money -= m;
            return true;
        }

        public Player AddHonor(Honors.Honor h)
        {
            this._honors.Add(h);
            return this;
        }
    }
}
