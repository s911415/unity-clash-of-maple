using NTUT.CSIE.GameDev.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.UI
{
    public class PlayerStatusPanel : CommonNetObject
    {
        public int _id = -1;
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
        }

        private string _lastSendName;

        private void Update()
        {
            SetNameView();

            if (IsSelf)
            {
                if (_lastSendName != _input.text)
                {
                    this.CmdSetName(_id, _input.text);
                    _lastSendName = _input.text;
                }
            }

            if (isServer)
            {
                var playerInfo = Manager.GetPlayerAt(_id);
                string name = playerInfo != null ? playerInfo.Name : "???";
                Player.Info.STATUS status = playerInfo != null ? playerInfo.Status : Player.Info.STATUS.NONE;
                RpcUpdatePlayerPanel(name, status);
            }
        }

        [Command]
        private void CmdSetName(int idx, string name)
        {
            Manager.GetPlayerAt(idx).SetUpName(name);
        }

        [ClientRpc]
        private void RpcUpdatePlayerPanel(string name, Player.Info.STATUS status)
        {
            _input.text = name;

            switch (status)
            {
                case Player.Info.STATUS.NONE:
                    _statusText.text = "等待連線";
                    break;

                case Player.Info.STATUS.CONNECTED:
                    _statusText.text = "準備中";
                    break;

                case Player.Info.STATUS.READY:
                    _statusText.text = "Ready";
                    break;
            }
        }

        private void SetNameView()
        {
            var inpObj = _input.transform.parent.gameObject;
            var readyBtnObj = _readyButton.gameObject;
            //Restore
            _nameText.gameObject.SetActive(true);
            inpObj.SetActive(true);
            readyBtnObj.SetActive(true);

            //
            if (IsSelf)
            {
                _nameText.gameObject.SetActive(false);
                return;
            }

            inpObj.SetActive(false);
            readyBtnObj.SetActive(false);
        }

        private bool IsSelf => Storage.PlayerInfo != null&& _id == Storage.PlayerInfo.Id;
    }
}
