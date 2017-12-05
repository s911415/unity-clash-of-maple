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
            UpdateWidth();
        }

        private void Update()
        {
            if (_width == 0) UpdateWidth();

// #if UNITY_EDITOR
            UpdateValueWidth();
// #endif
        }

        private void UpdateValueWidth()
        {
            _valRect.sizeDelta = new Vector2(_width * _value, _valRect.sizeDelta.y);
        }

        private void UpdateWidth()
        {
            _width = _bgRect.rect.width;
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
