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

        public Player.Player GetPlayerAt(int i) => _players[i];

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
        }

        public void SpawnMonster(int monsterID, int playerID, HouseInfo houseInfo)
        {
            SpawnMonster(string.Format("{0:2,00}", monsterID), playerID, houseInfo);
        }

        public void SpawnMonster(string monsterID, int playerID, HouseInfo houseInfo)
        {
            //var houseHeight = houseInfo.gameObject.GetComponent<>
            var objPrefab = Resources.Load<GameObject>(string.Format(MONSTER_PREFAB_PATH, monsterID));
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
            _controlPanel.DisplayInfo(this.HouseGenerator[p] ?? _defaultHouseInfo);
        }

        public Monster.Monster[] GetAllMonsterInfo()
        {
            return GameObject.Find("MonsterList").GetComponentsInChildren<Monster.Monster>();
        }

        protected virtual void Start()
        {
            //定時給錢
            SetInterval(GiveEveryoneMoney, 60 * 1000);
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

        public NumberCollection NumberCollection => _numberCollection;

        public MapGridGenerator MapGridGenerator => _mapGenerator;
        public HouseGenerator HouseGenerator => _mapGenerator.HouseGenerator;
        public MapStatusPanel ControlPanel => _controlPanel;

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

        internal static void CheckPlayer(Player.Player p)
        {
            ChooseCardSceneLogic.CheckPlayer(p.Info);

            if (p.Info.Status < Player.Info.STATUS.FIGHT)
            {
                var cards = new List<Monster.Info>(p.Manager.MonsterInfoCollection.GetInfoListLessOrEqualToLevel(Difficulty.MAX_LEVEL));
                cards.Sort((a, b) => Random.Range(-1, 2));
                cards.RemoveRange(Manager.REQUIRE_START_CARD_COUNT, cards.Count - Manager.REQUIRE_START_CARD_COUNT);
                var cardsID = new List<string>();

                foreach (var info in cards)
                    cardsID.Add(info.ID);

                p.Info.SetCardIds(cardsID);
                p.Info.SetStatus(Player.Info.STATUS.FIGHT);
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