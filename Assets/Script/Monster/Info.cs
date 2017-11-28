using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    [Serializable]
    public class Info
    {
        [SerializeField]
        private string id, name, desc;
        [SerializeField]
        private Difficulty.Level level;
        [SerializeField]
        private int hp, attack, speed, cost, spawnInterval;

        public Info()
        {
            id = name = desc = string.Empty;
            level = 0;
            hp = attack = 1;
        }

        public string ID => id;
        public string Name => name;
        public string Description => desc;
        public Difficulty.Level Level => level;

        public int MaxHP => hp;
        public int Attack => attack;
        public int Speed => speed;
        public int Cost => cost;
        public int SpawnInterval => spawnInterval;
    }

}