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
    public class PlayerStatusPanel : CommonObject
    {
        public int _id = -1;
        public Player.Info _player;
        public GameObject _avatarObj, _nameObj, _inputObj, _statusObj, _readyObj;
        internal Image _avatar;
        internal Text _input;
        internal Text _nameText;
        internal Text _statusText;
        internal Button _readyButton;


        private void Start()
        {
            _avatar = _avatarObj.GetComponent<Image>();
            _nameText = _nameObj.GetComponent<Text>();
            _input = _inputObj.transform.GetChild(1).GetComponent<Text>();
            _statusText = _statusObj.GetComponent<Text>();
            _readyButton = _readyObj.GetComponent<Button>();
            _readyButton.onClick .AddListener(SetReady);
        }

        private void Update()
        {
            SetNameView();
        }

        private void SetNameView()
        {
            var inpObj = _input.transform.parent.gameObject;
            var readyBtnObj = _readyButton.gameObject;
            _nameText.text = _input.text;
            //Restore
            _nameText.gameObject.SetActive(true);
            inpObj.SetActive(true);
            readyBtnObj.SetActive(true);

            //
            if (_player.Status == Player.Info.STATUS.NONE)
            {
                _nameText.gameObject.SetActive(false);
            }
            else
            {
                inpObj.SetActive(false);
                readyBtnObj.SetActive(false);
            }

            _statusText.text = _player.GetStatusString();
        }

        public void SetReady()
        {
            _player.SetStatus(Player.Info.STATUS.READY);
            GetSceneLogic<PrepareUIScene>().OnPlayerNumberChanged();
        }
    }
}
