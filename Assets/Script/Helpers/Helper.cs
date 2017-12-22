using UnityEngine;
using System.Linq;
using Array = System.Array;
using System.Collections.Generic;

namespace NTUT.CSIE.GameDev.Helpers
{
    public sealed class Helper
    {
        public static int GetMinIndex<T>(T[] array) where T : System.IComparable
        {
            if (array.Length > 0)
            {
                return Array.IndexOf(array, array.Min());
            }

            return -1;
        }

        public static T GetRandomElement<T>(IReadOnlyList<T> list)
        {
            var idx = Random.Range(0, list.Count);
            return list[idx];
        }

        public static bool IsMouseOnGUI => UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        public static Vector3 Clone(Vector3 v) => new Vector3(v.x, v.y, v.z);

        public static int CompareFloat(float a, float b)
        {
            const float RATE = 10f;
            float diff = (a * RATE) - (b * RATE);
            diff = Mathf.Floor(diff);

            if (diff == +0.0f || diff == -0.0f) return 0;

            if (diff < 0) return -1;

            return 1;
        }

    }
}
