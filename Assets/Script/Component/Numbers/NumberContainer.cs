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

        private void Start()
        {
            Destroy(this.gameObject, NumberController.TOTAL_TIME);
        }

        public void SetTargetPosition(Vector3 p)
        {
            _targetPosition = Helper.Clone(p);
            UpdatePosition();
        }

        protected void Update()
        {
            UpdatePosition();
        }

        protected void UpdatePosition()
        {
            this.transform.position = Camera.main.WorldToScreenPoint(_targetPosition);
        }
    }
}