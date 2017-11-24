using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NTUT.CSIE.GameDev.Scene
{
    public class FightSceneLogic : BasicSceneLogic
    {
        [SerializeField]
        private MapGridGenerator _mapGenerator;

        [SerializeField]
        protected List<Player.Player> _players;

        public Player.Player GetPlayerAt(int i) => _players[i];

        private void Start()
        {
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
            InitPlayersByDiff();
        }

        #region Check Members
        private void CheckDiff()
        {
            if (this.Manager.Difficulty == Difficulty.Level.None)
                this.Manager.SetDifficult(Difficulty.Level.Demo);
        }

        private void CheckPlayers()
        {
            //Check Player Enter Normaly
            foreach (var p in Manager.Players)
            {
                CheckPlayer(p);
            }
        }

        internal static void CheckPlayer(Player.Info p)
        {
            ChooseCardSceneLogic.CheckPlayer(p);

            if (p.Status != Player.Info.STATUS.FIGHT)
            {
                var cards = new List<Monster.Info>(p.Manager.MonsterInfoCollection.GetInfoListLessOrEqualToLevel(Difficulty.MAX_LEVEL));
                cards.Sort((a, b) => Random.Range(-1, 2));
                cards.RemoveRange(Manager.REQUIRE_START_CARD_COUNT, cards.Count - Manager.REQUIRE_START_CARD_COUNT);
                var cardsID = new List<string>();

                foreach (var info in cards)
                    cardsID.Add(info.ID);

                p.SetCardIds(cardsID);
            }
        }
        #endregion

        #region Initialize Members
        private void InitPlayersByDiff()
        {
            var diff = this.Manager.Difficulty;

            foreach (var p in _players)
                InitPlayer(p, diff);
        }
        private void InitPlayer(Player.Player p, Difficulty.Level level)
        {
            int money = 0;

            switch (level)
            {
                case Difficulty.Level.Easy:
                    money = 2000;
                    break;

                case Difficulty.Level.Normal:
                    money = 3000;
                    break;

                case Difficulty.Level.Hard:
                    money = 5000;
                    break;

                case Difficulty.Level.Demo:
                    money = int.MaxValue;
                    break;
            }

            p.SetMoney(money);
        }
        #endregion
    }
}