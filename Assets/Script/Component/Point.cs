using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Component
{
    [Serializable]
    public struct Point
    {
        [SerializeField]
        private int _row, _col;
        public Point(int row, int col)
        {
            _row = row;
            _col = col;
        }

        public int Row => _row;
        public int Column => _col;
    }
}
