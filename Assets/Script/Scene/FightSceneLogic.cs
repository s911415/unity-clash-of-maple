using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NTUT.CSIE.GameDev.Scene
{
    public class FightSceneLogic : BasicSceneLogic
    {
        [SerializeField]
        private MapGridGenerator _mapGenerator;

        private void Start()
        {
        }

        public MapGrid this[int r, int c] => _mapGenerator[r, c];
    }
}