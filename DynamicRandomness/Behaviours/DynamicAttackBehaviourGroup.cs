using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace DynamicRandomness.Behaviours
{
    public enum BattleStates
    {
        Balanced,
        Winning,
        Losing
    }

    class DynamicAttackBehaviourGroup : AttackBehaviorGroup, IAttackBehaviorGroup
    {
        protected BattleStates m_battleState = BattleStates.Balanced;

        private HealthHaver m_playerHealth;

        private int m_sequenceIndex;

        protected List<int> AttackSequence;

        protected AttackBehaviorBase m_currentBehavior
        {
            get
            {
                if (m_battleState == BattleStates.Winning)
                    return this.AttackBehaviors[this.AttackSequence[m_sequenceIndex]].Behavior;
                else
                    return this.CurrentBehavior;
            }
        }


        public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
        {
            base.Init(gameObject, aiActor, aiShooter);

            m_playerHealth = GameManager.Instance.PrimaryPlayer.healthHaver;
        }

        public override BehaviorResult Update()
        {
            var result = BehaviorResult.SkipAllRemainingBehaviors;

            switch(m_battleState)
            {
                default:
                    if(Module.Debug) Module.Log("Unrecognized battle state -> " + m_battleState);
                    break;

                case BattleStates.Losing:
                case BattleStates.Balanced:
                    result = base.Update();
                    break;

                case BattleStates.Winning:
                    result = this.SequentialUpdate();
                    break;

            }

            return result;
        }

        
        protected void UpdateBattleState()
        {
            float healthPercent = m_playerHealth.GetCurrentHealthPercentage();

            float bossHealthPercent = m_aiActor.healthHaver.GetCurrentHealthPercentage();

            float absDiff = Mathf.Abs(healthPercent - bossHealthPercent);

            if (absDiff < 0.10f)
            {
                if (m_battleState == BattleStates.Balanced) return;

                m_battleState = BattleStates.Balanced;
                this.ActivateBalancedState();
            }
            else if (bossHealthPercent < healthPercent)
            {
                if (m_battleState == BattleStates.Losing) return;

                m_battleState = BattleStates.Losing;
                this.ActivateLosingState();
            }
            else
            {
                if (m_battleState == BattleStates.Winning) return;

                m_battleState = BattleStates.Winning;
                m_sequenceIndex = 0;
                this.ActivateWinningState();
            }
        }
        

        protected BehaviorResult SequentialUpdate()
        {
            if (this.m_sequenceIndex >= this.AttackSequence.Count) this.m_sequenceIndex = 0;

            this.m_sequenceIndex = (this.m_sequenceIndex + 1) % this.AttackSequence.Count;

            while (!this.m_currentBehavior.IsReady())
            {
                this.m_sequenceIndex = (this.m_sequenceIndex + 1) % this.AttackSequence.Count;
            }

            return this.m_currentBehavior.Update();
        }


        #region Overrides
        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if(this.m_currentBehavior == null)
            {
                ETGModConsole.Log("ERROR -> Dyanmic Attack Behaviour had a null current behaviour", true);

                return ContinuousBehaviorResult.Finished;
            }
            return this.m_currentBehavior.ContinuousUpdate();
        }

        public override void EndContinuousUpdate()
        {
            if(m_battleState == BattleStates.Winning)
            {
                this.m_currentBehavior.EndContinuousUpdate();
            }
            else
            {
                base.EndContinuousUpdate();
            }

            this.UpdateBattleState();
        }

        public override bool UpdateEveryFrame()
        {
            return this.m_currentBehavior != null && this.m_currentBehavior.UpdateEveryFrame();
        }

        public override bool IsOverridable()
        {
            return (this.m_currentBehavior == null) ? base.IsOverridable() : this.m_currentBehavior.IsOverridable();
        }
        #endregion


        #region States
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
            return m_battleState;
        }
        #endregion
    }
}
