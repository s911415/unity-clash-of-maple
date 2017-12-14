using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Player
{
    [RequireComponent(typeof(Player))]
    public class RobotPlayerController : CommonObject
    {
        private Player _player;

        protected override void Awake()
        {
            base.Awake();
            _player = this.GetComponent<Player>();

            if (_player == null)
            {
                throw new System.Exception("Cannot found Player object");
            }
        }
    }
}
