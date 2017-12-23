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
        [SerializeField]
        Sprite[] _buildingLevel = new Sprite[3];
        public Text upgradeAttackText, upgradeHPText, upgradeSpeedText;

        [SerializeField]
        protected Button _buyOK, _buyCancel, _upgAtt, _upgHP, _upgSpeed, _disCard, _uniAttBtn, _uniDefBtn;
        [SerializeField]
        protected Button[] _selectCard;
        [SerializeField]
        private GameObject _pictureObj = null, _describeObj = null, _miniMapObj = null,
                           _buyPanel = null, _selectPanel = null, _upgradePanel = null,
                           _masterPanel = null;

        [SerializeField]
        private Image _picImage = null;
        [SerializeField]
        private Text _picHpText = null,
                     _picNameText = null;

        private FightSceneLogic _scene;

        protected override void Awake()
        {
            base.Awake();
            _cardSet.AddRange(
                Manager.GetPlayerAt(Manager.DEFAULT_PLAYER_ID)
                .GetCardIds()
                .Select(mobID => this.Manager.MonsterInfoCollection[mobID])
            );
            _scene = GetSceneLogic<FightSceneLogic>();
            Debug.Assert(_miniMapObj != null);
            BindButtonEvent();
        }

        // Use this for initialization
        void Start()
        {
            this.gameObject.SetActive(false);
        }

        private void BindButtonEvent()
        {
            var player = _scene.GetPlayerAt(Manager.DEFAULT_PLAYER_ID);
            System.Action noMoneyMsg = () => new DialogBuilder().SetContent("你沒錢").Show(_scene.Window);
            System.Action<System.Action> doActionOrShowErrMsg = (System.Action a) =>
            {
                try
                {
                    a();
                }
                catch (System.Exception e)
                {
                    new DialogBuilder()
                    .SetIcon(Dialog.Icon.Error)
                    .SetTitle("錯誤")
                    .SetContent(e.Message)
                    .Show(_scene.Window);
                }
            };
            System.Action<HouseInfo> checkInfoAndShow = (houseInfo) =>
            {
                if (houseInfo != null)
                {
                    CloseAllPanel();
                    DisplayInfo(houseInfo);
                }
                else
                    noMoneyMsg.Invoke();
            };
            _buyOK.onClick.AddListener(() =>
            {
                var houseInfo = player.BuyHouse(_scene.MapGridGenerator.CurPoint);
                checkInfoAndShow.Invoke(houseInfo);
            });
            _buyCancel.onClick.AddListener(() => Hide());

            for (int i = 0; i < _selectCard.Length; i++)
            {
                int finalI = i; // Important, if use i, i always equals to _selectCard.Length
                _selectCard[i].onClick.AddListener(() =>
                {
                    var houseInfo = player.SetHouseMonster(_scene.MapGridGenerator.CurPoint, finalI);
                    checkInfoAndShow.Invoke(houseInfo);
                });
            }

            _upgAtt.onClick.AddListener(() =>
            {
                checkInfoAndShow.Invoke(
                    player.UpgradeHouse(_scene.MapGridGenerator.CurPoint, HouseInfo.UpgradeType.Attack)
                );
            });
            _upgHP.onClick.AddListener(() =>
            {
                checkInfoAndShow.Invoke(
                    player.UpgradeHouse(_scene.MapGridGenerator.CurPoint, HouseInfo.UpgradeType.HP)
                );
            });
            _upgSpeed.onClick.AddListener(() =>
            {
                checkInfoAndShow.Invoke(
                    player.UpgradeHouse(_scene.MapGridGenerator.CurPoint, HouseInfo.UpgradeType.Speed)
                );
            });
            _disCard.onClick.AddListener(() =>
            {
                var houseInfo = player.DiscardHouseMonster(_scene.MapGridGenerator.CurPoint);
                checkInfoAndShow.Invoke(houseInfo);
            });
            _uniAttBtn.onClick.AddListener(() =>
            {
                doActionOrShowErrMsg(() => player.DoUniqueSkill(Player.Player.UniqueSkill.Attack));
            });
            _uniDefBtn.onClick.AddListener(() =>
            {
                doActionOrShowErrMsg(() => player.DoUniqueSkill(Player.Player.UniqueSkill.Defense));
            });
        }

        /// <summary>   Sets an image. </summary>
        ///
        /// <param name="img">          The image. </param>
        /// <param name="hpText">       The hp text. </param>
        /// <param name="houseName">    Name of the house. </param>
        protected void SetImage(Sprite img, string hpText, string houseName)
        {
            if (img) _picImage.sprite = img;

            _picHpText.text = hpText ?? "";
            _picNameText.text = houseName ?? "";
        }


        protected void SetImage(Sprite img, string houseName)
        {
            SetImage(img, houseName, "");
        }

        // display about mapgrid
        public void DisplayInfo(HouseInfo houseInfo)
        {
            Show();

            for (int i = 0; i < 3; i++)
                _pictureObj.transform.GetChild(i).gameObject.SetActive(true);

            SetImage(_buildingLevel[(int)houseInfo.Type], $"{houseInfo.HP}/{houseInfo.MAX_HP}", $"{houseInfo.houseName} {houseInfo.ID:00}");

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

                case HouseInfo.HouseType.Master:
                    this.ShowMaster();
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
            _buyPanel.SetActive(true);
            SetImage(_buildingLevel[0], "空地");
            // _buyPanel.transform.Find("BuildImage").Find("Image").GetComponent<Image>().sprite = _buildingLevel[1];
        }

        public void Select()
        {
            this.CloseDescribePanel();
            _selectPanel.gameObject.SetActive(true);

            for (int i = 0; i < 6; i++)
                _selectPanel
                .transform
                .GetChild(i)
                .GetComponent<Image>()
                .sprite = Resources.Load<Sprite>("Card/Card" + _cardSet[i].IDStr);
        }

        public void Upgrade()
        {
            var houseInfo = _scene.MapGridGenerator.CurHouseInfo;
            this.CloseDescribePanel();
            _upgradePanel.SetActive(true);
            _upgradePanel.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Monster/" + houseInfo.MonsterInfo.IDStr);
            upgradeAttackText.text = string.Format("攻擊：{0}", houseInfo.RealAttack);
            upgradeHPText.text = string.Format("血量：{0}", houseInfo.RealHP);
            upgradeSpeedText.text = string.Format("速度：{0}", houseInfo.RealSpeed);
        }

        public void ShowMaster()
        {
            this.CloseDescribePanel();
            _masterPanel.SetActive(true);
            SetImage(_buildingLevel[10 + _scene.MapGridGenerator.CurHouseInfo.PlayerID], "主塔");
        }

        public void CloseDescribePanel()
        {
            // 把描述介面全關閉 需要再另開起
            foreach (Transform t in _describeObj.transform)
            {
                t.gameObject.SetActive(false);
            }
        }

        public void CloseAllPanel()
        {
            for (int i = 0; i < 3; i++)
                _pictureObj.transform.GetChild(i).gameObject.SetActive(false);

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
