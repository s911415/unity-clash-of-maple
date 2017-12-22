using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Component.Message;

namespace NTUT.CSIE.GameDev.Player
{
    [RequireComponent(typeof(Player))]
    public class MessageHandler : CommonObject
    {
        private Player _player;
        private MessageConsole _console;

        protected override void Awake()
        {
            base.Awake();
            _console = GetSceneLogic<FightSceneLogic>().Console;
            _player = GetComponent<Player>();
            Debug.Assert(_console != null);
            Debug.Assert(_player != null);
        }

        protected void Start()
        {
            BindEvent();
        }

        private void BindEvent()
        {
            _player.OnHouseCreated += (house) => house.OnHouseDestroy += OnHouseDestroyed;
            _player.OnMoneyChanged += OnMoneyChanged;
        }

        private void OnHouseDestroyed(Point p)
        {
            _console.Show(Color.red, $"你在{p}的房子被打垮了。");
        }

        public void OnMoneyChanged(int offset)
        {
            var prefix = offset < 0 ? "扣除" : "獲得";
            int diff = System.Math.Abs(offset);
            _console.Show(Color.gray, $"{prefix}${diff:#,##0}");
        }
    }
}
