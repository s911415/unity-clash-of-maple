using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.UI;
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
        public int type;
        /*
        0 空地 可選擇建造
        1 房屋 可選擇兵種
        2 生產 可選擇升級
        */
        public MapGridGenerator _generator;

        protected void Start()
        {
            type = 0;
        }

        protected void OnMouseDown()
        {
            if (IsMouseOnGUI) return;
            GameObject.FindGameObjectWithTag("MapStatus").GetComponent<MapStatusPanel>().DisplayInfo(type);
            Debug.Log(string.Format("Click Grid: ({0}, {1})", row, col));
            _generator.SetHighLight(this.row, this.col);
        }
    }
}
