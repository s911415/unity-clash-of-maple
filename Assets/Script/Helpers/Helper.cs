using UnityEngine;
using System.Linq;
using Array = System.Array;

namespace NTUT.CSIE.GameDev.Helpers
{
    public sealed class Helper
    {
        public static int GetMinIndex(float[] array)
        {
            if (array.Length > 0)
            {
                return Array.IndexOf(array, array.Min());
            }

            return -1;
        }

        public static bool IsMouseOnGUI => UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        public static Vector3 Clone(Vector3 v) => new Vector3(v.x, v.y, v.z);

    }
}
