using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NTUT.CSIE.GameDev.Player.Honors;

namespace NTUT.CSIE.GameDev.Game
{
    [Serializable]
    public class Result
    {
        private readonly int _playerID, _money, _hp, _builtHouse, _destroyedHouse;
        private readonly IReadOnlyDictionary<int, int> _killedMonster;
        private readonly IReadOnlyCollection<Player.Honors.Honor> _achievement;

        public Result(int playerID, int money, int hp, int builtHouse, int destroyedHouse, IReadOnlyDictionary<int, int> killedMonster, IReadOnlyCollection<Player.Honors.Honor> achievement)
        {
            _playerID = playerID;
            _money = money;
            _hp = hp;
            _builtHouse = builtHouse;
            _destroyedHouse = destroyedHouse;
            var tmpInfo = new Dictionary<int, int>();
            var tmpAchievement = new List<Player.Honors.Honor>();

            foreach (var d in killedMonster)
                tmpInfo.Add(d.Key, d.Value);

            tmpAchievement.AddRange(achievement);
            _killedMonster = tmpInfo;
            _achievement = tmpAchievement;
        }

        public int PlayerID => _playerID;
        public int Money => _money;
        public int HP => _hp;
        public int BuiltHouse => _builtHouse;
        public int DestroyedHouse => _destroyedHouse;
        public IReadOnlyCollection<Honor> Achievement => _achievement;
        public int TotallyAmountMonster
        {
            get
            {
                int amount = 0;

                foreach (var m in _killedMonster)
                {
                    amount += m.Value;
                }

                return amount;
            }
        }
    }
}
