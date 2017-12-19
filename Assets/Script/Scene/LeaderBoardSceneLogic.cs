using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Component.Numbers;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Player.Honors;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace NTUT.CSIE.GameDev.Scene
{
    public class LeaderBoardSceneLogic : BasicSceneLogic
    {
        protected override void Awake()
        {
            base.Awake();
            CheckMembers();
            InitMembers();
        }

        private void CheckMembers()
        {
            CheckDiff();
            CheckPlayers();
        }
        private void InitMembers()
        {
        }

        protected virtual void Start()
        {
        }

        protected override void Update()
        {
            base.Update();
        }

        #region Check Members
        private void CheckDiff()
        {
            if (this.Manager.Difficulty == Difficulty.Level.None)
                this.Manager.SetDifficult(Difficulty.Level.Demo);
        }

        private void CheckPlayers()
        {
            // Check Player Enter Normaly
            foreach (var p in Manager.Players)
            {
                CheckPlayer(p);
            }
        }

        internal static void CheckPlayer(Player.Info p)
        {
            FightSceneLogic.CheckPlayer(p);

            if (p.Status < Player.Info.STATUS.OVER)
            {
                p.ResetCounter();
                p.SetStatus(Player.Info.STATUS.OVER);
            }
        }
        #endregion

        #region Initialize Members


        #endregion
    }
}