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
        public MapGridGenerator _generator;

        protected void Start()
        {
        }

        protected void OnMouseDown()
        {
            if (IsMouseOnGUI) return;

            Debug.Log(string.Format("Click Grid: ({0}, {1})", row, col));
            _generator.SetHighLight(this.row, this.col);
        }
    }
}
