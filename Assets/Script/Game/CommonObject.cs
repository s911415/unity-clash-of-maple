using NTUT.CSIE.GameDev.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Game
{
    public class CommonObject : MonoBehaviour
    {
        private void Awake()
        {
            if (this.Manager != null) { }

            if (this.Storage != null) { }
        }

        private static Manager _manager;
        protected virtual Manager Manager
        {
            get
            {
                if (_manager != null)
                    return _manager;

                var obj = GameObject.Find("GameManager");

                if (obj != null)
                {
                    _manager = obj.GetComponent<Manager>();

                    if (_manager != null)
                        return _manager;
                }

                throw new MissingReferenceException("Cannot found GameManager or Manager instance not found.");
            }
        }

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

        protected T GetSceneLogic<T>()  where T : BasicSceneLogic
        {
            return GameObject.Find("SceneLogic").GetComponent<T>();
        }
    }
}