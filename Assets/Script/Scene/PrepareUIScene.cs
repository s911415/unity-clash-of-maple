using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.Scene
{
    public class PrepareUIScene : BasicSceneLogic
    {
        internal void OnPlayerNumberChanged()
        {
            if (ReadyPlayerNumber == 1)
            {
                Debug.Log("All Ready");
                SceneManager.LoadScene("ChooseCard");
            }
        }

        public void OnReadyButtonClick()
        {
            var playerName = GameObject.Find("PlayerNameInput").GetComponent<InputField>().text.Trim();

            if (playerName != "")
            {
                Manager.GetPlayerAt(0).SetName(playerName).SetStatus(Player.Info.STATUS.READY);
                OnPlayerNumberChanged();
            }
            else
            {
                new DialogBuilder()
                .SetTitle("錯誤")
                .SetContent("名稱不可以為空")
                .SetIcon(Dialog.Icon.Error)
                .SetYesBtnStatus(true)
                .SetClickListener(
                    new Dialog.MessageDialogEventListener(
                        () => Debug.Log("你點了確定")
                    )
                )
                .Show(_window);
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