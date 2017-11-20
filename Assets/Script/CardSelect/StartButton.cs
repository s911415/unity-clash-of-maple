using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.CardSelect
{
    public class StartButton : MonoBehaviour
    {
        private List<string> _total;

        // Use this for initialization
        void Start()
        {
            ResetCardSet();
        }

        // reset
        public void ResetCardSet()
        {
            _total = new List<string>();
        }

        public void AddCard(string cardNumber)
        {
            _total.Add(cardNumber);
        }

        public string GetCard(int index)
        {
            return _total[index];
        }

        public void RemoveCard(string cardNumber)
        {
            _total.Remove(cardNumber);
        }

        // Update is called once per frame
        void Update()
        {
            if (_total.Count == 6)
            {
                string cardSet = null;
                this.gameObject.GetComponent<Button>().interactable = true;
                foreach (string tt in _total)
                    cardSet += tt;
                Debug.Log(cardSet);
            }
            this.gameObject.GetComponent<Button>().interactable = false;
        }
    }
}