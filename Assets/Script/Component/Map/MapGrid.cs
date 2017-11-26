using NTUT.CSIE.GameDev.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NTUT.CSIE.GameDev.Component.Map
{
    public class MapGrid : CommonObject
    {
        [SerializeField]
        private TextMesh _gridText;
        [SerializeField]
        private Color _nonSelectColor, _selectColor;
        public int row, col;
        public int width = 10;
        public List<Sprite> buildingImage = new List<Sprite>();
        private Material _mat;
        /*
        0 空地 可選擇建造
        1 房屋 可選擇兵種
        2 生產 可選擇升級
        */
        private int type;
        /*
        房屋資訊
        */
        public int hp;
        public int maxHp;
        public string gridName;
        public MapGridGenerator _generator;
        /*
        兵種編號
        素質
        */
        public string monsterNum;
        public int monsterAttack;
        public int monsterHp;
        public int monsterSpeed;

        protected override void Awake()
        {
            base.Awake();
            _mat = this.GetComponent<Renderer>().material;
        }

        protected void Start()
        {
            this.SetGridInitial();
        }

        private void SetGridInitial()
        {
            type = 0;
            hp = maxHp = 0;
            gridName = "空地";
            _gridText.text = "┌" + ToString() + "┐";
        }

        protected void OnMouseDown()
        {
            if (IsMouseOnGUI) return;

            // Debug.Log(string.Format("Click Grid: ({0}, {1})", row, col));
            _generator.SetHighLight(this.row, this.col);
            _generator.ShowInfoOnPanel();
        }

        public int Type
        {
            set
            {
                type = value;
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = buildingImage[type];

                if (type != 0)
                {
                    hp = maxHp = 5000;
                    gridName = "建築1";
                }
            }

            get
            {
                return type;
            }
        }
        private void Update()
        {
            _gridText.gameObject.SetActive(_generator.ShowGridText);
        }

        public bool Selected
        {
            set
            {
                const string COLOR_TAG = "_Color";

                if (value)
                {
                    _mat.SetColor(COLOR_TAG, _selectColor);
                }
                else
                {
                    _mat.SetColor(COLOR_TAG, _nonSelectColor);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", row, col);
        }
    }
}
