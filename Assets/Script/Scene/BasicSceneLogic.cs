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
    }
}