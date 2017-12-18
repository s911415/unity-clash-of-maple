using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.CardSelect;
using NTUT.CSIE.GameDev.Component.Map;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.UI
{
    public class MapStatusPanel : CommonObject
    {
        List<Monster.Info> _cardSet = new List<Monster.Info>();
        public GameObject cardPrefab;

        Sprite[] _buildingLevel = new Sprite[3];
        Transform picturePanel;
        Transform describePanel;
        public Text upgradeAttackText, upgradeHPText, upgradeSpeedText;

        [SerializeField]
        protected Button _buyOK, _buyCancel, _upgAtt, _upgHP, _upgSpeed, _disCard;
        [SerializeField]
        protected Button[] _selectCard;

        // Use this for initialization
        void Start()
        {
            _buildingLevel[0] = Resources.Load<Sprite>("Building/empty");
            _buildingLevel[1] = Resources.Load<Sprite>("Building/Building");
            _buildingLevel[2] = Resources.Load<Sprite>("Building/produceBuilding");
            _cardSet.AddRange(
                Manager.GetPlayerAt(Manager.DEFAULT_PLAYER_ID)
                .GetCardIds()
                .Select(mobID => this.Manager.MonsterInfoCollection[mobID])
            );
            BindButtonEvent();
        }

        private void BindButtonEvent()
        {
            var scene = GetSceneLogic<FightSceneLogic>();
            var player = scene.GetPlayerAt(Manager.DEFAULT_PLAYER_ID);
            System.Action noMoneyMsg = () => new DialogBuilder().SetContent("你沒錢").Show(scene.Window);
            System.Action<HouseInfo> checkInfoAndShow = (houseInfo) =>
            {
                if (houseInfo != null)
                {
                    CloseAllPanel();
                    DisplayInfo(houseInfo);
                }
                else
                {
                    noMoneyMsg.Invoke();
                }
            };
            _buyOK.onClick.AddListener(() =>
            {
                var houseInfo = player.BuyHouse(scene.MapGridGenerator.CurPoint);
                checkInfoAndShow.Invoke(houseInfo);
            });
            _buyCancel.onClick.AddListener(() => Hide());

            for (int i = 0; i < _selectCard.Length; i++)
            {
                int finalI = i; // Important, if use i, i always equals to _selectCard.Length
                _selectCard[i].onClick.AddListener(() =>
                {
                    var houseInfo = player.SetHouseMonster(scene.MapGridGenerator.CurPoint, finalI);
                    checkInfoAndShow.Invoke(houseInfo);
                });
            }

            _upgAtt.onClick.AddListener(() =>
            {
                checkInfoAndShow.Invoke(
                    player.UpgradeHouse(scene.MapGridGenerator.CurPoint, HouseInfo.UpgradeType.Attack)
                );
            });
            _upgHP.onClick.AddListener(() =>
            {
                checkInfoAndShow.Invoke(
                    player.UpgradeHouse(scene.MapGridGenerator.CurPoint, HouseInfo.UpgradeType.HP)
                );
            });
            _upgSpeed.onClick.AddListener(() =>
            {
                checkInfoAndShow.Invoke(
                    player.UpgradeHouse(scene.MapGridGenerator.CurPoint, HouseInfo.UpgradeType.Speed)
                );
            });
            _disCard.onClick.AddListener(() =>
            {
                var houseInfo = player.DiscardHouseMonster(scene.MapGridGenerator.CurPoint);
                checkInfoAndShow.Invoke(houseInfo);
            });
        }

        // display about mapgrid
        public void DisplayInfo(HouseInfo houseInfo)
        {
            Show();
            picturePanel = this.transform.Find("Picture");

            for (int i = 0; i < 3; i++)
                picturePanel.transform.GetChild(i).gameObject.SetActive(true);

            describePanel = this.transform.Find("Describe");
            picturePanel.Find("Image").GetComponent<Image>().sprite = _buildingLevel[(int)houseInfo.Type];
            picturePanel.Find("Hp").GetComponent<Text>().text = houseInfo.hp.ToString() + "/" + houseInfo.maxHp.ToString();
            picturePanel.Find("Name").GetComponent<Text>().text = houseInfo.houseName.ToString() + " " + houseInfo.ID.ToString().PadLeft(2, '0');

            switch (houseInfo.Type)
            {
                case HouseInfo.HouseType.Empty:
                    this.Buy();
                    break;

                case HouseInfo.HouseType.Building:
                    this.Select();
                    break;

                case HouseInfo.HouseType.Summon:
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
                describePanel.Find("Select")
                .GetChild(i)
                .GetComponent<Image>()
                .sprite = Resources.Load<Sprite>("Card/Card" + _cardSet[i].IDStr);
        }

        public void Upgrade()
        {
            var houseInfo = GetSceneLogic<FightSceneLogic>().MapGridGenerator.CurHouseInfo;
            this.CloseDescribePanel();
            describePanel.Find("Upgrade").gameObject.SetActive(true);
            describePanel.Find("Upgrade").GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Monster/" + houseInfo.MonsterInfo.IDStr);
            upgradeAttackText.text = string.Format("攻擊：{0}", houseInfo.RealAttack);
            upgradeHPText.text = string.Format("血量：{0}", houseInfo.RealHP);
            upgradeSpeedText.text = string.Format("速度：{0}", houseInfo.RealSpeed);
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

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
        }
    }
}
