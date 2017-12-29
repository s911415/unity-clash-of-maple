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
        private Result _result;

        protected override void Awake()
        {
            base.Awake();
            _status = STATUS.NONE;
            _name = string.Empty;
            _result = null;
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

        public void SetResult(Result result)
        {
            this._result = result;
        }

        public Result Result => _result;
    }
}