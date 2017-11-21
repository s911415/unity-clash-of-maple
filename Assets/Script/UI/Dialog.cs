using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace NTUT.CSIE.GameDev.UI
{
    public class Dialog : MonoBehaviour
    {
        public enum ButtonValue
        {
            Yes, No
        }
        public enum Icon
        {
            Info, Error
        }
        public bool showYesBtn = true;
        public bool showNoBtn = false;
        public bool showBackDrop = true;
        public Image background;
        public Sprite InfoBg, ErrorBg;
        public Button yesBtn, noBtn;
        public Text title, content;
        public GameObject backDrop;
        private IDialogEventListener _dialogListener = null;

        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
            if (!showYesBtn) yesBtn.gameObject.SetActive(false);

            if (!showNoBtn) noBtn.gameObject.SetActive(false);

            if (!showBackDrop) backDrop.SetActive(false);

            yesBtn.onClick.AddListener(() => OnButtonClick(ButtonValue.Yes));
            noBtn.onClick.AddListener(() => OnButtonClick(ButtonValue.No));
        }

        public void SetListener(IDialogEventListener eventListener)
        {
            this._dialogListener = eventListener;
        }

        public void SetBackground(Icon icon)
        {
            if (icon == Icon.Info)
                background.sprite = InfoBg;
            else if (icon == Icon.Error)
                background.sprite = ErrorBg;
        }

        private void OnButtonClick(ButtonValue value)
        {
            if (_dialogListener != null)
                _dialogListener.OnClick(value);

            Destroy(this.gameObject);
        }

        public interface IDialogEventListener
        {
            void OnClick(ButtonValue value);
        }

        public class ConfirmDialogEventListener : IDialogEventListener
        {
            private Action _onYesClicked, _onNoClicked;

            public ConfirmDialogEventListener(Action yesCallback, Action noCallback)
            {
                _onYesClicked = yesCallback;
                _onNoClicked = noCallback;
            }

            public virtual void OnClick(ButtonValue value)
            {
                if (value == ButtonValue.Yes)
                {
                    _onYesClicked?.Invoke();
                }
                else if (value == ButtonValue.No)
                {
                    _onNoClicked?.Invoke();
                }
            }
        }

        public class MessageDialogEventListener : ConfirmDialogEventListener
        {

            public MessageDialogEventListener(Action callback = null) : base(callback, null)
            {
            }
        }

        public string Title
        {
            get
            {
                return title.text;
            }
            set
            {
                title.text = value;
            }
        }

        public string Content
        {
            get
            {
                return content.text;
            }
            set
            {
                content.text = value;
            }
        }

    }
}
