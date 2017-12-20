using NTUT.CSIE.GameDev.Game;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Component.Message
{
    public class MessageConsole : CommonObject
    {
        [SerializeField]
        protected GameObject _textTemplate;

        public MessageConsole Show(Color color, string text)
        {
            var obj = Instantiate(_textTemplate, this.transform);
            var textObj = obj.GetComponent<MessageText>();
            textObj.Text = text;
            textObj.Color = color;
            UpdateTextPosition();
            return this;
        }

        public MessageConsole Show(string text)
        {
            return Show(Color.white, text);
        }

        protected void Update()
        {
        }

        private void UpdateTextPosition()
        {
            int lastY = 0;
            var textObjArray = new List<MessageText>();

            foreach (Transform t in this.transform)
            {
                var textObj = t.gameObject.GetComponent<MessageText>();

                if (textObj && textObj.gameObject.activeSelf) textObjArray.Add(textObj);
            }

            var x = textObjArray[0].GetComponent<RectTransform>();
            int height = textObjArray.Count > 0 ? (int)x.sizeDelta.y : 20;

            for (int i = textObjArray.Count - 1; i >= 0; i--)
            {
                var textObj = textObjArray[i];
                var rectTrans = textObj.GetComponent<RectTransform>();
                var pos = rectTrans.localPosition;
                pos.y = lastY;
                rectTrans.localPosition = pos;
                lastY += height;
            }
        }
    }
}
