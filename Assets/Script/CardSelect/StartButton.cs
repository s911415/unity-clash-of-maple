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
        private Image _image;
        private Button _button;

        // Use this for initialization
        void Start()
        {
            _difficultBtn = GameObject.FindGameObjectWithTag("CreateBtn");
            _image = GetComponent<Image>();
            _button = gameObject.GetComponent<Button>();
            ResetCardSet();
        }

        // reset
        public void ResetCardSet()
        {
            _selectCardSet = new List<string>();
            OnCardListChanged();
        }

        public void AddCard(string cardNumber)
        {
            _selectCardSet.Add(cardNumber);
            OnCardListChanged();
        }

        public string GetCard(int index)
        {
            return _selectCardSet[index];
        }

        public void RemoveCard(string cardNumber)
        {
            _selectCardSet.Remove(cardNumber);
            OnCardListChanged();
        }

        public void OnClick()
        {
            CreateCard createCard = _difficultBtn.GetComponent<CreateCard>();
            GetSceneLogic<ChooseCardSceneLogic>().SetPlayerInfoAndGameDiff(createCard.gameDifficult, _selectCardSet);
            GetSceneLogic<ChooseCardSceneLogic>().OnClickStartButton();
        }

        protected void OnCardListChanged()
        {
            Debug.Log("Select Card: " + ((_selectCardSet.Count == 0) ? "Empty" : string.Join(", ", _selectCardSet)));

            if (_selectCardSet.Count == Manager.REQUIRE_START_CARD_COUNT)
            {
                _image.color = Color.white;
                _button.interactable = true;
            }
            else
            {
                _image.color = Color.gray;
                _button.interactable = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}