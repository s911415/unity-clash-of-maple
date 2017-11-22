using NTUT.CSIE.GameDev.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Player
{
    public class Info : CommonObject
    {
        public enum STATUS
        {
            NONE, CONNECTED, READY
        }

        public int id;

        private string _name;
        public string Name => _name;

        private STATUS _status;
        public STATUS Status => _status;

        private List<string> _cardIds = new List<string>();

        private void Start()
        {
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

        public void SetCardIds(List<string> list)
        {
            _cardIds.AddRange(list);
        }

        public List<string> GetCardIds()
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
    }
}