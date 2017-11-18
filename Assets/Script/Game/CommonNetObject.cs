using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Game
{
    public class CommonNetObject : NetworkBehaviour
    {
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

        public Storage Storage => CommonObject.Storage;

        private void Awake()
        {
            if (this.Manager != null) ;
        }
    }
}