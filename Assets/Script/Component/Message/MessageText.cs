using NTUT.CSIE.GameDev.Game;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.Component.Message
{
    public class MessageText : CommonObject
    {
        public const float SHOW_TIME = 5f;
        private const float HIDE_TIME = .5f;
        private float _createTime;
        [SerializeField]
        private Text _textObj;
        [SerializeField]
        private string _text;
        [SerializeField]
        private Color _color;

        protected override void Awake()
        {
            base.Awake();
            _textObj = this.GetComponent<Text>();
        }

        protected void Start()
        {
            _createTime = Time.time;
        }

        protected void Update()
        {
            float startHideTime = _createTime + SHOW_TIME;

            if (Time.time > startHideTime)
            {
                Color tmpColor = _textObj.color;
                float p = Mathf.InverseLerp(startHideTime, startHideTime + HIDE_TIME, Time.time);
                tmpColor.a = Mathf.Lerp(1, 0, p);
                _textObj.color = tmpColor;

                if (Time.time > startHideTime + HIDE_TIME)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                _textObj.text = _text;
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                _textObj.color = _color;
            }
        }
    }
}
