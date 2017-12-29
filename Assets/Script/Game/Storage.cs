using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Game
{
    public class Storage : CommonObject
    {
        private Player.Info _playerInfo;
        public Player.Info PlayerInfo
        {
            get
            {
                return _playerInfo;
            }
            set
            {
                _playerInfo = value;
            }
        }
    }
}