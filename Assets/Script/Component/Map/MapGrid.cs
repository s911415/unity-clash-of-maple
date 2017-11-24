using NTUT.CSIE.GameDev.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NTUT.CSIE.GameDev.Component.Map
{
    public class MapGrid : CommonObject
    {
        public int row, col;
        public int width = 10;
        public List<Sprite> buildingImage = new List<Sprite>();
        public MapGridGenerator _generator;
        private HouseInfo _houseInfo;
        /*
        0 空地 可選擇建造
        1 房屋 可選擇兵種
        2 生產 可選擇升級
        */
        private int type;

        protected void Start()
        {
            _houseInfo = new HouseInfo();
        }

        protected void OnMouseDown()
        {
            if (IsMouseOnGUI) return;
           // Debug.Log(string.Format("Click Grid: ({0}, {1})", row, col));
            _generator.SetHighLight(this.row, this.col);
            _generator.ShowInfoOnPanel(_houseInfo);
        }

        public int Type

        {
            set
            {
                type = value;
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = buildingImage[type];
                _houseInfo.SetHouseInfo(type);
            }
             get
            {
                return type;
            }

        }

        public void SelectThisMonster(string Number)
        {
            _houseInfo.monsterNumber = Number;
        }

        private void Update()
        {
            
        }
    }
}
