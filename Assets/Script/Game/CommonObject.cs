﻿using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Scene;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Action = System.Action;

namespace NTUT.CSIE.GameDev.Game
{
    public class CommonObject : MonoBehaviour
    {
        protected virtual void Awake()
        {
            GetManager();
            Random.InitState(System.Guid.NewGuid().GetHashCode());
        }

        protected bool InRange(Vector3 v, float range)
        {
            var d = Vector3.Distance(this.transform.localPosition, v);
            return Helper.CompareFloat(d, range) <= 0;
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


        protected virtual bool IsMouseOnGUI => EventSystem.current.IsPointerOverGameObject();

        protected uint SetTimeout(Action action, uint timeoutMS)
        {
            if (Manager)
                return Manager._SetTimeout(action, timeoutMS);
            else
                return 0;
        }

        protected uint SetInterval(Action action, uint timeoutMS)
        {
            if (Manager)
                return Manager._SetInterval(action, timeoutMS);
            else
                return 0;
        }

        protected void ClearTimeout(uint timerId)
        {
            try
            {
                Manager._ClearTimeout(timerId);
            }
            catch (System.Exception)
            {
            }
        }

        protected void ClearInterval(uint timerId)
        {
            try
            {
                Manager._ClearTimeout(timerId);
            }
            catch (System.Exception)
            {
            }
        }
    }
}