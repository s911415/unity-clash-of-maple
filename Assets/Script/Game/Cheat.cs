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

            //For Test
            if (Input.GetKey(KeyCode.F3))
            {
                _scene.GetPlayerAt(1).Damage(Random.Range(150, 1500));
            }

            //For Test
            if (Input.GetKeyDown(KeyCode.F9))
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
                Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)
            )
            {
                var player = _scene.GetPlayerAt(0);

                if (Input.GetKeyUp(KeyCode.H))
                    AddHP(player);

                if (Input.GetKeyUp(KeyCode.R))
                    AddMoney(player);

                if (Input.GetKeyUp(KeyCode.V))
                    ResetAllSpawnCounter();
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
    }
}
