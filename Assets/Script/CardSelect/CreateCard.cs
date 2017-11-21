using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.CardSelect
{
    public class CreateCard : MonoBehaviour
    {
        public RectTransform ParentPanel;
        public GameObject prefab;
        List<GameObject> _card;
        const int CARD_NUMBER = 12;
        string[,] threeCardSet = new string[3, CARD_NUMBER] {
            { "00", "01", "02", "06", "07", "08", "10", null, null, null, null, null },
            { "00", "01", "02", "04", "06", "07", "08", "09", "10", "11", null, null },
            { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11" } };
        //
        public List<string> monsterInfo = new List<string>();
        // game mode 1 easy 2 normal 3 hard
        public int gameDifficult;

        // set difficult
        public void SetGameDifficult(int mode)
        {
            Clean();
            gameDifficult = mode;
            Create();
        }

        void Start()
        {
            this.SetGameDifficult(1);
            //
            for (int i = 0; i < 20; i++)
                monsterInfo.Add(i.ToString());
            //
        }

        // Clean
        public void Clean()
        {
            foreach (Transform child in transform)
                GameObject.Destroy(child.gameObject);
        }

        // create
        void Create()
        {
            GameObject startbtn = GameObject.FindGameObjectWithTag("Startbtn");
            startbtn.GetComponent<StartButton>().ResetCardSet();

            for (int i = 0; i < CARD_NUMBER; i++)
            {
                if (threeCardSet[gameDifficult - 1, i] != null)
                {
                    GameObject btn = (GameObject)Instantiate(prefab);
                    btn.GetComponent<Select>().SetNumber(threeCardSet[gameDifficult - 1, i]);
                    btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Card/Card" + threeCardSet[gameDifficult - 1, i]);
                    btn.transform.SetParent(ParentPanel, false);
                    btn.GetComponent<Select>().flag = 0;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
