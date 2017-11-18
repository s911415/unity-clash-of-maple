using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Game
{
    public class KeepAlive : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void Update()
        {
            int a = 0;
            a++;

            foreach (GameObject f in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                Debug.Log(f.name);
            }

            a = 0;
        }
    }
}