using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Game
{
    public class Manager : CommonObject
    {
        public const int DEFAULT_PLAYER_ID = 0;
        public const int ROBOT_PLAYER_ID = 1;
        public const int REQUIRE_START_CARD_COUNT = 6;

        private static Manager _managerInstance = null;
        [SerializeField]
        private static Monster.InfoCollection _monsterInfoCollection = null;
        private bool _initialized = false;
        public Player.Info[] _playerList;

        [SerializeField]
        private Difficulty.Level _difficult = Game.Difficulty.Level.None;

        protected override void Awake()
        {
            if (_managerInstance != null)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                Initialize();
            }
        }

        private void Start()
        {
            if (_playerList == null || _playerList.Length != 2)
                throw new System.Exception("PlayerInfo not set.");
        }

        internal Player.Info[] Players => _playerList;

        internal Player.Info GetPlayerAt(int i)
        {
            return _playerList[i];
        }

        public void SetDifficult(Difficulty.Level i)
        {
            this._difficult = i;
        }

        public void Initialize()
        {
            _managerInstance = this;
            DontDestroyOnLoad(this.gameObject);
            _monsterInfoCollection = new Monster.InfoCollection();
            _initialized = true;
        }

        public Difficulty.Level Difficulty => _difficult;

        public int PlayerCount => _playerList.Length;
        public Monster.InfoCollection MonsterInfoCollection => _monsterInfoCollection;
        public static Manager Instance => _managerInstance;
        public bool IsInit => _initialized;
    }
}