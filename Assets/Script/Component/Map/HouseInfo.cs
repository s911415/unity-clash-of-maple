using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NTUT.CSIE.GameDev.Game;

namespace NTUT.CSIE.GameDev.Component.Map
{
    public class HouseInfo : CommonObject, IHurtable
    {
        // 房屋基本資訊
        public int type;
        public int hp;
        public int maxHp;
        public string houseName;
        // 出產怪物資訊
        private string _monsterNum;
        private int _extraAttack;
        private int _extraHp;
        private int _extraSpeed;

        public HouseInfo()
        {
            hp = maxHp = 0;
            houseName = "空地";
        }

        public void SetHouseInfo(int type)
        {
            this.type = type;

            if (type == 1)
            {
                hp = maxHp = 0;
                houseName = "空地";
            }
            else if (type == 2)
            {
                hp = maxHp = MAX_HP;
                houseName = "建築";
            }
        }

        public string MonsterNumber
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

        public Monster.Info MonsterInfo
        {
            get
            {
                if (_monsterNum == null)
                    return null;

                return this.Manager.MonsterInfoCollection[_monsterNum];
            }
        }

        private void SetMonsterAbility(string num)
        {
            // read monster info
        }

        public void Damage(int attack)
        {
            hp -= attack;
        }

        public void Recovery(int recovery)
        {
            hp += recovery;
        }

        public void ResetMonster()
        {
            type--;
            _monsterNum = null;
            _extraAttack = 0;
            _extraHp = 0;
            _extraSpeed = 0;
        }

        public int RealAttack => MonsterInfo == null ? 0 : (MonsterInfo.Attack + _extraAttack);
        public int RealHP => MonsterInfo == null ? 0 : (MonsterInfo.MaxHP + _extraHp);
        public int RealSpeed => MonsterInfo == null ? 0 : (MonsterInfo.Speed + _extraSpeed);

        public int MAX_HP => 5000;
    }
}