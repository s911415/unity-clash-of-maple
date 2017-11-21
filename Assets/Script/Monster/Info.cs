using NTUT.CSIE.GameDev.Game;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Monster
{
    public class Info : CommonObject
    {
        private const string MONSTER_FILE = "Info/Monster/list.json";
        public string id;
        private string  _name, _desc;
        private int _level;
        private static Dictionary<string, Info> _list = null;

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
                return null;
            }
            else
            {
                throw new System.Exception("Monster file list not found");
            }
        }

        private void LoadJsonFile()
        {
        }


        public string ID => id;
        public string Name => _name;
        public string Description => _desc;
        public int Level => _level;
    }
}