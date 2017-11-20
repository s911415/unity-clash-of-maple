using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.CardSelect
{
    public class Select : MonoBehaviour
    {
        public bool selected;
        private string _number;
        private GameObject _startButton;
        // set number
        public void SetNumber(string number)
        {
            this._number = number;
        }

        // get number 
        public string GetNumber()
        {
            return _number;
        }

        // onclick 
        public void SelectCard()
        {
            if (selected)
            {
                selected = false;
                _startButton.GetComponent<StartButton>().RemoveCard(_number);
            }
            else
            {
                selected = true;
                _startButton.GetComponent<StartButton>().AddCard(_number);
            }

        }

        public void DeleteCard()
        {
            Destroy(this.gameObject);
        }

        // Use this for initialization
        void Start()
        {
            _startButton = GameObject.FindGameObjectWithTag("Startbtn");
        }

        // Update is called once per frame
        void Update()
        {
            if (selected)
                this.GetComponent<Image>().color = Color.gray;
            else
                this.GetComponent<Image>().color = Color.white;
        }
    }
}