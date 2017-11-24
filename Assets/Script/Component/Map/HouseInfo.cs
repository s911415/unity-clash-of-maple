using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NTUT.CSIE.GameDev.Game;

namespace NTUT.CSIE.GameDev.Component.Map
{
    public class HouseInfo
    {
        // 房屋基本資訊
        public int type;
        public int hp;
        public int maxHp;
        public string name;
        // 出產怪物資訊
        private string _monsterNum;
        private int _monsterAttack;
        private int _monsterHp;
        private int _monsterSpeed;

        public HouseInfo()
        {
            hp = maxHp = 0;
            name = "空地";
        }

        public void SetHouseInfo(int type)
        {
            this.type = type;
            if (type == 1)
            {
                hp = maxHp = 0;
                name = "空地";
            }
            else if (type == 2)
            {
                hp = maxHp = 5000;
                name = "建築";
            }
        }

        public string monsterNumber
        {
            set
            {
                _monsterNum = value;
                this.SetMonsterAbility(value);
            }
            get
            {
                return _monsterNum;
            }
        }

        private void SetMonsterAbility(string num)
        {
            // read monster info
        }
    }
}