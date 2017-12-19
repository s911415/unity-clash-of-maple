using NTUT.CSIE.GameDev.Game;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace NTUT.CSIE.GameDev.Scene
{
    public class SplashScreenLogic : CommonObject
    {
        [SerializeField]
        private AudioSource _unityLogoAudioSource, _gameLogoAudioSource;

        protected IEnumerator Start()
        {
            _unityLogoAudioSource.Play();
            _gameLogoAudioSource.PlayDelayed(2.45f);
            SplashScreen.Begin();

            while (!SplashScreen.isFinished)
            {
                SplashScreen.Draw();
                yield return null;
            }

            SceneManager.LoadScene("PrepareUI");
        }
    }
}
