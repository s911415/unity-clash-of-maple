using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Component
{
    [Serializable]
    public struct Size
    {
        [SerializeField]
        private int _width, _height;
        public Size(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public int Width => _width;
        public int Height => _height;
    }
}
