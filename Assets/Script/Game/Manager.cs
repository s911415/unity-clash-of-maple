using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Game
{
    public class Manager : CommonObject
    {
        public Player.Info[] _playerList;

        private void Start()
        {
            if (_playerList == null || _playerList.Length != 2)
                throw new System.Exception("PlayerInfo not set.");
        }

        internal Player.Info GetPlayerAt(int i)
        {
            return _playerList[i];
        }

        public int PlayerCount => _playerList.Length;
    }
}