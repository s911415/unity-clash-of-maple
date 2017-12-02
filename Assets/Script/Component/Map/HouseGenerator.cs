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
        private GameObject[,] _houseArray; // 對應mapgrid
        int houseCount = 0;
        public void AddHouse()
        {
            HouseInfo currentHouseInfo;
            GameObject house = (GameObject)Instantiate(_housePrefab);
            house.GetComponent<HouseInfo>().houseId = houseCount++;
            house.GetComponent<HouseInfo>().Type = 1;
            house.GetComponent<HouseInfo>().SetPosition(_mapGridGenerator.curRow, _mapGridGenerator.curCol);
            _houseArray[_mapGridGenerator.curRow, _mapGridGenerator.curCol] = house;
            _houseArray[_mapGridGenerator.curRow, _mapGridGenerator.curCol].transform.SetParent(_houseList.transform);
            currentHouseInfo = _houseArray[_mapGridGenerator.curRow, _mapGridGenerator.curCol].GetComponent<HouseInfo>();
            GameObject.FindGameObjectWithTag("MapStatus").GetComponent<MapStatusPanel>().DisplayInfo(currentHouseInfo);
        }

        public void ClickCardButton(int n)
        {
            HouseInfo currentHouseInfo = _houseArray[_mapGridGenerator.curRow, _mapGridGenerator.curCol].GetComponent<HouseInfo>();
            string monsterNum = Manager.GetPlayerAt(Manager.DEFAULT_PLAYER_ID).GetCardIds()[n];
            currentHouseInfo.Type += 1;
            currentHouseInfo.MonsterNumber = monsterNum;
            GameObject.FindGameObjectWithTag("MapStatus").GetComponent<MapStatusPanel>().CloseAllPanel();
            GameObject.FindGameObjectWithTag("MapStatus").GetComponent<MapStatusPanel>().DisplayInfo(currentHouseInfo);
        }

        public void DiscardMonster()
        {
            HouseInfo currentHouseInfo = _houseArray[_mapGridGenerator.curRow, _mapGridGenerator.curCol].GetComponent<HouseInfo>();
            currentHouseInfo.ResetMonster();
            GameObject.FindGameObjectWithTag("MapStatus").GetComponent<MapStatusPanel>().DisplayInfo(currentHouseInfo);
        }

        // Use this for initialization
        void Start()
        {
            _houseArray = new GameObject[_mapGridGenerator.row, _mapGridGenerator.col];
            for (int i = 0; i < _mapGridGenerator.col; i++)
                for (int j = 0; j < _mapGridGenerator.row; j++)
                {
                    _houseArray[j, i] = _housePrefab;
                }
            houseCount = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public GameObject this[int r, int c] => _houseArray[r, c];
    }
}