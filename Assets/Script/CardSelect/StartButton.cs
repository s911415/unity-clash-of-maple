using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.CardSelect
{
    public class StartButton : CommonObject
    {
        private List<string> _selectCardSet;
        private GameObject _difficultBtn;

        // Use this for initialization
        void Start()
        {
            ResetCardSet();
            _difficultBtn = GameObject.FindGameObjectWithTag("CreateBtn");
        }

        // reset
        public void ResetCardSet()
        {
            _selectCardSet = new List<string>();
        }

        public void AddCard(string cardNumber)
        {
            _selectCardSet.Add(cardNumber);
        }

        public string GetCard(int index)
        {
            return _selectCardSet[index];
        }

        public void RemoveCard(string cardNumber)
        {
            _selectCardSet.Remove(cardNumber);
        }

        public void OnClick()
        {
            CreateCard createCard = _difficultBtn.GetComponent<CreateCard>();
            GetSceneLogic<ChooseCardSceneLogic>().SetPlayerInfoAndGameDiff(createCard.gameDifficult, _selectCardSet);
            GetSceneLogic<ChooseCardSceneLogic>().OnClickStartButton();
        }

        // Update is called once per frame
        void Update()
        {
            if (_selectCardSet.Count == 6)
            {
                string cardSet = null;
                this.GetComponent<Image>().color = Color.white;
                this.gameObject.GetComponent<Button>().interactable = true;
                foreach (string tt in _selectCardSet)
                    cardSet += tt;
                Debug.Log(cardSet);
            }
            else
            {
                this.GetComponent<Image>().color = Color.gray;
                this.gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }
}