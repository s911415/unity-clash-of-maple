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
                _scene.NumberCollection.ShowNumber(GameObject.Find("Player0"), NumberCollection.Type.Blue, 50);
                /*foreach ( var m in GameObject.Find("MonsterTemplate").GetComponentsInChildren<Monster.Monster>())
                {
                    m.Damage(0);
                }*/
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
