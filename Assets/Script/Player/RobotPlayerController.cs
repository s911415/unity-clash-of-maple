using NTUT.CSIE.GameDev.Component;
using NTUT.CSIE.GameDev.Component.Map;
using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using NTUT.CSIE.GameDev.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HouseType =  NTUT.CSIE.GameDev.Component.Map.HouseInfo.HouseType;
using UpgradeType = NTUT.CSIE.GameDev.Component.Map.HouseInfo.UpgradeType;
using NTUT.CSIE.GameDev.Helpers;

namespace NTUT.CSIE.GameDev.Player
{
    [RequireComponent(typeof(Player))]
    public class RobotPlayerController : CommonObject
    {
        private enum RobotOp {BuyHouse, SetMonster, UpgradeAttack, UpgradeHP, UpgradeSpeed, UniAttack, UniProtect}
        private Player _player;
        private FightSceneLogic _scene;
        private float _nextActionTime = 0;
        private Dictionary<RobotOp, float> _operatorProbability = new Dictionary<RobotOp, float>();

        protected RobotPlayerController()
        {
            _operatorProbability.Add(RobotOp.BuyHouse, .15f);
            _operatorProbability.Add(RobotOp.SetMonster, .7f);
            _operatorProbability.Add(RobotOp.UpgradeAttack, .1f);
            _operatorProbability.Add(RobotOp.UpgradeHP, .1f);
            _operatorProbability.Add(RobotOp.UpgradeSpeed, .1f);
            _operatorProbability.Add(RobotOp.UniAttack, .9f);
            _operatorProbability.Add(RobotOp.UniProtect, .7f);
        }

        protected override void Awake()
        {
            base.Awake();
            _player = this.GetComponent<Player>();

            if (_player == null)
                throw new System.Exception("Cannot found Player object");

            _scene = GetSceneLogic<FightSceneLogic>();

            if (_scene == null)
                throw new System.Exception("Cannot found Fight Scene");

            _nextActionTime = 0;
        }

        protected void Start()
        {
            _player.SetMoney(System.Math.Max(_player.Money, 100000000));
        }

        private HouseInfo[] GetMyHouses(HouseType? type = null)
        {
            var q = _scene.HouseGenerator.GetAllHouseInfo().Where(h => h.PlayerID == _player.Info.id);

            if (type != null)
            {
                q = q.Where(h => h.Type == type);
            }

            return q.ToArray();
        }

        private HouseInfo[] GetMySummonHouses()
        {
            return _scene.HouseGenerator.GetAllHouseInfo().Where(h => h.PlayerID == _player.Info.id).ToArray();
        }

        private IReadOnlyList<Point> GetAvailableEmptyGrid()
        {
            var result = new List<Point>();
            const int MIN_COL = 10, MAX_COL = 19;

            for (var r = 0; r < 10; r++)
            {
                for (var c = MIN_COL; c <= MAX_COL; c++)
                {
                    if (_scene.HouseGenerator[r, c] == null)
                        result.Add(new Point(r, c));
                }
            }

            return result;
        }

        protected void Update()
        {
            if (Time.time >= _nextActionTime)
            {
                DoSomeAction();
                SetNextActionTime();
            }
        }

        protected void DoSomeAction()
        {
            var allowOpList = new List<RobotOp>();

            if (Money > 20e3)
                allowOpList.Add(RobotOp.BuyHouse);

            if (GetMyHouses(HouseType.Building).Length > 0)
                allowOpList.Add(RobotOp.SetMonster);

            if (GetMyHouses(HouseType.Summon).Length > 0)
            {
                allowOpList.Add(RobotOp.UpgradeAttack);
                allowOpList.Add(RobotOp.UpgradeHP);
                allowOpList.Add(RobotOp.UpgradeSpeed);
            }

            if (!_player.IsUniqueSkillUsed && _player.HP < (int)(_player.MAX_HP * Config.PLAYER_UNIQUE_REQUIRE_HP))
            {
                allowOpList.Add(RobotOp.UniAttack);
                allowOpList.Add(RobotOp.UniProtect);
            }

            var op = GetRobotOp(allowOpList);
            Debug.Log(string.Format("Robot: {0}", op.ToString()));

            if (op == RobotOp.BuyHouse)
                DoBuyHouse();
            else if (op == RobotOp.SetMonster)
                DoSetMonster();
            else if (op == RobotOp.UpgradeAttack)
                DoUpgradeAttack();
            else if (op == RobotOp.UpgradeHP)
                DoUpgradeHP();
            else if (op == RobotOp.UpgradeSpeed)
                DoUpgradeSpeed();
            else if (op == RobotOp.UniAttack)
                DoUniAttack();
            else if (op == RobotOp.UniProtect)
                DoUniProtect();
        }

        private RobotOp? GetRobotOp(IReadOnlyList<RobotOp> allowOpList)
        {
            var sum = allowOpList.Select(x => _operatorProbability[x]).Sum();
            var p = Random.value * sum;
            float s = 0f;

            for (int i = 0; i < allowOpList.Count; i++)
            {
                var curP = _operatorProbability[allowOpList[i]];

                if (p <= s + curP)
                    return allowOpList[i];

                s += curP;
            }

            Debug.LogError("Null returned");
            return null;
        }

        private void DoBuyHouse()
        {
            var targetHost = new Point(4, 15);
            var list = GetAvailableEmptyGrid();
            // 都買距離敵方主塔最近der
            var distanceList = list.Select(g => g.Distance(targetHost)).ToArray();
            var idx = Helper.GetMinIndex(distanceList);
            _player.BuyHouse(list[idx]);
        }

        private void DoSetMonster()
        {
            var house = Helper.GetRandomElement(GetMyHouses(HouseType.Building));
            int idx = Random.Range(0, 6);
            _player.SetHouseMonster(house.Position, idx);
        }

        private void DoUpgradeAttack()
        {
            var house = Helper.GetRandomElement(GetMyHouses(HouseType.Summon));
            _player.UpgradeHouse(house.Position, UpgradeType.Attack);
        }

        private void DoUpgradeHP()
        {
            var house = Helper.GetRandomElement(GetMyHouses(HouseType.Summon));
            _player.UpgradeHouse(house.Position, UpgradeType.HP);
        }

        private void DoUniAttack()
        {
            try
            {
                _player.DoUniqueSkill(Player.UniqueSkill.Attack);
            }
            catch (System.Exception)
            {
            }
        }

        private void DoUniProtect()
        {
            try
            {
                _player.DoUniqueSkill(Player.UniqueSkill.Defense);
            }
            catch (System.Exception)
            {
            }
        }

        private void DoUpgradeSpeed()
        {
            var house = Helper.GetRandomElement(GetMyHouses(HouseType.Summon));
            _player.UpgradeHouse(house.Position, UpgradeType.Speed);
        }


        private void SetNextActionTime()
        {
            var offset = 1f;

            switch (this.Manager.Difficulty)
            {
                case Difficulty.Level.Easy:
                    offset = 10f;
                    break;

                case Difficulty.Level.Normal:
                    offset = 6f;
                    break;

                case Difficulty.Level.Hard:
                    offset = 2f;
                    break;

                case Difficulty.Level.Demo:
                    offset = 3f;
                    break;
            }

            _nextActionTime = Time.time + offset;
        }

        protected int Money => _player.Money;
    }
}
