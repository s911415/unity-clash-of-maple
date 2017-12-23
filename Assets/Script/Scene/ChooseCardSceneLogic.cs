using NTUT.CSIE.GameDev.CardSelect;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.Scene
{
    public class ChooseCardSceneLogic : BasicSceneLogic
    {
        private List<string> _selectCardSet;
        private GameObject _difficultBtn;
        [SerializeField]
        private GridLayoutGroup _gridLayout;
        [SerializeField]
        private GameObject _cardBag;
        [SerializeField]
        private GameObject _cardContainer;
        private bool _prepared = false, _callScene;
        private float _prepareTime;

        // Use this for initialization
        private int _difficult;
        private List<int> _cardList = new List<int>();
        private List<GameObject> _cardObj;
        private List<Vector3> _cardObjStartPos;
        private const float COLLECT_CARD_WAIT = 0.4f;
        private const float COLLECT_CARD_TIME = 1f;
        private const float COLLECT_CARD_OFFSET_TIME = .125f;
        private readonly Vector3 destPos = new Vector3(0, -70, 0);
        private readonly KeyCode[] SHORTCUT_KEY =
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
            KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0,
            KeyCode.Minus, KeyCode.Equals
        };


        protected override void Awake()
        {
            base.Awake();
            CheckPlayers();

            foreach (var player in Manager.Players)
            {
                player.SetStatus(Player.Info.STATUS.SELECTING_CARD);
            }
        }

        protected void Start()
        {
            _prepareTime = 0;
            _prepared = false;
            _callScene = false;
        }

        public void SetPlayerInfoAndGameDiff(int difficult, IReadOnlyList<int>cardList)
        {
            _difficult = difficult;
            _cardList.AddRange(cardList);
        }

        public void OnClickStartButton()
        {
            Manager.SetDifficult((Difficulty.Level)_difficult);
            Manager.GetPlayerAt(Manager.DEFAULT_PLAYER_ID).SetCardIds(_cardList);
            Manager.GetPlayerAt(Manager.ROBOT_PLAYER_ID).SetCardIds(SelectCardRandomly());
            _gridLayout.enabled = false;
            _cardBag.gameObject.SetActive(true);

            foreach (var player in Manager.Players)
                player.SetStatus(Player.Info.STATUS.FIGHT);

            _prepared = true;
            _prepareTime = Time.time;
            _cardObj = new List<GameObject>();
            _cardObjStartPos = new List<Vector3>();

            foreach (Transform t in _cardContainer.transform)
            {
                Select s = t.GetComponent<Select>();

                if (s.selected)
                {
                    _cardObj.Add(t.gameObject);
                    _cardObjStartPos.Add(t.localPosition);
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            float curTime = Time.time;

            for (int i = 0, j = _cardContainer.transform.childCount; i < j && i < SHORTCUT_KEY.Length; i++)
            {
                var k = SHORTCUT_KEY[i];
                var cardObjT = _cardContainer.transform.GetChild(i);

                if (Input.GetKeyDown(k) && cardObjT)
                {
                    cardObjT.gameObject.GetComponent<Button>().onClick.Invoke();
                }
            }

            if (_prepared)
            {
                int cardObjCount = _cardObj.Count;
                float[] timeCheckPoint =
                {
                    _prepareTime + COLLECT_CARD_WAIT,
                    _prepareTime + COLLECT_CARD_WAIT +
                    + COLLECT_CARD_OFFSET_TIME* cardObjCount + COLLECT_CARD_TIME
                    + 2 //緩衝用
                };

                if (curTime <= timeCheckPoint[0])
                {
                    //Do Nothing
                }
                else if (curTime <= timeCheckPoint[1])
                {
                    for (int i = 0; i < cardObjCount; i++)
                    {
                        float diffTime = COLLECT_CARD_OFFSET_TIME * i;
                        float startTime = _prepareTime + COLLECT_CARD_TIME + diffTime;
                        float endTime = startTime + COLLECT_CARD_TIME;
                        float t = Mathf.InverseLerp(startTime, endTime, curTime);

                        if (t <= 0) t = 0;
                        else if (t >= 1) t = 1;

                        Debug.Log($"{i}: {t}, endTime: {endTime}, currTime: {curTime}");
                        _cardObj[i].transform.localPosition = Vector3.Lerp(_cardObjStartPos[i], destPos, t);
                    }
                }
                else if (!_callScene && curTime > timeCheckPoint[1])
                {
                    _callScene = true;
                    SceneManager.LoadScene("Fight");
                }
            }
        }

        private void CheckPlayers()
        {
            // Check Player Enter Normaly
            foreach (var p in Manager.Players)
            {
                CheckPlayer(p);
            }
        }

        internal static void CheckPlayer(Player.Info p)
        {
            if (p.Status < Player.Info.STATUS.READY)
            {
                p.SetName((p.id == 0) ? "Player0" : "Robot");
            }
        }


        private IEnumerable<int> SelectCardRandomly()
        {
            var list = new HashSet<int>();
            var set = Manager.MonsterInfoCollection.GetInfoListLessOrEqualToLevel(_difficult);

            while (list.Count < Manager.REQUIRE_START_CARD_COUNT)
            {
                var idx = Random.Range(0, set.Count);

                list.Add(set[idx].ID);
            }

            return list;
        }

    }
}