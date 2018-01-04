using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Numbers;
using NTUT.CSIE.GameDev.Scene;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NTUT.CSIE.GameDev.Game
{
    public class Cheat : CommonObject
    {
        private FightSceneLogic _scene;

        private void Start()
        {
            _scene = null;
            var obj = GameObject.FindGameObjectWithTag("SceneLogic");

            if (obj != null)
            {
                _scene = obj.GetComponent<FightSceneLogic>();
            }
        }

        protected void Update()
        {
            if (_scene == null) return;

            bool CTRL_KEY = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool ALT_KEY = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            //For Test
            if (Input.GetKeyUp(KeyCode.F3))
            {
                _scene.GetPlayerAt(1).Damage(Random.Range(100, 15000));
                _scene.Console.Show("God");
            }

            //For Test
            if (Input.GetKey(KeyCode.F9))
            {
                var house = _scene.HouseGenerator[_scene.MapGridGenerator.CurPoint];

                if (house && house.Type == Component.Map.HouseInfo.HouseType.Summon)
                {
                    const int MAX_VALUE = 1 << 30;
                    house.UpgradeAttribute(Component.Map.HouseInfo.UpgradeType.Attack, MAX_VALUE);
                    house.UpgradeAttribute(Component.Map.HouseInfo.UpgradeType.HP, MAX_VALUE);
                    house.UpgradeAttribute(Component.Map.HouseInfo.UpgradeType.Speed, 5);
                }
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                _scene.GetPlayerAt(0).IsGodMode = !_scene.GetPlayerAt(0).IsGodMode;
                Debug.Log("God Mode: " + _scene.GetPlayerAt(0).IsGodMode);
            }

            if (
                (CTRL_KEY && ALT_KEY) &&
                Input.GetKeyDown(KeyCode.End)
            )
            {
                var pos = Camera.main.transform.localPosition;
                pos.y = 0;
                _scene.SkillGenerator.UseSkill(pos, 2, -1);
            }

            if (CTRL_KEY)
            {
                var player = _scene.GetPlayerAt(0);

                if (Input.GetKeyUp(KeyCode.H))
                    AddHP(player);

                if (Input.GetKeyUp(KeyCode.R))
                    AddMoney(player);

                if (Input.GetKeyUp(KeyCode.V))
                    ResetAllSpawnCounter();

                if (Input.GetKeyUp(KeyCode.A))
                    BuildHouseAtAllAvailableSpace(player);
            }
        }

        private void AddHP(Player.Player p)
        {
            p.Recovery(5000);
        }

        private void AddMoney(Player.Player p)
        {
            p.AddMoney(5000);
        }

        private void ResetAllSpawnCounter()
        {
            foreach (var info in _scene.HouseGenerator.GetAllHouseInfo())
            {
                info._ResetSpawnCounter();
            }
        }

        private void BuildHouseAtAllAvailableSpace(Player.Player player)
        {
            foreach (var p in GetAvailableEmptyGrid())
            {
                player.BuyHouse(p, true);
            }
        }

        private IReadOnlyList<Point> GetAvailableEmptyGrid()
        {
            var result = new List<Point>();
            const int MIN_COL = 0, MAX_COL = 19;

            for (var r = 0; r < 10; r++)
            {
                for (var c = MIN_COL; c <= MAX_COL; c++)
                {
                    if (_scene.HouseGenerator[r, c] == null)
                        result.Add(new Point(r, c));
                }
            }

            return result;
        }
    }
}
