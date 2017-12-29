using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Helpers;
using NTUT.CSIE.GameDev.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NTUT.CSIE.GameDev.Skills
{
    public class Skill : CommonObject
    {
        [SerializeField]
        protected int _playerID;
        protected int _skillID;
        protected SpriteRenderer _render;
        protected AudioSource _audio = null;
        protected FightSceneLogic _scene;
        [SerializeField]
        protected AudioClip _hitClip = null, _startClip = null;


        protected override void Awake()
        {
            base.Awake();
            _audio = GetComponent<AudioSource>();
            _render = GetComponentInChildren<SpriteRenderer>();
            _scene = GetSceneLogic<FightSceneLogic>();
        }

        public void Init(int skillID, int pid)
        {
            this._skillID = skillID;
            this._playerID = pid;
            this.name = $"Skill #{skillID}";
            _render.flipX = this._playerID == 0;
        }

        protected Monster.Monster[] GetNearMobs(float range)
        {
            return  _scene
                    .GetAllMonsterInfo()
                    .Where(m => m.PlayerID != _playerID && InRange(m.transform.localPosition, range))
                    .ToArray();
        }

        public int SkillID => _skillID;
        public int PlayerID => _playerID;

        public virtual void SkillStart()
        {
            if (_startClip)
                _audio.PlayOneShot(_startClip);
        }

        public virtual void SkillFinish()
        {
            Destroy(this.gameObject);
        }

        public virtual void SkillTime()
        {
        }
    }
}
