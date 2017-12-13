using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
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
        private float _nonSelectAlpha = 1f, _selectAlpha = .8f;
        [SerializeField]
        public Point _position;
        public int width = 10;
        //public List<Sprite> buildingImage = new List<Sprite>();

        private Material _mat;
        public MapGridGenerator _generator;

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
            _gridText.text = "┌" + ToString() + "┐";
        }

        protected void OnMouseDown()
        {
            if (IsMouseOnGUI) return;

            //Debug.Log(string.Format("Click Grid: ({0}, {1})", row, col));
            _generator.SetHighLight(this._position);
            GetSceneLogic<FightSceneLogic>().ShowInfoOnPanel(this._position);
        }

        private void Update()
        {
            _gridText.gameObject.SetActive(_generator.ShowGridText);
        }

        public bool Selected
        {
            set
            {
                const string TRANS_TAG = "_BaseTransVal";

                if (value)
                {
                    _mat.SetFloat(TRANS_TAG, _selectAlpha);
                }
                else
                {
                    _mat.SetFloat(TRANS_TAG, _nonSelectAlpha);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", _position.Row, _position.Column);
        }

        public Point Position => _position;

        public void SetPosition(int row, int col)
        {
            _position = new Point(row, col);
        }
    }
}
