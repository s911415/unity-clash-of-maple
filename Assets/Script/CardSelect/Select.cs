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
            GameObject[] allCard = null;
            if (flag == 1)
            {
                allCard = GameObject.FindGameObjectsWithTag("Card");
                foreach (GameObject go in allCard)
                {
                    if (go.GetComponent<Select>().GetNumber() == _number && go.GetComponent<Select>().flag == 0)
                    {
                        go.GetComponent<Select>().selected = !selected;
                    }
                }
            }
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
                this.GetComponent<Image>().color = Color.white;
            else
                this.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
        }

        GameObject cloneObject = null;
        public int flag = 0;//辨識是原來的卡片還是新複製的卡片 原來的卡片=0 複製出的卡片=1
        //滑鼠進入事件parameter=2 離開parameter=1
        public void AdjustScale(int parameter)
        {
            if (parameter == 2 && !cloneObject && flag == 0) //滑鼠進入原來卡片
            {
                this.DeleteRedundancy();
                cloneObject = (GameObject)Instantiate(gameObject);//複製新的卡片
                cloneObject.GetComponent<Select>().flag = 1;//將複製卡片的改為1
                cloneObject.GetComponent<Select>().SetNumber(_number);
                cloneObject.transform.SetParent(GameObject.FindGameObjectWithTag("canvas").transform); //修改父物件
                Vector3 v3 = transform.localPosition;
                v3.y += 35;
                cloneObject.transform.localPosition = v3;
                cloneObject.GetComponent<RectTransform>().sizeDelta = new Vector2(84f, 120f);
                // cloneObject.GetComponent<Select>().enabled = false;                
                cloneObject.transform.localScale = new Vector2(1f, 1f) * parameter;
            }
            else if (parameter == 1 && flag == 1)
            {
                Destroy(this.gameObject);
            }     
        }

        private void DeleteRedundancy()
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
    }
}