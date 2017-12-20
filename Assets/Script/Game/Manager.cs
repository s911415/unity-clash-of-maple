using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Action = System.Action;

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
                if (this != _managerInstance)
                {
                    this.gameObject.SetActive(false);
                    Destroy(this.gameObject);
                }
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
            if (_managerInstance != null)
                return;

            _managerInstance = this;
            DontDestroyOnLoad(this.gameObject);
            _monsterInfoCollection = new Monster.InfoCollection();
            _initialized = true;
        }


        private static uint _timerID = 0;
        private static Dictionary<uint, Timer> _timerList = new Dictionary<uint, Timer>();

        protected virtual void Update()
        {
            var keyClone = new List<uint>();
            keyClone.AddRange(_timerList.Keys);

            foreach (var t in keyClone)
                _timerList[t].Check();
        }

        public uint _SetTimeout(Action action, uint timeoutMS)
        {
            var id = _timerID++;
            _timerList.Add(
                id,
                new Timer(
                    id, action, timeoutMS
                )
            );
            return id;
        }

        public uint _SetInterval(Action action, uint timeoutMS)
        {
            var id = _timerID++;
            _timerList.Add(
                id,
                new ContinuousTimer(
                    id, action, timeoutMS
                )
            );
            return id;
        }

        public void _ClearTimeout(uint timerId)
        {
            _timerList.Remove(timerId);
        }

        protected virtual void OnDestroy()
        {
            if (_managerInstance == this)
            {
                var keyClone = new List<uint>();
                keyClone.AddRange(_timerList.Keys);

                foreach (var t in keyClone)
                    _ClearTimeout(t);

                _managerInstance = null;
            }
        }

        private class Timer
        {
            protected uint _id;
            protected float _runTime;
            protected Action _action;
            protected uint _timeout;

            public Timer(uint id, Action action, uint timeoutMS)
            {
                _id = id;
                _timeout = timeoutMS;
                _runTime = Time.time + (float)_timeout / 1e3f;
                _action = action;
            }

            public virtual void Check()
            {
                if (Time.time >= _runTime)
                {
                    _timerList.Remove(this._id);
                    _action();
                }
            }
        }

        private class ContinuousTimer : Timer
        {
            public ContinuousTimer(uint id, Action action, uint timeoutMS): base(id, action, timeoutMS)
            {
            }

            public override void Check()
            {
                if (Time.time >= _runTime)
                {
                    _runTime = Time.time + (float)_timeout / 1e3f;
                    _action();
                }
            }
        }

        public Difficulty.Level Difficulty => _difficult;

        public int PlayerCount => _playerList.Length;
        public Monster.InfoCollection MonsterInfoCollection => _monsterInfoCollection;
        public static Manager Instance => _managerInstance;
        public bool IsInit => _initialized;
    }
}