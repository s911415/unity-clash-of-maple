using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.CardSelect;
using NTUT.CSIE.GameDev.Component.Map;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NTUT.CSIE.GameDev.Component;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NTUT.CSIE.GameDev.UI
{
    public class MiniMap : CommonObject
    {
        [SerializeField]
        public MapGridGenerator _generator;
        [SerializeField]
        public GameObject _miniMap;
        [SerializeField]
        public GameObject _bigWindow;
        const int GRIDSIZE = 10;
        private void Update()
        {
            float w = Camera.main.pixelWidth;
            float h = Camera.main.pixelHeight;
            if(Input.GetMouseButtonDown(0)&&IsMouseOnGUI)
            {
                float hPercent = _miniMap.GetComponent<RectTransform>().rect.height / _bigWindow.GetComponent<RectTransform>().rect.height;
                float wPercent = _miniMap.GetComponent<RectTransform>().rect.width / _bigWindow.GetComponent<RectTransform>().rect.width;
                Vector3 clickPos = new Vector3();
                Vector3 computePos = new Vector3();
                clickPos.x = Input.mousePosition.x;
                clickPos.y = h-Input.mousePosition.y;
                clickPos.z = Input.mousePosition.z;
              
                computePos.x = ((clickPos.x / w) - (1 - wPercent)) / wPercent;
                computePos.y = ((clickPos.y / h) - (1 - hPercent)) / hPercent;
                computePos.z = clickPos.z;

                
                ScreenMove(computePos.x, computePos.y);
            }
        }
        protected void ScreenMove(float destinationX,float destinationY)
        {
            if (destinationX >= 0 && destinationX <= 1 && destinationY >= 0 && destinationY <= 1)
            {
                Vector3 cameraBase = new Vector3(_generator.col * GRIDSIZE, Camera.main.transform.position.y, _generator.row * GRIDSIZE);
                cameraBase.x *= destinationX;
                cameraBase.z *= destinationY;
                cameraBase.z = _generator.row * GRIDSIZE - cameraBase.z;
                Camera.main.transform.position = cameraBase;
            }
        }
    }
}
