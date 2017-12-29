using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NTUT.CSIE.GameDev.UI
{
    public class DialogBuilder
    {
        private static List<Dialog> _dialogInstances = new List<Dialog>();
        private string _title, _content;
        private bool _yesBtn, _noBtn, _backDrop;
        private Dialog.Icon _icon;
        private Dialog.IDialogEventListener _listener;
        private event Action _onBeforeDestroyEvent;

        public DialogBuilder()
        {
            SetDefault();
        }

        private void SetDefault()
        {
            _title = string.Empty;
            _content = "Content";
            _yesBtn = true;
            _noBtn = false;
            _backDrop = true;
            _listener = null;
        }

        public DialogBuilder SetTitle(string title)
        {
            this._title = title;
            return this;
        }

        public DialogBuilder SetContent(string content)
        {
            this._content = content;
            return this;
        }

        public DialogBuilder SetYesBtnStatus(bool enable)
        {
            this._yesBtn = enable;
            return this;
        }

        public DialogBuilder SetNoBtnStatus(bool enable)
        {
            this._noBtn = enable;
            return this;
        }

        public DialogBuilder SetBackDrop(bool enable)
        {
            this._backDrop = enable;
            return this;
        }

        public DialogBuilder SetIcon(Dialog.Icon icon)
        {
            this._icon = icon;
            return this;
        }

        public DialogBuilder AddOnBeforeDestroyListener(Action action)
        {
            this._onBeforeDestroyEvent += action;
            return this;
        }

        public DialogBuilder SetClickListener(Dialog.IDialogEventListener listener)
        {
            this._listener = listener;
            return this;
        }

        public Dialog Show(Canvas canvas)
        {
            var dialogObj = UnityEngine.Object.Instantiate(
                                Resources.Load<GameObject>("Prefab/Dialog"),
                                canvas.transform
                            );
            var dialog = dialogObj.GetComponent<Dialog>();
            dialog.Title = _title;
            dialog.Content = _content;
            dialog.showYesBtn = _yesBtn;
            dialog.showNoBtn = _noBtn;
            dialog.showBackDrop = _backDrop;
            dialog.SetBackground(_icon);
            dialog.SetListener(_listener);
            dialog.OnBeforeDestroy += _onBeforeDestroyEvent;
            dialogObj.transform.localPosition = Vector3.zero;
            dialog.AddOnBeforeDestroyListener(() => _dialogInstances.Remove(dialog));
            _dialogInstances.Add(dialog);
            return dialog;
        }

        public static bool HasAnyDialog => _dialogInstances.Count == 0;

        public static IReadOnlyList<Dialog> DialogList => _dialogInstances;
    }
}
