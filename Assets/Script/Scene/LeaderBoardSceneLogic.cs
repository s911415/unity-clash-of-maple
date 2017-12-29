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
            new DialogBuilder().SetContent("是否要重新開始遊戲?")
            .SetYesBtnStatus(true).SetNoBtnStatus(true)
            .SetClickListener(new Dialog.ConfirmDialogEventListener(RestartGame, PromptQuitGame))
            .Show(_window);
            //QuitGame();
        }

        private void RestartGame()
        {
            Destroy(this.Manager.gameObject);
            SceneManager.LoadScene("PrepareUI");
        }

        private void PromptQuitGame()
        {
            new DialogBuilder().SetContent("是否要結束遊戲?")
            .SetYesBtnStatus(true).SetNoBtnStatus(true)
            .SetClickListener(new Dialog.ConfirmDialogEventListener(QuitGame, () => { }))
            .Show(_window);
        }

        public int GetWinner()
        {
            var playerInfo = this.Manager.Players.Where(p => p.Result.HP > 0).FirstOrDefault();

            if (playerInfo)
            {
                return playerInfo.id;
            }

            return -1;
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public int GetLoser()
        {
            var playerInfo = this.Manager.Players.Where(p => p.Result.HP <= 0).FirstOrDefault();

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
                p.SetStatus(Player.Info.STATUS.OVER);
                var randomKilledMonsterInfo = new Dictionary<int, int>();

                foreach (var id in p.Manager.MonsterInfoCollection.GetAllMonsterId())
                {
                    randomKilledMonsterInfo.Add(id, Random.Range(0, 10));
                }

                p.SetResult(new Result(
                                p.id,
                                1000,
                                500 * (1 - p.id),
                                1,
                                2,
                                randomKilledMonsterInfo,
                                new List<Honor> { Honor.開發者模式, Honor.除錯大師}
                            ));
            }
        }
        #endregion

        #region Initialize Members


        #endregion
    }
}