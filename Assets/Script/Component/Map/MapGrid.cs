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
        public MapGridGenerator _generator;
        private HouseInfo _houseInfo;
        /*
        0 空地 可選擇建造
        1 房屋 可選擇兵種
        2 生產 可選擇升級
        */
        private int type;

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
            _gridText.text = "┌" + ToString() + "┐";
            _houseInfo = new HouseInfo();
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
                _houseInfo.SetHouseInfo(type);
            }

            get
            {
                return type;
            }
        }

        public void SelectThisMonster(string Number)
        {
            _houseInfo.monsterNumber = Number;
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

        public void UpdgradeMonster(string item)
        {
            _houseInfo.Upgrade(item);
        }

        public HouseInfo GetInfo()
        {
            return _houseInfo;
        }

        public void DiscardMonster()
        {
            _houseInfo.ResetMonster();
        }
    }
}
