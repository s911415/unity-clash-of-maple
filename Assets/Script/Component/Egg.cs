using NTUT.CSIE.GameDev.Game;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.Component
{
    public class Egg : CommonObject
    {
        [SerializeField]
        private Text _text = null;
        [SerializeField]
        private Image _image = null;
        private float _startTime = 0;
        private int _curStage = -1;
        private Transform _trans;
        [SerializeField]
        private Sprite[] _sprites;

        private string[] texts =
        {
            "\u6c5d\uff0c\u606d\u559c\u767c\u73fe\u524d\u5f80\u65b0\u4e16\u754c\u7684\u5927\u9580",
            "\u73fe\u5728\u5165\u6559\u9084\u9001\u80a5\u7682\u5594\uff0c\u53ef\u4ee5\u5403\u7684\u5537",
            "\u9019\u9ebc\u597d\u5eb7\u7684\u4e8b\x20\u9084\u4e0d\u8d95\u5feb\u52a0\u5165\u963f\u514b\u897f\u65af\u6559"
        };


        protected override void Awake()
        {
            base.Awake();
            _trans = this.transform;
        }

        private void Start()
        {
            _startTime = Time.time;
        }

        private void Update()
        {
            float passedTime = PassedTime;
            int stage = 0;

            if (passedTime >= 7.5) stage = 4;
            else if (passedTime >= 6) stage = 3;
            else if (passedTime >= 4) stage = 2;
            else  if (passedTime > 2) stage = 1;

            if (stage != _curStage)
            {
                switch (stage)
                {
                    case 0:
                    case 1:
                    case 2:
                        _text.text = texts[stage];
                        _image.sprite = _sprites[stage];
                        break;

                    case 3:
                        break;

                    case 4:
                        LaunchApplyForm();
                        break;
                }

                _curStage = stage;
            }

            if (_curStage == 3)
            {
                _trans.Rotate(new Vector3(0, 0, -25));
                _trans.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            }
        }


        private void LaunchApplyForm()
        {
            Application.OpenURL(Encoding.UTF8.GetString(System.Convert.FromBase64String("aHR0cHM6Ly93ZWItcHJvZ3JhbW1pbmctczE3dS5zOTExNDE1LnRr")));
            Destroy(this.gameObject);
        }

        protected float PassedTime => Time.time - _startTime;
    }
}
