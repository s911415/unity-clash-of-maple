using NTUT.CSIE.GameDev.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Component.Map
{
    public class MapGridGenerator : CommonObject
    {
        public GameObject gridObject;
        public int col, row;
        public float y;
        private MapGrid[,] _mapGridArray;

        private void Start()
        {
            _mapGridArray = new MapGrid[row, col];
            int step = 0;
            int halfStep = 0;

            for (int c = 0; c < col; c++)
            {
                for (int r = 0; r < row; r ++)
                {
                    var grid = Object.Instantiate(gridObject, this.gameObject.transform);
                    var mapGrid = grid.GetComponent<MapGrid>();
                    _mapGridArray[r, c] = mapGrid;

                    if (step == 0)
                    {
                        step = mapGrid.width;
                        halfStep = step >> 1;
                    }

                    mapGrid.col = c;
                    mapGrid.row = r;
                    int posX = c * step + halfStep, posY = r * step + halfStep;
                    grid.transform.localPosition = new Vector3(posX, y, posY);
                }
            }
        }

        public MapGrid this[int r, int c] => _mapGridArray[r, c];
    }
}
