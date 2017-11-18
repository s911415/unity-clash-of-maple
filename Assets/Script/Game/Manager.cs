using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Game
{
    public class Manager : CommonNetObject
    {
        private Dictionary<int, Player.Info> _playerList;

        private void Awake()
        {
            _playerList = new Dictionary<int, Player.Info>();
        }

        internal void Attach(Player.Info info)
        {
            info.SetUpId(PlayerCount);
            _playerList.Add(info.Id, info);
        }

        internal Player.Info GetPlayerAt(int i)
        {
            if (!_playerList.ContainsKey(i))
                return null;

            return _playerList[i];
        }

        public int PlayerCount => _playerList.Count;
        internal Dictionary<int, Player.Info> PlayerList => _playerList;
    }
}