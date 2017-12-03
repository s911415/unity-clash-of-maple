using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NTUT.CSIE.GameDev.Game;
using UnityEngine.Serialization;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.UI;

namespace NTUT.CSIE.GameDev.Component.Map
{
    public class HouseInfo : CommonObject, IHurtable
    {
        public int scale = 1;
        public int houseId;
        [SerializeField]
        private SpriteRenderer _houseRenderer;
        // 房屋基本資訊
        [SerializeField]
        private HouseType type;
        public int hp;
        public int maxHp;
        public string houseName;
        public Sprite[] houseImage = new Sprite[3];
        // 出產怪物資訊
        private string _monsterNum;
        private int _extraAttack;
        private int _extraHp;
        private int _extraSpeed;
        [FormerlySerializedAs("Remaining Next Spawn Time (Readonly)")]
        public int RemainingNextSpawnTime;
        private float _lastSpawnTime = 0f;
        [SerializeField]
        private Point _position;
        private int _playerID;
        private int _upgAttackCnt, _upgHpCnt, _upgSpeedCnt;

        public enum HouseType
        {
            Empty, Building, Summon
        }
        public enum UpgradeType
        {
            Attack, HP, Speed
        }

        public HouseInfo()
        {
            hp = maxHp = 0;
            houseName = "空地";
            _extraAttack = _extraHp = _extraSpeed = 0;
            _upgAttackCnt = _upgHpCnt = _upgSpeedCnt = 0;
        }

        public HouseType Type
        {
            set
            {
                type = value;

                if (type == HouseType.Empty)
                {
                    hp = maxHp = 0;
                    houseName = "空地";
                }
                else if (type == HouseType.Building)
                {
                    hp = maxHp = MAX_HP;
                    houseName = "建築";
                }
                else if (type == HouseType.Summon)
                {
                    _lastSpawnTime = Time.time;
                }

                _houseRenderer.sprite = houseImage[(int)type];
            }

            get
            {
                return type;
            }
        }

        public void SetPosition(int row, int col)
        {
            this._position = new Point(row, col);
            var gen = GetSceneLogic<FightSceneLogic>().MapGridGenerator;
            gen.SetHighLight(row, col);
            var pos = Clone(gen[row, col].gameObject.transform.localPosition);
            pos.y = 0;
            gameObject.transform.localPosition = pos;
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

        private void SetMonsterAbility(string num)
        {
            // read monster info
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
            Debug.Log(string.Format("召喚: {0}", MonsterInfo.Name));
            var playerID = Manager.DEFAULT_PLAYER_ID;
            GetSceneLogic<FightSceneLogic>().SpawnMonster(MonsterInfo.ID, playerID, this);
        }


        protected void OnMouseDown()
        {
            // 空地時 顯示hightlight的工作 是給格線MapGrid執行的 之後選卡跟升級給HouseInfo來執行
            if (Type != 0)
            {
                GetSceneLogic<FightSceneLogic>().MapGridGenerator.SetHighLight(_position);
                GameObject.FindGameObjectWithTag("MapStatus").GetComponent<MapStatusPanel>().DisplayInfo(this);
            }
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
            Type = HouseInfo.HouseType.Building;
            _monsterNum = null;
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

        public int MAX_HP => 5000;

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
    }
}