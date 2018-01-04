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
        private List<int> _selectCardSet;
        private GameObject _difficultBtn;
        private Image _image;
        private Button _button;
        private Text _btnText;

        // Use this for initialization
        void Start()
        {
            _difficultBtn = GameObject.FindGameObjectWithTag("CreateBtn");
            _image = GetComponent<Image>();
            _button = gameObject.GetComponent<Button>();
            _btnText = _button.GetComponentInChildren<Text>();
            ResetCardSet();
        }

        // reset
        public void ResetCardSet()
        {
            _selectCardSet = new List<int>();
            OnCardListChanged();
        }

        public void AddCard(int cardNumber)
        {
            _selectCardSet.Add(cardNumber);
            OnCardListChanged();
        }

        public int GetCard(int index)
        {
            return _selectCardSet[index];
        }

        public void RemoveCard(int cardNumber)
        {
            _selectCardSet.Remove(cardNumber);
            OnCardListChanged();
        }

        public void OnClick()
        {
            this.DeleteLargerCard();
            CreateCard createCard = _difficultBtn.GetComponent<CreateCard>();
            _button.interactable = false;
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
                _btnText.text = $"開始遊戲";
            }
            else
            {
                _image.color = Color.gray;
                _button.interactable = false;
                _btnText.text = $"再選{Manager.REQUIRE_START_CARD_COUNT - _selectCardSet.Count}張卡牌";
            }
        }

        private void DeleteLargerCard()
        {
            GameObject[] allcard = GameObject.FindGameObjectsWithTag("Card");

            foreach (GameObject redundancy in allcard)
            {
                if (redundancy.GetComponent<Select>().flag == 1)
                {
                    Destroy(redundancy);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}