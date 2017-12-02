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
        private int type;
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
        private Size _size;

        public HouseInfo()
        {
            hp = maxHp = 0;
            houseName = "空地";
        }

        public int Type
        {
            set
            {
                type = value;

                if (type == 0)
                {
                    hp = maxHp = 0;
                    houseName = "空地";
                }
                else if (type == 1)
                {
                    hp = maxHp = MAX_HP;
                    houseName = "建築";
                }
                else if (type == 2)
                {
                    _lastSpawnTime = Time.time;
                }

                _houseRenderer.sprite = houseImage[type];
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

        private void SetMonsterAbility(string num)
        {
            // read monster info
        }

        private void Update()
        {
            RemainingNextSpawnTime = -1;

            if (type == 2)
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
            Type--;
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