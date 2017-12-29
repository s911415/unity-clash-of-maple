using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.Component.Numbers
{
    public class NumberContainer : CommonObject
    {
        private Vector3 _targetPosition = Vector3.zero;
        public delegate void NumberDisappearEventHandler();
        public event NumberDisappearEventHandler OnNumbersDisappear;

        private void Start()
        {
            Destroy(this.gameObject, NumberController.TOTAL_TIME);
        }

        public void SetTargetPosition(Vector3 p)
        {
            _targetPosition = p;
            UpdatePosition();
        }

        protected void OnDestroy()
        {
            OnNumbersDisappear?.Invoke();
        }

        protected void Update()
        {
            UpdatePosition();
        }

        protected void UpdatePosition()
        {
            var p = Camera.main.WorldToScreenPoint(_targetPosition);
            this.transform.position = p;
        }
    }
}