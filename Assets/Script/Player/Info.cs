using NTUT.CSIE.GameDev.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Player
{
    public class Info : CommonNetObject
    {
        public enum STATUS
        {
            NONE, CONNECTED, READY
        }

        [SyncVar]
        private int _id;
        public int Id => _id;

        [SyncVar]
        private string _name;
        public string Name => _name;

        [SyncVar]
        private STATUS _status;
        public STATUS Status => _status;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            _status = STATUS.NONE;

            if (isServer)
            {
                Manager.Attach(this);
                _name = string.Empty;
            }

            if (isLocalPlayer)
            {
                Storage.PlayerInfo = this;
            }
        }

        internal Info SetUpId(int id)
        {
            this._id = id;
            return this;
        }

        internal Info SetUpName(string name)
        {
            this._name = name;
            return this;
        }

    }
}