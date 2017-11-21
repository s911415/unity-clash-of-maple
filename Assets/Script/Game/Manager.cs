using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Game
{
    public class Manager : CommonObject
    {
        private static Manager _managerInstance = null;
        private static Monster.InfoCollection _monsterInfoCollection = null;
        public Player.Info[] _playerList;

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
                Initialize();
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

        private void Initialize()
        {
            _monsterInfoCollection = new Monster.InfoCollection();
        }

        public int PlayerCount => _playerList.Length;
        public Monster.InfoCollection MonsterInfoCollection => _monsterInfoCollection;
        public static Manager Instance => _managerInstance;
    }
}