using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Player.Honors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Player
{
    public class Info : CommonObject
    {
        public enum STATUS
        {
            NONE, CONNECTED, READY, SELECTING_CARD, FIGHT, OVER
        }

        public int id;
        [SerializeField]
        private string _name;
        public string Name => _name;
        [SerializeField]
        private STATUS _status;
        public STATUS Status => _status;
        [SerializeField]
        private List<int> _cardIds = new List<int>();
        [SerializeField]
        private Dictionary<int, int> _killedMonsterCount;
        [SerializeField]
        private int _houseDestroyedCount, _builtHouseCount, _lastHP;
        [SerializeField]
        private List<Honor> _achievement;

        protected override void Awake()
        {
            base.Awake();
            _status = STATUS.NONE;
            _name = string.Empty;
        }

        internal Info SetName(string name)
        {
            this._name = name;
            return this;
        }

        internal Info SetStatus(STATUS status)
        {
            this._status = status;
            return this;
        }

        public Info SetCardIds(IEnumerable<int> list)
        {
            _cardIds.Clear();
            _cardIds.AddRange(list);
            return this;
        }

        public IReadOnlyList<int> GetCardIds()
        {
            return _cardIds;
        }

        internal string GetStatusString()
        {
            switch (_status)
            {
                case STATUS.NONE:
                    return "等待加入";

                case STATUS.READY:
                    return "Ready";
            }

            return "";
        }

        public void AddMonsterKillCount(int mobID)
        {
            _killedMonsterCount[mobID]++;
        }

        public void AddHouseDestroyedCount()
        {
            _houseDestroyedCount++;
        }

        public void AddBuiltHouseCount()
        {
            _builtHouseCount++;
        }

        public void ResetCounter()
        {
            _killedMonsterCount = new Dictionary<int, int>();
            _houseDestroyedCount = 0;
            _builtHouseCount = 0;
            _lastHP = 0;

            foreach (var m in this.Manager.MonsterInfoCollection.GetAllMonsterId())
            {
                _killedMonsterCount.Add(m, 0);
            }
        }

        private int GetTotallyAmountMonster()
        {
            int amount=0;
            foreach(var m in this.Manager.MonsterInfoCollection.GetAllMonsterId())
            {
                amount += _killedMonsterCount[m];
            }
            return amount;
        }

        public void SetLastHP(int value)
        {
            _lastHP = value;
        }

        public int LastHP => _lastHP;

        public IReadOnlyDictionary<int, int> KilledMonsterCount => _killedMonsterCount;
        public int TotallyAmountMonster => GetTotallyAmountMonster();
        public int HouseDestroyedCount => _houseDestroyedCount;
        public int BuiltHouseCount => _builtHouseCount;
        public List<Honor> Achievement => _achievement;
    }
}