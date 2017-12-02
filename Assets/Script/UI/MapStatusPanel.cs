using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.CardSelect;
using NTUT.CSIE.GameDev.Component.Map;
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
        private HouseInfo _houseInfo;
        Sprite[] _buildingLevel = new Sprite[3];
        Transform picturePanel;
        Transform describePanel;
        public Text upgradeAttackText, upgradeHPText, upgradeSpeedText;

        // Use this for initialization
        void Start()
        {
            _buildingLevel[0] = Resources.Load<Sprite>("Building/empty");
            _buildingLevel[1] = Resources.Load<Sprite>("Building/Building");
            _buildingLevel[2] = Resources.Load<Sprite>("Building/produceBuilding");
            _cardSet.AddRange(Manager.GetPlayerAt(Manager.DEFAULT_PLAYER_ID).GetCardIds());
        }

        // display about mapgrid
        public void DisplayInfo(HouseInfo houseInfo)
        {
            _houseInfo = houseInfo;
            picturePanel = this.transform.Find("Picture");

            for (int i = 0; i < 3; i++)
                picturePanel.transform.GetChild(i).gameObject.SetActive(true);

            describePanel = this.transform.Find("Describe");
            picturePanel.Find("Image").GetComponent<Image>().sprite = _buildingLevel[_houseInfo.Type];
            picturePanel.Find("Hp").GetComponent<Text>().text = _houseInfo.hp.ToString() + "/" + _houseInfo.maxHp.ToString();
            picturePanel.Find("Name").GetComponent<Text>().text = _houseInfo.houseName.ToString() + " " + _houseInfo.houseId.ToString().PadLeft(2,'0');

            switch (_houseInfo.Type)
            {
                case 0:
                    this.Buy();
                    break;

                case 1:
                    this.Select();
                    break;

                case 2:
                    this.Upgrade();
                    break;

                default:
                    Debug.Log("Type error");
                    break;
            }
        }

        // buy
        public void Buy()
        {
            this.CloseDescribePanel();
            describePanel.Find("Buy").gameObject.SetActive(true);
            describePanel.Find("Buy").Find("BuildImage").Find("Image").GetComponent<Image>().sprite = _buildingLevel[1];
        }

        public void Select()
        {
            this.CloseDescribePanel();
            describePanel.Find("Select").gameObject.SetActive(true);

            for (int i = 0; i < 6; i++)
                describePanel.Find("Select").GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("Card/Card" + _cardSet[i]);
        }

        public void Upgrade()
        {
            Debug.Log("This building is Card" + _houseInfo.MonsterInfo.Name + "'s Home.");
            this.CloseDescribePanel();
            describePanel.Find("Upgrade").gameObject.SetActive(true);
            describePanel.Find("Upgrade").GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Monster/" + _houseInfo.MonsterNumber);
            upgradeAttackText.text = string.Format("攻擊：{0}", _houseInfo.RealAttack);
            upgradeHPText.text = string.Format("血量：{0}", _houseInfo.RealHP);
            upgradeSpeedText.text = string.Format("速度：{0}", _houseInfo.RealSpeed);
        }

        public void CloseDescribePanel()
        {
            // 把描述介面全關閉 需要再另開起
            describePanel.Find("Buy").gameObject.SetActive(false);
            describePanel.Find("Select").gameObject.SetActive(false);
            describePanel.Find("Upgrade").gameObject.SetActive(false);
        }

        public void CloseAllPanel()
        {
            for (int i = 0; i < 3; i++)
                picturePanel.transform.GetChild(i).gameObject.SetActive(false);

            CloseDescribePanel();
        }
        // Update is called once per frame
        void Update()
        {
        }
    }
}
