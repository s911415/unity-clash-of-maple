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
        /*
        0 空地 可選擇建造
        1 房屋 可選擇兵種
        2 生產 可選擇升級
        */
        private int type;
        /*
        房屋資訊
        */
        public int hp;
        public int maxHp;
        public string gridName;
        public MapGridGenerator _generator;
        /*
        兵種編號
        素質
        */
        public string monsterNum;
        public int monsterAttack;
        public int monsterHp;
        public int monsterSpeed;

        protected void Start()
        {
            this.SetGridInitial();
        }

        private void SetGridInitial()
        {
            type = 0;
            hp = maxHp = 0;
            gridName = "空地";
        }

        protected void OnMouseDown()
        {
            if (IsMouseOnGUI) return;
           // Debug.Log(string.Format("Click Grid: ({0}, {1})", row, col));
            _generator.SetHighLight(this.row, this.col);
            _generator.ShowInfoOnPanel();
        }

        public int Type

        {
            set
            {
                type = value;
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = buildingImage[type];
                if (type != 0)
                {
                    hp = maxHp = 5000;
                    gridName = "建築1";
                }
            }
             get
            {
                return type;
            }

        }
        private void Update()
        {
            
        }
    }
}
