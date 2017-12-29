using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using UnityEngine;
using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Component.Message;

namespace NTUT.CSIE.GameDev.Component
{
    public class ItemGenerator : CommonObject
    {
        [SerializeField]
        private Sprite[] _sprites = null;
        [SerializeField]
        private GameObject _itemPrefab = null;
        private FightSceneLogic _scene;

        protected override void Awake()
        {
            base.Awake();
            _scene = GetSceneLogic<FightSceneLogic>();
        }

        public void DropItem(Vector3 position, int itemID, int playerID)
        {
            var obj = Instantiate(_itemPrefab, this.transform);
            var item = obj.GetComponent<Item.Item>();
            position.y = 0;
            obj.transform.localPosition = position;
            item.Init(_sprites[itemID], itemID, playerID);
            // Rival Collect Item
            item.OnCollected += _scene.GetPlayerAt(1 - playerID).OnCollectItem;
        }

        public Item.Item[] AllItems => this.GetComponentsInChildren<Item.Item>();

        public static string GetItemName(int id)
        {
            switch (id)
            {
                case 0:
                    return "金幣";

                case 1:
                    return "橡樹果實";

                case 2:
                    return "衛生紙碎片";

                case 3:
                    return "機器人的燈泡";

                case 5:
                    return "錯誤Log";
            }

            return "";
        }
    }
}
