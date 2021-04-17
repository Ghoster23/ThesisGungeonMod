using UnityEngine;
using EnemyAPI;

namespace DynamicRandomness
{
    public enum BattleStates
    {
        Balanced,
        Winning,
        Losing
    }

    class DynamicAttackBehaviourGroup : AttackBehaviorGroup, IAttackBehaviorGroup
    {
        protected BattleStates _battleState = BattleStates.Balanced;

        private HealthHaver _playerHealth;


        public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
        {
            base.Init(gameObject, aiActor, aiShooter);

            _playerHealth = GameManager.Instance.PrimaryPlayer.healthHaver;
        }

        public override BehaviorResult Update()
        {
            this.UpdateBattleState();

            var result = base.Update();

            //ETGModConsole.Log("<color=#7851A9>Current attack behaviour: " + CurrentBehavior + "</color>", true);

            return result;
        }

        
        protected void UpdateBattleState()
        {
            float healthPercent = _playerHealth.GetCurrentHealthPercentage();

            float bossHealthPercent = m_aiActor.healthHaver.GetCurrentHealthPercentage();

            float absDiff = Mathf.Abs(healthPercent - bossHealthPercent);

            /*
            ETGModConsole.Log("<color=#7851A9>Battle Status -></color>");
            ETGModConsole.Log("<color=#7851A9>  Player: " + healthPercent + "</color>");
            ETGModConsole.Log("<color=#7851A9>  Boss: " + bossHealthPercent + "</color>");
            ETGModConsole.Log("<color=#7851A9>  Diff: " + absDiff + "</color>");
            */

            if (absDiff < 0.10f)
            {
                _battleState = BattleStates.Balanced;
                this.ActivateBalancedState();
            }
            else if (bossHealthPercent < healthPercent)
            {
                _battleState = BattleStates.Losing;
                this.ActivateLosingState();
            }
            else
            {
                _battleState = BattleStates.Winning;
                this.ActivateWinningState();
            }

            ETGModConsole.Log("<color=#7851A9>Current Boss Battle Pattern -> " + _battleState + "</color>");
        }
        

        protected virtual void ActivateBalancedState()
        {
            throw new System.NotImplementedException();
        }

        protected virtual void ActivateWinningState()
        {
            throw new System.NotImplementedException();
        }

        protected virtual void ActivateLosingState()
        {
            throw new System.NotImplementedException();
        }


        public BattleStates GetBattleState()
        {
            return _battleState;
        }
    }
}
