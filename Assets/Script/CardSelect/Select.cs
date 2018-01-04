using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.CardSelect
{
    public class Select : MonoBehaviour
    {
        public bool selected;
        private int _number;
        private GameObject _startButton;
        bool _isFinshAnimation = false;
        // set number
        public void SetNumber(int number)
        {
            this._number = number;
        }

        // get number
        public int GetNumber()
        {
            return _number;
        }

        // onclick
        public void SelectCard()
        {
            GameObject[] allCard = null;
            var startBtn = _startButton.GetComponent<StartButton>();

            if (flag == 1) //Is Big Card
            {
                allCard = GameObject.FindGameObjectsWithTag("Card");

                foreach (GameObject go in allCard)
                {
                    var select = go.GetComponent<Select>();

                    if (select.GetNumber() == _number && select.flag == 0)
                    {
                        if (startBtn.IsAllowAddCard && !select.selected)
                            select.selected = true;
                        else if (startBtn.IsAllowRemoveCard && select.selected)
                            select.selected = false;
                    }
                }
            }

            if (selected)
            {
                if (startBtn.IsAllowRemoveCard)
                {
                    selected = false;
                    startBtn.RemoveCard(_number);
                }
            }
            else
            {
                if (startBtn.IsAllowAddCard)
                {
                    startBtn.AddCard(_number);
                    selected = true;
                }
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
            // 滑鼠進入原卡片且複製prefab不為空 複製卡片
            if (parameter == 2 && !cloneObject && flag == 0)
            {
                this.DeleteRedundancy();
                cloneObject = (GameObject)Instantiate(gameObject);//複製新的卡片
                cloneObject.GetComponent<Select>().flag = 1;//將複製卡片的改為1
                cloneObject.GetComponent<Select>().SetNumber(_number);
                cloneObject.transform.SetParent(GameObject.FindGameObjectWithTag("canvas").transform); //修改父物件
                Vector3 v3 = transform.localPosition;
                v3.y += 100;
                cloneObject.transform.localPosition = v3;
                cloneObject.GetComponent<RectTransform>().sizeDelta = new Vector2(84f, 120f);
                cloneObject.GetComponent<Animator>().Play("CardAnimation");
            }
            //// 滑鼠離開卡片 刪除他的複製卡片
            //else if (parameter == 1 && flag == 0)
            //{
            //    GameObject[] allcard = GameObject.FindGameObjectsWithTag("Card");
            //    foreach (GameObject redundancy in allcard)
            //    {
            //        if (redundancy.GetComponent<Select>().flag == 1 && redundancy.GetComponent<Select>()._number == this._number)
            //        {
            //            Destroy(redundancy);
            //        }
            //    }
            //}
            // 離開複製卡片且動畫結束
            else if (parameter == 1 && flag == 1 && _isFinshAnimation)
            {
                this.DestroyTheObject();
            }
        }

        public void FinishAnimation()
        {
            _isFinshAnimation = true;
        }

        private void DestroyTheObject()
        {
            Destroy(this.gameObject);
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