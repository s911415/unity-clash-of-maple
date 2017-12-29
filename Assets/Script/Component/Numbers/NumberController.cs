using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.Component.Numbers
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class NumberController : CommonObject
    {
        [SerializeField]
        protected Image _image;
        [SerializeField]
        protected RectTransform _trans;
        public const float TOTAL_TIME = 2f;
        private const float ALPHA_SHOW_TIME = .125f;
        private const float ALPHA_DISAPPEAR_START_TIME = 1f;
        private float _startTime;
        private float _startY;

        protected override void Awake()
        {
            base.Awake();
            _image = GetComponent<Image>();
            _trans = GetComponent<RectTransform>();
        }

        protected void Start()
        {
            _startTime = Time.time;
            _startY = _trans.localPosition.y;
        }

        // Update is called once per frame
        void Update()
        {
            var imgA = GetAlpha();
            var imgYOffset = GetPosYOffset();
            SetImageAlpha(imgA);
            SetImageYOffset(imgYOffset);
        }

        private void SetImageAlpha(float a)
        {
            var c = _image.color;
            c.a = a;
            _image.color = c;
        }

        private void SetImageYOffset(float y)
        {
            var p = transform.localPosition;
            p.y = y;
            _trans.localPosition = p;
        }

        private float GetPosYOffset()
        {
            var at = AliveTime;
            var p = at / TOTAL_TIME;
            return Mathf.Lerp(-10, 20, p) + _startY;
        }

        private float GetAlpha()
        {
            var at = AliveTime;

            if (at <= ALPHA_SHOW_TIME)
            {
                var p = at / ALPHA_SHOW_TIME;
                return Mathf.Lerp(0, 1, p);
            }
            else if (at >= ALPHA_DISAPPEAR_START_TIME)
            {
                var p = (at - ALPHA_DISAPPEAR_START_TIME) / (TOTAL_TIME - ALPHA_DISAPPEAR_START_TIME);
                return Mathf.Lerp(1, 0, p);
            }

            return 1;
        }

        private float AliveTime => Time.time - _startTime;
    }
}