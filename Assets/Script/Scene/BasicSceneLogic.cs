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

        protected virtual void Update()
        {
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