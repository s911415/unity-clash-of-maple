using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Component.Map
{
    public class MapGridGenerator : CommonObject
    {
        private const int EMPTY = -1;
        public GameObject gridObject;
        public int col, row;
        public int curCol, curRow;
        public float y;
        private MapGrid[,] _mapGridArray;
        public HighlightsFX _highlightFX;

        private void Start()
        {
            _mapGridArray = new MapGrid[row, col];
            int step = 0;
            int halfStep = 0;
            DeleteChild();
            curCol = curRow = EMPTY;

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
                    mapGrid._generator = this;
                    int posX = c * step + halfStep, posY = r * step + halfStep;
                    grid.transform.localPosition = new Vector3(posX, y, posY);
                }
            }
        }

        public void SetHighLight(int r, int c)
        {
            this.curCol = c;
            this.curRow = r;
            _highlightFX.ClearOutlineData();

            if (r == EMPTY || c == EMPTY) return;

            Renderer renderer = this[r, c].gameObject.GetComponent<Renderer>();
            _highlightFX.SetRenderer(renderer);
        }

        public void ShowInfoOnPanel()
        {
            GameObject.FindGameObjectWithTag("MapStatus").GetComponent<MapStatusPanel>().DisplayInfo(_mapGridArray[curRow, curCol]);
        }

        public void Building()
        {
            Debug.Log(string.Format("Click Grid: ({0}, {1})", curRow, curCol));
            _mapGridArray[curRow, curCol].Type += 1;
            GameObject.FindGameObjectWithTag("MapStatus").GetComponent<MapStatusPanel>().CloseAllPanel();
        }

        private void DeleteChild()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        public MapGrid this[int r, int c] => _mapGridArray[r, c];
    }
}
