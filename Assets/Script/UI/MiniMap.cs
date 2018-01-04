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
        [SerializeField]
        public GameObject _rect;
        const int GRID_SIZE = 10;
        private RectTransform _rectTrans, _bigWinRect, _rectRect;

        protected void Start()
        {
            _rectTrans = _miniMap.GetComponent<RectTransform>();
            _bigWinRect = _bigWindow.GetComponent<RectTransform>();
            _rectRect = _rect.GetComponent<RectTransform>();
        }

        private void Update()
        {
            float w = Camera.main.pixelWidth;
            float h = Camera.main.pixelHeight;

            _rectRect.anchoredPosition = new Vector2(_rectTrans.rect.width * (Camera.main.transform.position.x / 200), _rectTrans.rect.height * (Camera.main.transform.position.z / 100));

            if (Input.GetMouseButton(0) && IsMouseOnGUI)
            {
                float hPercent = _rectTrans.rect.height / _bigWinRect.rect.height;
                float wPercent = _rectTrans.rect.width / _bigWinRect.rect.width;
                Vector3 clickPos = new Vector3();
                Vector3 computePos = new Vector3();
                clickPos.x = Input.mousePosition.x;
                clickPos.y = h - Input.mousePosition.y;
                clickPos.z = Input.mousePosition.z;
                computePos.x = ((clickPos.x / w) - (1 - wPercent)) / wPercent;
                computePos.y = ((clickPos.y / h) - (1 - hPercent)) / hPercent;
                computePos.z = clickPos.z;
                //Debug.Log("x = " + computePos.x + "y = " + computePos.y);
                ScreenMove(computePos.x, computePos.y);
            }
        }

        protected void ScreenMove(float destinationX, float destinationY)
        {
            if (destinationX >= 0f && destinationX <= 1f && destinationY >= 0f && destinationY <= 1f)
            {
                Vector3 cameraBase = new Vector3(_generator.col * GRID_SIZE, Camera.main.transform.position.y, _generator.row * GRID_SIZE);
                cameraBase.x *= (destinationX < 0.22f) ? 0.22f : ((destinationX > 0.78f) ? 0.78f : destinationX);
                cameraBase.z *= (destinationY < 0.25f) ? 0.25f : ((destinationY > 0.75f) ? 0.74f : destinationY);
                cameraBase.z = _generator.row * GRID_SIZE - cameraBase.z;
                Camera.main.transform.position = cameraBase;
            }
        }
    }
}
