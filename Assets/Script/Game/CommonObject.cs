using NTUT.CSIE.GameDev.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Game
{
    public class CommonObject : MonoBehaviour
    {
        protected virtual void Awake()
        {
            GetManager();
            Random.InitState(System.Guid.NewGuid().GetHashCode());
        }

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

        protected virtual bool IsMouseOnGUI => EventSystem.current.IsPointerOverGameObject();

        /*
        protected static Storage _storage;
        public Storage Storage
        {
            get
            {
                if (_storage != null)
                    return _storage;

                var obj = GameObject.Find("GameStorage");

                if (obj != null)
                {
                    _storage = obj.GetComponent<Storage>(); ;

                    if (_storage != null)
                        return _storage;
                }

                throw new MissingReferenceException("Cannot found GameStorage or Storage instance not found.");
            }
        }
        */

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
    }
}