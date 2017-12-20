using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Component.Numbers;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Player.Honors;
using NTUT.CSIE.GameDev.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace NTUT.CSIE.GameDev.Player.Summary
{
    public class Display : CommonObject
    {
        [SerializeField]
        protected Text _name, _achievement, _numberOfBuildings, _killMonster, _totalAmount;
        [SerializeField]
        protected GameObject _thisObj;
        private LeaderBoardSceneLogic _scene;

        private void Start()
        {
            var obj = GameObject.FindGameObjectWithTag("SceneLogic");
            _scene = obj.GetComponent<LeaderBoardSceneLogic>();
            SetText();
        }

        private void SetText()
        {
            var playerInfo = _scene.Manager.Players;
            int i = (_thisObj.name == "Composing") ? 0 : 1;

            _name.text = playerInfo[i].Name;
            //_achievement[i].GetComponent<Text>().text = playerInfo[i].Achievement;
            _numberOfBuildings.text = playerInfo[i].BuiltHouseCount.ToString();
            _killMonster.text = playerInfo[1-i].TotallyAmountMonster.ToString();
            //_totalAmount[i].GetComponent<Text>().text=playerInfo[i]

        }
    }
}
