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
        private ItemGenerator _itemGenerator;

        [SerializeField]
        private SkillGenerator _skillGenerator;

        [SerializeField]
        private HouseInfo _defaultHouseInfo;

        [SerializeField]
        private NumberCollection _numberCollection;

        [SerializeField]
        protected List<Player.Player> _players;

        [SerializeField]
        protected MessageConsole _console;

        [SerializeField]
        protected AudioSource _audio, _bgmAudio;

        [SerializeField]
        protected AudioClip _defaultBgmClip, _terroristAttackClip;

        [SerializeField]
        protected GameObject _gameResult;
        [SerializeField]
        protected ResultController _gameResultController;
        protected bool _isOver;
        private Dictionary<string, GameObject> _monsterPrefabCache = null;
        [SerializeField]
        private GameObject _menuContainer;

        public Player.Player GetPlayerAt(int i) => _players[i];

        private uint _godReward;

        protected override void Awake()
        {
            base.Awake();
            CheckMembers();
            InitMembers();
            _bgmAudio.clip = _defaultBgmClip;
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
            _godReward = SetInterval(GiveEveryoneMoney, Config.GIVE_MONEY_INTERVAL);
            _isOver = false;

            foreach (var p in _players)
            {
                p.OnDied += GameOver;
            }
        }

        public void ShowMenu()
        {
            _menuContainer.SetActive(true);
            Time.timeScale = 0;
        }

        public void HideMenu()
        {
            _menuContainer.SetActive(false);
            Time.timeScale = 1;
        }

        public void SpawnMonster(Monster.Info monsterInfo, int playerID, HouseInfo houseInfo)
        {
            //var houseHeight = houseInfo.gameObject.GetComponent<>
            var objPrefab = GetMonsterObject(monsterInfo.IDStr);
            var obj = Instantiate(objPrefab, _monsterListObject.transform);
            var mob = obj.GetComponent<Monster.Monster>();
            var offset = (playerID == Manager.DEFAULT_PLAYER_ID) ? 1 : -1;
            mob.SetInfo(playerID, houseInfo.RealHP, houseInfo.RealAttack, houseInfo.RealSpeed).Initialize();
            var newPos = houseInfo.transform.position + offset * new Vector3(4, 0, 0);
            mob.transform.position = newPos;
            var newLocPos = mob.transform.localPosition;
            newLocPos.y = 0;
            newLocPos.z -= 5f;
            mob.transform.localPosition = newLocPos;
            //
            // Bind to Rival's player event
            mob.OnMonsterKilled += _players[1 - playerID].OnRivalMonsterKilled;

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

        private GameObject GetMonsterObject(string mobID)
        {
            if (_monsterPrefabCache == null) _monsterPrefabCache = new Dictionary<string, GameObject>();

            if (!_monsterPrefabCache.ContainsKey(mobID))
            {
                var obj = Resources.Load<GameObject>(string.Format(MONSTER_PREFAB_PATH, mobID));
                _monsterPrefabCache.Add(mobID, obj);
                return obj;
            }

            return _monsterPrefabCache[mobID];
        }

        public void ShowInfoOnPanel(Point p)
        {
            var house = this.HouseGenerator[p] ?? _defaultHouseInfo;
            var allowShow = false;

            if (
                p.Column < 10 ||
                this.Manager.Difficulty == Difficulty.Level.Demo
            )
            {
                _controlPanel.DisplayInfo(house);
                allowShow = true;
            }

            if (!allowShow)
            {
                _controlPanel.Hide();
            }
        }

        public void SetTerroristAttack(bool flag)
        {
            _bgmAudio.Stop();

            if (flag)
            {
                _bgmAudio.Stop();
                _bgmAudio.PlayOneShot(_terroristAttackClip);
            }
            else
            {
                _bgmAudio.clip = _defaultBgmClip;
                _bgmAudio.Play();
            }
        }

        public Monster.Monster[] GetAllMonsterInfo()
        {
            if (!_monsterListObject)
                return new Monster.Monster[0];

            return _monsterListObject.GetComponentsInChildren<Monster.Monster>();
        }

        protected virtual void Start()
        {
        }

        private void GiveEveryoneMoney()
        {
            _console.Show($"來自上天的獎勵，獲得{Config.GIVE_MONEY_AMOUNT}元");
            _players.ForEach(p =>
            {
                p.AddMoney(Config.GIVE_MONEY_AMOUNT);
            });
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (_controlPanel.gameObject.activeSelf)
                {
                    _controlPanel.Hide();
                }
                else
                {
                    if (!_menuContainer.activeSelf)
                    {
                        ShowMenu();
                    }
                    else
                    {
                        HideMenu();
                    }
                }
            }
        }

        public void RestartGame()
        {
            foreach (var player in Manager.Players)
                player.SetStatus(Player.Info.STATUS.SELECTING_CARD);

            SceneManager.LoadScene("ChooseCard");
        }

        protected void OnDestroy()
        {
            ClearInterval(_godReward);
            Time.timeScale = 1;
        }

        protected void GameOver()
        {
            if (_isOver) return;

            _isOver = true;

            foreach (var p in _players)
            {
                p.Info.SetStatus(Player.Info.STATUS.OVER);
                p.Info.SetResult(p.Result);
            }

            //Stop All Monster Action
            foreach (var m in GetAllMonsterInfo()) m.Freeze();

            _gameResult.SetActive(true);
            _bgmAudio.Stop();
            _gameResultController.SetResult(_players[0].Alive ? ResultController.Result.Win : ResultController.Result.Lose);
            _gameResultController.OnAnimationFinished += () => SceneManager.LoadScene("LeaderBoard");
        }

        public NumberCollection NumberCollection => _numberCollection;

        public MapGridGenerator MapGridGenerator => _mapGenerator;
        public HouseGenerator HouseGenerator => _mapGenerator.HouseGenerator;
        public MapStatusPanel ControlPanel => _controlPanel;
        public MessageConsole Console => _console;

        public ItemGenerator ItemGenerator => _itemGenerator;

        public SkillGenerator SkillGenerator => _skillGenerator;

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
            ChooseCardSceneLogic.CheckPlayer(p);

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
                    money = Config.PLAYER_MAX_MONEY;
                    break;
            }

            p.SetMoney(money);
        }
        #endregion
    }
}