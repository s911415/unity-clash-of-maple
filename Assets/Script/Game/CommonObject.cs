using NTUT.CSIE.GameDev.Scene;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NTUT.CSIE.GameDev.Game
{
    public class CommonObject : MonoBehaviour
    {
        protected virtual void Awake()
        {
            GetManager();
            Random.InitState(System.Guid.NewGuid().GetHashCode());
        }

        #region manage getter
        private static Manager _manager;
        internal virtual Manager Manager
        {
            get
            {
                var manager = GetManager();

                if (manager != null)
                    return manager;

                throw new MissingReferenceException("Cannot found GameManager or Manager instance not found.");
            }
        }

        private Manager GetManager()
        {
            if (_manager != null)
                return _manager;

            var obj = GameObject.Find("GameManager");

            if (obj != null)
            {
                _manager = obj.GetComponent<Manager>();

                if (_manager != null)
                {
                    if (!_manager.IsInit) _manager.Initialize();

                    return _manager;
                }
            }

            return null;
        }
        #endregion
        protected virtual T GetSceneLogic<T>()  where T : BasicSceneLogic
        {
            var logicObj = GameObject.Find("SceneLogic");

            if (logicObj == null)
                throw new System.Exception("SceneLogic Not Found");

            var component = logicObj.GetComponent<T>();

            if (component == null)
                throw new System.Exception("Target Logic Component Not Attached");

            return component;
        }

        public static int GetMinIndex(float[] array) 
        {
            if (array.Length > 0)
            {
                var minIndex = 0;
                for (int i = 1; i < array.Length; i++)
                    if (array[minIndex] > array[i])
                    {
                        minIndex = i;
                    }

                return minIndex;
            }

            return -1;
        }
        #region Helper
        protected virtual bool IsMouseOnGUI => EventSystem.current.IsPointerOverGameObject();
        protected static Vector3 Clone(Vector3 v) => new Vector3(v.x, v.y, v.z);

        #endregion
    }
}