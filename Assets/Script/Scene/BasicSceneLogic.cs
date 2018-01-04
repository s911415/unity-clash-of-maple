using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Scene
{
    public class BasicSceneLogic : CommonObject
    {
        [SerializeField]
        protected Canvas _window;
        public Canvas Window => _window;
        private KeyCode[] eggKeySeq = { KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.B, KeyCode.A };
        private int eggKeyIdx = 0;

        protected virtual void Update()
        {
        }

        protected void OnGUI()
        {
            Event e = Event.current;

            if (e.rawType == EventType.KeyUp && e.isKey)
            {
                if (eggKeyIdx >= 0)
                {
                    if (e.keyCode == eggKeySeq[eggKeyIdx])
                    {
                        eggKeyIdx++;
                        Debug.Log(eggKeyIdx);

                        if (eggKeyIdx >= eggKeySeq.Length)
                        {
                            eggKeyIdx = -1;
                            LaunchEgg();
                        }
                    }
                    else
                    {
                        eggKeyIdx = 0;
                    }
                }
            }
        }

        protected void LaunchEgg()
        {
            GameObject.Instantiate(Resources.Load("Prefab/Axys"), _window.transform);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}