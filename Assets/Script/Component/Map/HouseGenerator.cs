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
        private ulong houseCount = 0;

        public HouseInfo AddHouse(int row, int col, int playerID)
        {
            GameObject house = Instantiate(_housePrefab, _houseList.transform);
            var houseInfo = house.GetComponent<HouseInfo>();
            houseInfo
            .SetId(houseCount++)
            .SetType(HouseInfo.HouseType.Building)
            .SetPosition(row, col)
            .SetPlayerID(playerID)
            .SetDirection((playerID == Manager.DEFAULT_PLAYER_ID) ? Direction.Right : Direction.Left);

            if (_houseArray[row, col] != null)
            {
                throw new System.Exception("Grid is occupy");
            }
            else
            {
                _houseArray[row, col] = houseInfo;
                return houseInfo;
            }
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

        // Use this for initialization
        void Start()
        {
            _houseArray = new HouseInfo[_mapGridGenerator.row, _mapGridGenerator.col];
            houseCount = 0;
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