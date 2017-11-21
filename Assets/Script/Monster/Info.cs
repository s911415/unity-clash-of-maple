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
        private int level;

        public Info()
        {
            id = name = desc = string.Empty;
            level = 0;
        }

        public string ID => id;
        public string Name => name;
        public string Description => desc;
        public int Level => level;
    }

}