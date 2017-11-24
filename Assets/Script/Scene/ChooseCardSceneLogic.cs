﻿using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NTUT.CSIE.GameDev.Scene
{
    public class ChooseCardSceneLogic : BasicSceneLogic
    {
        private List<string> _selectCardSet;
        private GameObject _difficultBtn;

        // Use this for initialization
        private int _difficult;
        private List<string> _cardList = new List<string>();

        protected void Start()
        {
            CheckPlayers();

            foreach (var player in Manager.Players)
            {
                player.SetStatus(Player.Info.STATUS.SELECTING_CARD);
            }
        }

        public void SetPlayerInfoAndGameDiff(int difficult, List<string>cardList)
        {
            _difficult = difficult;
            _cardList.AddRange(cardList);
        }

        public void OnClickStartButton()
        {
            Manager.SetDifficult((Difficulty.Level)_difficult);
            Manager.GetPlayerAt(Manager.DEFAULT_PLAYER_ID).SetCardIds(_cardList);
            Manager.GetPlayerAt(Manager.ROBOT_PLAYER_ID).SetCardIds(SelectCardRandomly());

            foreach (var player in Manager.Players)
                player.SetStatus(Player.Info.STATUS.FIGHT);

            SceneManager.LoadScene("Fight");
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
            if (p.Status != Player.Info.STATUS.READY)
            {
                p.SetName("Player" + p.id);
            }
        }


        private IEnumerable<string> SelectCardRandomly()
        {
            var list = new HashSet<string>();
            var set = Manager.MonsterInfoCollection.GetInfoListLessOrEqualToLevel(_difficult);

            while (list.Count < Manager.REQUIRE_START_CARD_COUNT)
            {
                var idx = Random.Range(0, set.Count);

                list.Add(set[idx].ID);
            }

            return list;
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}