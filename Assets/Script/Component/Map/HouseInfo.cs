using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;

namespace NTUT.CSIE.GameDev.Component.Map
{
    public class HouseInfo : CommonObject
    {
        // 房屋基本資訊
        public int type;
        public int hp;
        public int maxHp;
        public string name;
        // 出產怪物資訊
        private string _monsterNum;
        private int _monsterAttack = 1;
        private int _monsterHp = 1;
        private int _monsterSpeed = 1;

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
            _monsterAttack =  this.Manager.MonsterInfoCollection[int.Parse(_monsterNum)].Attack;
            _monsterHp = this.Manager.MonsterInfoCollection[int.Parse(_monsterNum)].MaxHP;
            _monsterSpeed = 1;

        }

        public void AttackHouse(int attack)
        {
            hp -= attack;
        }

        public void ResetMonster()
        {
            _monsterNum = null;
            _monsterAttack = 1;
            _monsterHp = 1;
            _monsterSpeed = 1;
        }

        public void Upgrade(string item)
        {
            switch (item)
            {
                case "attack":
                    _monsterAttack *= 2;
                    break;
                case "hp":
                    _monsterHp *= 2;
                    break;
                case "speed":
                    _monsterSpeed *= 2;
                    break;
                default:
                    break;
            }
        }

        public int GetMonsterAbility(string name)
        {
            switch (name)
            {
                case "attack":
                    return _monsterAttack;
                case "hp":
                    return _monsterHp;
                case "speed":
                    return _monsterSpeed;
                default:
                    return 0;
            }
        }
    }
}