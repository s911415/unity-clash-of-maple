using NTUT.CSIE.GameDev.Component.Numbers;
using NTUT.CSIE.GameDev.Scene;
using System.Collections;
using System.Collections.Generic;
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
            if (Input.GetKeyUp(KeyCode.F3))
            {
                //
                _scene.NumberCollection.ShowNumber(GameObject.Find("Monster_03"), NumberCollection.Type.Violet, 4567);
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
