using NTUT.CSIE.GameDev.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using NTUT.CSIE.GameDev.Game;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.Component
{
    public class SelectCardButton : CommonObject, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void MouseHoverEvent();
        public event MouseHoverEvent OnMouseOver;
        public event MouseHoverEvent OnMouseLeave;
        [SerializeField]
        private Button _button;

        protected override void Awake()
        {
            base.Awake();
        }

        public Button Button => _button;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnMouseOver?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnMouseLeave?.Invoke();
        }
    }
}
