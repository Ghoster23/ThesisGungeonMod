using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace DynamicRandomness.Behaviours
{
    public enum BattleStates
    {
        Winning,
        Balanced,
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

        protected AttackGroupItem m_previousBehaviour;


        public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
        {
            base.Init(gameObject, aiActor, aiShooter);

            m_playerHealth = GameManager.Instance.PrimaryPlayer.healthHaver;
        }


        public override BehaviorResult Update()
        {
            var result = BehaviorResult.SkipAllRemainingBehaviors;

            Module.Data.UpdateBattleState(1 + (int) m_battleState);

            switch (m_battleState)
            {
                default:
                    if(Module.Debug) Module.Log("Unrecognized battle state -> " + m_battleState);
                    break;

                case BattleStates.Winning:
                    result = this.SequentialUpdate();
                    break;

                case BattleStates.Balanced:
                    result = base.Update();
                    break;

                case BattleStates.Losing:
                    result = this.RandomUpdate();
                    break;
            }

            return result;
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

        protected BehaviorResult RandomUpdate()
        {
            this.ActivateLosingState();

            return base.Update();
        }


        protected void UpdateBattleState()
        {
            float healthPercent = m_playerHealth.GetCurrentHealthPercentage();

            float bossHealthPercent = m_aiActor.healthHaver.GetCurrentHealthPercentage();

            float absDiff = Mathf.Abs(healthPercent - bossHealthPercent);

            if (absDiff < 0.075f)
            {
                m_battleState = BattleStates.Balanced;
                this.ActivateBalancedState();
            }
            else if (bossHealthPercent < healthPercent)
            {
                m_battleState = BattleStates.Losing;
                this.ActivateLosingState();
            }
            else
            {
                // This is necessary as to not restart the sequence at every step
                if (m_battleState == BattleStates.Winning) return;

                m_battleState = BattleStates.Winning;
                m_sequenceIndex = 0;
                this.ActivateWinningState();
            }
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
            for(var i = 0; i < this.AttackBehaviors.Count; i++)
            {
                if (this.AttackBehaviors[i].Behavior == this.CurrentBehavior)
                    this.m_previousBehaviour = this.AttackBehaviors[i];
            }

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
