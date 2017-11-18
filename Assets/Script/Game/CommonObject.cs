using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Game
{
    public class CommonObject : MonoBehaviour
    {
        protected static Storage _storage;
        public static Storage Storage
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
    }
}