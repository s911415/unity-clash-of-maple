using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Component.Numbers;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Player.Honors;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using NTUT.CSIE.GameDev.Component.Message;

namespace NTUT.CSIE.GameDev.Scene
{
    public class FightSceneLogic : BasicSceneLogic
    {
        private const string MONSTER_PREFAB_PATH = "Prefab/Monster/Monster_{0}";

        [SerializeField]
        public MapStatusPanel _controlPanel;

        [SerializeField]
        private GameObject _monsterListObject;

        [SerializeField]
        private MapGridGenerator _mapGenerator;

        [SerializeField]
        private HouseInfo _defaultHouseInfo;

        [SerializeField]
        private NumberCollection _numberCollection;

        [SerializeField]
        protected List<Player.Player> _players;

        [SerializeField]
        protected MessageConsole _console;

        public Player.Player GetPlayerAt(int i) => _players[i];

        private uint _godReward;

        protected override void Awake()
        {
            base.Awake();
            CheckMembers();
            InitMembers();
        }

        private void CheckMembers()
        {
            CheckDiff();
            CheckPlayers();
        }
        private void InitMembers()
        {
            InitPlayersByDiff();
            //定時給錢
            _godReward = SetInterval(GiveEveryoneMoney, 60 * 1000);

            foreach (var p in _players)
            {
                p.OnHPChanged += OnPlayerHPChanged;
            }
        }

        public void SpawnMonster(Monster.Info monsterInfo, int playerID, HouseInfo houseInfo)
        {
            //var houseHeight = houseInfo.gameObject.GetComponent<>
            var objPrefab = Resources.Load<GameObject>(string.Format(MONSTER_PREFAB_PATH, monsterInfo.IDStr));
            var obj = Instantiate(objPrefab, _monsterListObject.transform);
            var mob = obj.GetComponent<Monster.Monster>();
            var offset = (playerID == Manager.DEFAULT_PLAYER_ID) ? 1 : -1;
            mob.SetInfo(playerID, houseInfo.RealHP, houseInfo.RealAttack, houseInfo.RealSpeed).Initialize();
            var newPos = houseInfo.transform.position + offset * new Vector3(4, 0, 0);
            mob.transform.position = newPos;
            var newLocPos = Helper.Clone(mob.transform.localPosition);
            newLocPos.y = 0;
            newLocPos.z -= 5f;
            mob.transform.localPosition = newLocPos;

            if (playerID == Manager.DEFAULT_PLAYER_ID)
            {
                mob.Direction = Direction.Right;
            }
            else
            {
                mob.Direction = Direction.Left;
            }

            mob.Walk();
        }

        public void ShowInfoOnPanel(Point p)
        {
            var house = this.HouseGenerator[p] ?? _defaultHouseInfo;
            var allowShow = false;

            if (house.Type != HouseInfo.HouseType.Master)
            {
                if (
                    p.Column < 10 ||
                    this.Manager.Difficulty == Difficulty.Level.Demo
                )
                {
                    _controlPanel.DisplayInfo(house);
                    allowShow = true;
                }
            }

            if (!allowShow)
            {
                _controlPanel.Hide();
            }
        }

        public Monster.Monster[] GetAllMonsterInfo()
        {
            return GameObject.Find("MonsterList").GetComponentsInChildren<Monster.Monster>();
        }

        protected virtual void Start()
        {
        }

        private void GiveEveryoneMoney()
        {
            Debug.Log("發$囉");
            _players.ForEach(p =>
            {
                p.AddMoney(1250);
            });
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _controlPanel.Hide();
            }
        }

        protected void OnApplicationQuit()
        {
            ClearInterval(_godReward);
        }

        protected void OnPlayerHPChanged(int value)
        {
            if (value <= 0)
            {
                GameOver();
            }
        }

        protected void GameOver()
        {
            foreach (var p in _players)
            {
                p.Info.SetLastHP(p.HP);
            }

            SceneManager.LoadScene("LeaderBoard");
        }

        public NumberCollection NumberCollection => _numberCollection;

        public MapGridGenerator MapGridGenerator => _mapGenerator;
        public HouseGenerator HouseGenerator => _mapGenerator.HouseGenerator;
        public MapStatusPanel ControlPanel => _controlPanel;
        public MessageConsole Console => _console;

        #region Check Members
        private void CheckDiff()
        {
            if (this.Manager.Difficulty == Difficulty.Level.None)
                this.Manager.SetDifficult(Difficulty.Level.Demo);
        }

        private void CheckPlayers()
        {
            //Check Player Enter Normaly
            foreach (var p in _players)
            {
                CheckPlayer(p);
            }
        }

        internal static void CheckPlayer(Player.Info p)
        {
            if (p.Status < Player.Info.STATUS.FIGHT)
            {
                var cards = new List<Monster.Info>(p.Manager.MonsterInfoCollection.GetInfoListLessOrEqualToLevel(Difficulty.MAX_LEVEL));
                cards.Sort((a, b) => Random.Range(-1, 2));
                cards.RemoveRange(Manager.REQUIRE_START_CARD_COUNT, cards.Count - Manager.REQUIRE_START_CARD_COUNT);
                var cardsID = new List<int>();

                foreach (var info in cards)
                    cardsID.Add(info.ID);

                p.SetCardIds(cardsID);
                p.SetStatus(Player.Info.STATUS.FIGHT);
            }
        }

        internal static void CheckPlayer(Player.Player p)
        {
            ChooseCardSceneLogic.CheckPlayer(p.Info);

            if (p.Info.Status < Player.Info.STATUS.FIGHT)
            {
                CheckPlayer(p.Info);
                p.AddHonor(Honor.開發者模式);
            }
        }
        #endregion

        #region Initialize Members
        private void InitPlayersByDiff()
        {
            var diff = this.Manager.Difficulty;

            foreach (var p in _players)
                InitPlayer(p, diff);
        }
        private void InitPlayer(Player.Player p, Difficulty.Level level)
        {
            p.Info.ResetCounter();
            int money = 0;

            switch (level)
            {
                case Difficulty.Level.Easy:
                    money = 2000;
                    break;

                case Difficulty.Level.Normal:
                    money = 3000;
                    break;

                case Difficulty.Level.Hard:
                    money = 5000;
                    break;

                case Difficulty.Level.Demo:
                    money = Player.Player.MAX_MONEY;
                    break;
            }

            p.SetMoney(money);
        }
        #endregion
    }
}