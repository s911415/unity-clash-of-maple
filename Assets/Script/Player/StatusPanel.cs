using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using String = System.String;

namespace NTUT.CSIE.GameDev.Player
{
    public class StatusPanel : CommonObject
    {
        [SerializeField]
        protected Player _player;
        [SerializeField]
        protected Image _avatar;
        [SerializeField]
        protected Text _nameText, _hpText, _moneyText, _honorsText;
        [SerializeField]
        protected RectBar _hpBar;

        private void Start()
        {
            _nameText.text = _player.Info.Name;
            BindEvents();
        }

        private void BindEvents()
        {
            _player.OnHPChanged += OnHPChanged;
            _player.OnMoneyChanged += OnMoneyChanged;
            _player.OnTowersChanged += OnTowersChanged;
            _player.OnHonorsChanged += OnHonorsChanged;
            _player.Attached();
        }

        private void Update()
        {
            // Call event from Editor
#if UNITY_EDITOR
            _player.Attached();
#endif
        }

        private void OnHPChanged(int value)
        {
            var text = string.Format("{0}/{1}", value, Player.MAX_HP);
            _hpText.text = text;
            _hpBar.Value = (float)value / (float)Player.MAX_HP;
        }

        private void OnMoneyChanged(int value)
        {
            var text = String.Format("${0:#,##0}", value);
            _moneyText.text = text;
        }

        private void OnTowersChanged(ICollection<Tower> list)
        {
        }

        private void OnHonorsChanged(ICollection<Honors.Honor> list)
        {
            var text = String.Join(System.Environment.NewLine, list);
            _honorsText.text = text;
        }
    }
}
