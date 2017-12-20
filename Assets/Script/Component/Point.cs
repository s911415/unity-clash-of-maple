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

        public static float Distance(Point p1, Point p2)
        {
            return Mathf.Sqrt(Mathf.Pow(p1._row - p2._row, 2) + Mathf.Pow(p1._col - p2._col, 2));
        }

        public float Distance(Point p)
        {
            return Distance(this, p);
        }

        public override string ToString()
        {
            return $"({_row}, {_col})";
        }

        public int Row => _row;
        public int Column => _col;
    }
}
