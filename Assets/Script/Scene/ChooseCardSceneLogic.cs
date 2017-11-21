using NTUT.CSIE.GameDev.Game;
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

        public void SetPlayerInfoAndGameDiff(int difficult, List<string>cardList)
        {
            _difficult = difficult;
            _cardList.AddRange(cardList);
        }

        public void OnClickStartButton()
        {
            Manager.SetDifficult(_difficult);
            Manager.GetPlayerAt(Manager.DEFAULT_PLAYER_ID).SetCardIds(_cardList);
            SceneManager.LoadScene("Fight");
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}