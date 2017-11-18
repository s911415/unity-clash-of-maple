using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NTUT.CSIE.GameDev.Scene
{
    public class PrepareUIScene : BasicSceneLogic
    {
        internal void OnPlayerNumberChanged()
        {
            if (ReadyPlayerNumber == 2)
            {
                Debug.Log("All Ready");
                SceneManager.LoadScene("Fight");
            }
        }

        protected int ReadyPlayerNumber
        {
            get
            {
                return Array.FindAll(Manager._playerList, p => p.Status == Player.Info.STATUS.READY).Length;
            }
        }
    }
}