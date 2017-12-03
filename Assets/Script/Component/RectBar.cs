using NTUT.CSIE.GameDev.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Component
{
    public class RectBar : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _bgRect = null, _valRect = null;
        private float _value = 0;
        private float _width;

        private void Start()
        {
            _width = _bgRect.rect.width;
        }

        private void Update()
        {
#if UNITY_EDITOR
            UpdateValueWidth();
#endif
        }

        private void UpdateValueWidth()
        {
            _valRect.sizeDelta = new Vector2(_width * _value, _valRect.sizeDelta.y);
        }

        [SerializeField]
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                UpdateValueWidth();
            }
        }
    }
}
