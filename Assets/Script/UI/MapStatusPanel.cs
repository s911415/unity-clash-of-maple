using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.CardSelect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.UI
{
    public class MapStatusPanel : CommonObject
    {
        List<string> _cardSet = new List<string>();
        public GameObject cardPrefab;
        Sprite[] _buildingLevel = new Sprite[3];
        // Use this for initialization
        void Start()
        {
            _buildingLevel[0] = Resources.Load<Sprite>("Building/empty");
            _buildingLevel[1] = Resources.Load<Sprite>("Building/Building");
            _buildingLevel[2] = Resources.Load<Sprite>("Building/produceBuilding");
            _cardSet.AddRange(Manager.GetPlayerAt(Manager.DEFAULT_PLAYER_ID).GetCardIds());
        }

        // display about mapgrid
        public void DisplayInfo(int type)
        {
            this.transform.Find("Picture").Find("Image").GetComponent<Image>().sprite = _buildingLevel[type];
            if (type == 0) this.Buy();
            else if (type == 1) this.Select();
            else if (type == 2) this.Upgrade();
        }

        // buy
        public void Buy()
        {
            this.CloseDescribePanel();
            this.transform.Find("Describe").Find("Buy").gameObject.SetActive(true);
            this.transform.Find("Describe").Find("Buy").Find("Image").GetComponent<Image>().sprite = _buildingLevel[1];
        }

        public void Select()
        {
            GameObject btn;
            this.CloseDescribePanel();
            this.transform.Find("Describe").Find("Select").gameObject.SetActive(true);

            foreach (string cardNumber in _cardSet)
            {
                btn = (GameObject)Instantiate(cardPrefab);
                btn.GetComponent<Select>().SetNumber(cardNumber);
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Card/Card" + cardNumber);
                btn.transform.SetParent(this.transform.Find("Describe").Find("Select").transform, false);
            }
        }

        public void Upgrade()
        {
            this.CloseDescribePanel();
            this.transform.Find("Describe").Find("Upgrade").gameObject.SetActive(true);
        }

        void CloseDescribePanel()
        {
            this.transform.Find("Describe").Find("Buy").gameObject.SetActive(false);
            this.transform.Find("Describe").Find("Select").gameObject.SetActive(false);
            this.transform.Find("Describe").Find("Upgrade").gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}
