using NTUT.CSIE.GameDev.Game;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.Component.Numbers
{
    public class NumberCollection : CommonObject
    {
        public Sprite[] blueNumberSprits = new Sprite[10];
        public Sprite[] redNumberSprits = new Sprite[11];
        public Sprite[] violetNumberSprits = new Sprite[11];
        private readonly static int[] NumbersWidth = { 31, 22, 29, 28, 31, 29, 31, 29, 31, 31, 98 };
        private readonly static int[] NumbersHeight = { 33, 32, 33, 32, 33, 32, 33, 32, 33, 33, 38 };
        private const int MISS_IDX = 10;
        private const int NUMBERS_MAX_COUNT = 256;
        private int _numberCount = 0;
        private Camera _mainCamera;

        [SerializeField]
        private GameObject _numberContainer = null;
        [SerializeField]
        private GameObject _numberTemplate = null;

        public enum Type
        {
            Blue, Red, Violet
        }

        protected void Start()
        {
            _mainCamera = Camera.main;
        }

        public void ShowNumber(GameObject target, Type type, uint number)
        {
            if (!target || _numberCount > NUMBERS_MAX_COUNT) return;

            if (_numberCount > NUMBERS_MAX_COUNT / 2 && Random.value < .6f) return;

            if (_numberCount > 3 * NUMBERS_MAX_COUNT / 4 && Random.value < .7f) return;

            if (_numberCount > 4 * NUMBERS_MAX_COUNT / 5 && Random.value < .85f) return;

            if (_numberCount > 5 * NUMBERS_MAX_COUNT / 6 && Random.value < .9f) return;

            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);

                if (!GeometryUtility.TestPlanesAABB(planes, new Bounds(target.transform.position, new Vector3(1, 1, 1))))
                {
                    return;
                }
            }
            const int NUMBER_OFFSET = -5;
            const int MARGIN_WIDTH = -5;
            int[] numbers = number.ToString().Select(n => (int)System.Char.GetNumericValue(n)).ToArray<int>();
            Sprite[] sprites = GetSpriteArray(type);

            if (number == 0  && sprites.Length > MISS_IDX)
            {
                numbers = new int[] { MISS_IDX };
            }

            int totalWidth = numbers.Select(x => NumbersWidth[x]).Sum() - MARGIN_WIDTH * (numbers.Length - 1);
            GameObject[] go = new GameObject[numbers.Length];
            var container = Instantiate(_numberContainer, this.gameObject.transform);
            var containerRectT = container.GetComponent<RectTransform>();
            var sr = target.GetComponentInChildren<SpriteRenderer>() ?? target.GetComponent<SpriteRenderer>();
            var targetHeight = sr != null ? sr.size.y : 0;
            var nContainer = container.GetComponent<NumberContainer>();
            nContainer.SetTargetPosition(
                target.transform.position + new Vector3(0, 0, targetHeight)
            );
            _numberCount++;
            nContainer.OnNumbersDisappear += () => _numberCount--;
            containerRectT.sizeDelta = new Vector2(totalWidth, containerRectT.sizeDelta.y);
            container.name = number.ToString();

            for (
                int i = 0, posX = 0, halfWidth = totalWidth >> 1;
                i < numbers.Length;
                i++
            )
            {
                var num = numbers[i];
                var o = Object.Instantiate(_numberTemplate, container.transform);
                o.name = num.ToString();
                o.GetComponent<Image>().sprite = sprites[num];
                var t = o.GetComponent<RectTransform>();
                t.sizeDelta = new Vector2(NumbersWidth[num], NumbersHeight[num]);
                t.localPosition = new Vector2(
                    posX - halfWidth,
                    (i & 1) == 0 ? 0 : NUMBER_OFFSET
                );
                posX += NumbersWidth[num] + MARGIN_WIDTH;
                go[i] = o;
            }
        }

        private Sprite[] GetSpriteArray(Type t)
        {
            switch (t)
            {
                case Type.Blue:
                    return blueNumberSprits;

                case Type.Red:
                    return redNumberSprits;

                case Type.Violet:
                    return violetNumberSprits;
            }

            return null;
        }
    }
}
