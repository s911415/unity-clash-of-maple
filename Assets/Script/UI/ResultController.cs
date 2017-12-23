using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NTUT.CSIE.GameDev.UI
{
    public class ResultController : CommonObject
    {
        public delegate void ValueChangedEventHandler();
        public event ValueChangedEventHandler OnAnimationFinished;
        public enum Result { None, Win, Lose }
        protected int _result = (int) Result.None;
        [SerializeField]
        protected Animator _ani;
        [SerializeField]
        protected AudioSource _audio;
        [SerializeField]
        protected AudioClip _winClip, _loseClip;

        protected override void Awake()
        {
            base.Awake();
        }

        public void SetResult(Result value)
        {
            if (value != Result.None)
            {
                if (value == Result.Win) _audio.PlayOneShot(_winClip);
                else if (value == Result.Lose) _audio.PlayOneShot(_loseClip);
            }

            _result = (int)value;
        }

        protected void Update()
        {
            _ani.SetInteger("result", _result);
        }

        public void OnAnimationFinish()
        {
            OnAnimationFinished?.Invoke();
        }

    }
}
