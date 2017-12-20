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
        public const int DEFAULT_PLAYERID = 0;
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
            ShowImage();
        }

        protected override void Update()
        {
            base.Update();          
        }

        private void ShowImage()
        {
                var winner = GameObject.FindGameObjectWithTag("Win");
                var loser = GameObject.FindGameObjectWithTag("Lose");
                winner.SetActive(((GetWinner() == DEFAULT_PLAYERID)));
                loser.SetActive(!(GetWinner() == DEFAULT_PLAYERID));
        }

        public void OnButtonClick()
        {
            Debug.Log("Click Button");
        }

        public int GetWinner()
        {
            var playerInfo = this.Manager.Players.Where(p => p.LastHP > 0).FirstOrDefault();
            if (playerInfo)
            {                                                                                                                               
                return playerInfo.id;
            }

            return -1;
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public int GetLoser()
        {
            var playerInfo = this.Manager.Players.Where(p => p.LastHP <= 0).FirstOrDefault();
            if (playerInfo)
            {
                return playerInfo.id;
            }

            return -1;
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
                if (p.id == 0)
                {
                    p.SetName("Player0");
                    p.SetLastHP(500);
                }
                else
                {
                    p.SetName("Robot");
                    p.SetLastHP(0);
                }
            }

        }
        #endregion

        #region Initialize Members


        #endregion
    }
}