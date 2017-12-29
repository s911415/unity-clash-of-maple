using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Item
{
    public class Item : HurtableObject
    {
        public delegate void ItemCollectedEventHandler(int itemID);
        [SerializeField]
        protected SpriteRenderer _render;
        [SerializeField]
        protected int _playerID;
        protected int _itemID;
        public ItemCollectedEventHandler OnCollected;
        private uint _cleanUpTimer = 0;

        public void Init(Sprite sp, int itemID, int pid)
        {
            Render.sprite = sp;
            this._itemID = itemID;
            this._playerID = pid;
            this.name = $"Item #{itemID}";
            _cleanUpTimer = SetTimeout(() => Destroy(this.gameObject), Config.ITEM_CLEAN_UP_TIME);
        }

        public override void Damage(int damage)
        {
            // Destroy(this.gameObject);
        }

        public void Collect()
        {
            OnCollected?.Invoke(this._itemID);
            Destroy(this.gameObject);
        }

        public override void Recovery(int recover)
        {
        }

        protected void OnDestroy()
        {
            ClearTimeout(_cleanUpTimer);
        }

        public int ItemID => _itemID;
        public int PlayerID => _playerID;

        public SpriteRenderer Render
        {
            get
            {
                if (_render == null)
                    _render = GetComponentInChildren<SpriteRenderer>();

                return _render;
            }
        }

        public override int MAX_HP
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool Alive => !!this;

        public override int HP => 1;
    }
}
