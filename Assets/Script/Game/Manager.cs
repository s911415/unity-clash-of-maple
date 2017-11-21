using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Game
{
    public class Manager : CommonObject
    {
        public static int DEFAULT_PLAYER_ID = 0;
        public static int ROBOT_PLAYER_ID = 1;

        private static Manager _managerInstance = null;
        public Player.Info[] _playerList;
        private int _difficult;

        private void Awake()
        {
            if (_managerInstance != null)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                _managerInstance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        private void Start()
        {
            if (_playerList == null || _playerList.Length != 2)
                throw new System.Exception("PlayerInfo not set.");
        }

        internal Player.Info GetPlayerAt(int i)
        {
            return _playerList[i];
        }

        public void SetDifficult(int i)
        {
            this._difficult = i;
            Debug.Log(i);
        }

        public int PlayerCount => _playerList.Length;
        public static Manager Instance => _managerInstance;
    }
}