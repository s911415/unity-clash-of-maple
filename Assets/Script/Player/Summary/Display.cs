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
using System.Linq;

namespace NTUT.CSIE.GameDev.Player.Summary
{
    public class Display : CommonObject
    {
        [SerializeField]
        protected Text _name, _achievement, _numberOfBuildings, _killMonster, _totalAmount;
        [SerializeField]
        protected GameObject _thisObj;
        [SerializeField]
        protected int _playerID = -1;

        private void Start()
        {
            if (_playerID == -1)
                this.gameObject.SetActive(false);
            else
                SetText();
        }

        private void SetText()
        {
            var playerInfo = this.Manager.GetPlayerAt(_playerID);
            var result = playerInfo.Result;
            _name.text = playerInfo.Name;
            _achievement.text = string.Join(", ", result.Achievement.Select(h => h.Name).ToArray());
            _numberOfBuildings.text = result.BuiltHouse.ToString();
            _killMonster.text = result.TotallyAmountMonster.ToString();
            _totalAmount.text = result.Money.ToString();
        }
    }
}
