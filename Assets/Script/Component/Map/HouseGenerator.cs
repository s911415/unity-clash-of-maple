using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.UI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NTUT.CSIE.GameDev.Component.Map
{
    public class HouseGenerator : CommonObject
    {
        [SerializeField]
        private GameObject _housePrefab;
        [SerializeField]
        private GameObject _houseList;
        [SerializeField]
        private MapGridGenerator _mapGridGenerator;
        private HouseInfo[,] _houseArray; // 對應mapgrid
        private ulong _houseSeq = 0;

        protected override void Awake()
        {
            base.Awake();
            _houseArray = new HouseInfo[_mapGridGenerator.row, _mapGridGenerator.col];
            _houseSeq = 0;
        }

        // Use this for initialization
        protected void Start()
        {
            for (var i = 0; i < 2; i++)
                CreateMasterHouse(i);
        }

        public HouseInfo AddHouse(int row, int col, int playerID)
        {
            if (_houseArray[row, col] != null)
            {
                throw new System.Exception(string.Format("Grid ({0}, {1}) is occupy", row, col));
            }

            GameObject house = Instantiate(_housePrefab, _houseList.transform);
            var houseInfo = house.GetComponent<HouseInfo>();
            houseInfo
            .SetId(_houseSeq++)
            .SetType(HouseInfo.HouseType.Building)
            .SetPosition(row, col)
            .SetPlayerID(playerID)
            .SetDirection((playerID == Manager.DEFAULT_PLAYER_ID) ? Direction.Right : Direction.Left);
            _houseArray[row, col] = houseInfo;
            return houseInfo;
        }

        public HouseInfo SetHouseMonster(int row, int col, string monsterID)
        {
            HouseInfo currentHouseInfo = _houseArray[row, col];
            currentHouseInfo.Type = HouseInfo.HouseType.Summon;
            currentHouseInfo.MonsterNumber = monsterID;
            return currentHouseInfo;
        }

        public HouseInfo DiscardMonster(int row, int col)
        {
            HouseInfo currentHouseInfo = _houseArray[row, col];
            currentHouseInfo.ResetMonster();
            return currentHouseInfo;
        }

        public HouseInfo UpgradeHouse(HouseInfo.UpgradeType type, int row, int col)
        {
            HouseInfo hInfo = _houseArray[row, col];

            switch (type)
            {
                case HouseInfo.UpgradeType.Attack:
                    hInfo.UpgradeAttack();
                    break;

                case HouseInfo.UpgradeType.HP:
                    hInfo.UpgradeHP();
                    break;

                case HouseInfo.UpgradeType.Speed:
                    hInfo.UpgradeSpeed();
                    break;
            }

            return hInfo;
        }

        public void DestroyHouse(Point p)
        {
            var house = this[p];

            if (house != null)
            {
                _houseArray[p.Row, p.Column] = null;
                Destroy(house.gameObject);
            }
            else
            {
                throw new System.ArgumentException("Cannot find house to destroy");
            }
        }

        protected void CreateMasterHouse(int playerID)
        {
            const int CENTER_X = 4;
            var points = new Point[][]
            {
                new Point[]{new Point(CENTER_X, 0), new Point(CENTER_X, 1)},
                new Point[]{new Point(CENTER_X, 18), new Point(CENTER_X, 19)}
            };

            foreach (var p in points[playerID])
            {
                var house = AddHouse(p.Row, p.Column, playerID)
                            .SetType(HouseInfo.HouseType.Master);
                house.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        public HouseInfo[] GetAllHouseInfo()
        {
            return this.gameObject.GetComponentsInChildren<HouseInfo>();
        }

        public HouseInfo this[Point p] => this[p.Row, p.Column];

        public HouseInfo this[int r, int c] => _houseArray[r, c];
    }
}