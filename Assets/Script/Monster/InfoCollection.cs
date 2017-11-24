using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class InfoCollection
    {
        private const string MONSTER_FILE = "Info/Monster/list.json";
        private static Dictionary<string, Info> _list = null;
        private Dictionary<int, List<Info>> _diffCardCache = new Dictionary<int, List<Info>>();
        private Dictionary<int, List<Info>> _levelLessOrEqualSetCache = new Dictionary<int, List<Info>>();

        public InfoCollection()
        {
            GetList();
            var a = this[0];
            var b = this["00"];
            Debug.Assert(a == b);
        }

        public List<Info> GetInfoListAtLevel(int level)
        {
            if (_diffCardCache.ContainsKey(level))
                return _diffCardCache[level];

            List<Info> list = new List<Info>();

            foreach (var info in GetList().Values)
                if ((int)info.Level == level) list.Add(info);

            list.Sort((a, b) => a.ID.CompareTo(b.ID));
            _diffCardCache.Add(level, list);
            return list;
        }

        public List<Info> GetInfoListLessOrEqualToLevel(int level)
        {
            if (_levelLessOrEqualSetCache.ContainsKey(level))
                return _levelLessOrEqualSetCache[level];

            List<Info> list = new List<Info>();

            for (int i = Difficulty.MIN_LEVEL; i <= Difficulty.MAX_LEVEL && i <= level; i++)
            {
                list.AddRange(GetInfoListAtLevel(i));
            }

            list.Sort((a, b) => a.ID.CompareTo(b.ID));
            _levelLessOrEqualSetCache.Add(level, list);
            return list;
        }

        private static Dictionary<string, Info> GetList()
        {
            if (_list != null) return _list;

            _list = new Dictionary<string, Info>();
            string filePath = Path.Combine(Application.streamingAssetsPath, MONSTER_FILE);

            if (File.Exists(filePath))
            {
                // Read the json from the file into a string
                string dataAsJson = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                // Pass the json to JsonUtility, and tell it to create a GameData object from it
                var result = JsonHelper.GetJsonArray<Info>(dataAsJson);

                foreach (Info ret in result)
                {
                    _list.Add(ret.ID, ret);
                }

                return _list;
            }
            else
            {
                throw new System.Exception("Monster file list not found");
            }
        }

        public Info this[string id] => GetList()[id];
        public Info this[int id] => this[id.ToString().PadLeft(2, '0')];
    }
}