using UnityEngine;
using EnemyAPI;
using System.Collections.Generic;

namespace DynamicRandomness.Behaviours
{
    class SequentialAttackBehaviourGroup : AttackBehaviorGroup, IAttackBehaviorGroup
    {
		private enum State
		{
			Idle,
			Update,
			ContinuousUpdate,
			Cooldown
		}

		private State m_state;

		private int m_currentIndex = -1;

		private float m_overrideCooldownTimer;

		protected List<int> SequenceOverride;

		protected bool IsOverriden
		{
			get
            {
				return this.SequenceOverride != null;
            }
		}

		private AttackBehaviorBase m_currentBehavior
		{
			get
			{
				if(this.SequenceOverride != null)
                {
					var ind = this.m_currentIndex % this.SequenceOverride.Count;
					return this.AttackBehaviors[ind].Behavior;
                }
				else
					return this.AttackBehaviors[this.m_currentIndex].Behavior;
			}
		}



		public override BehaviorResult Update()
		{
			BehaviorResult behaviorResult = BehaviorResult.Continue;

			if (behaviorResult != BehaviorResult.Continue)
			{
				return behaviorResult;
			}

			if (!this.IsReady())
			{
				return BehaviorResult.Continue;
			}

			this.m_currentIndex = 0;
			this.m_state = State.Update;

			bool flag = this.StepBehaviors();

			if (flag)
			{
				return BehaviorResult.RunContinuous;
			}

			return BehaviorResult.SkipAllRemainingBehaviors;
		}

		public override ContinuousBehaviorResult ContinuousUpdate()
		{
			bool flag = this.StepBehaviors();
			return (!flag) ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
		}

		public override void EndContinuousUpdate()
		{ 
			if ((this.IsOverriden && this.m_currentIndex < this.SequenceOverride.Count) ||
				(!this.IsOverriden && this.m_currentIndex < this.AttackBehaviors.Count))
			{
				this.m_currentBehavior.EndContinuousUpdate();
			}
			this.m_currentIndex = -1;
		}

		private bool StepBehaviors()
		{
			switch(this.m_state)
            {
				default:
					Debug.LogError("Unrecognized State " + this.m_state);
					return false;

				case State.Cooldown:
					this.m_overrideCooldownTimer += this.m_deltaTime;

					if ((!this.IsOverriden && this.m_currentIndex == this.AttackBehaviors.Count - 1) ||
						(this.IsOverriden && this.m_currentIndex == this.SequenceOverride.Count - 1))
					{
						return false;
					}

					if (this.m_currentBehavior.IsReady())
					{
						this.m_currentIndex++;
						this.m_state = State.Update;
						return this.StepBehaviors();
					}
					return true;

				case State.Update:
					BehaviorResult behaviorResult = this.m_currentBehavior.Update();

					if (behaviorResult == BehaviorResult.Continue ||
						behaviorResult == BehaviorResult.SkipRemainingClassBehaviors ||
						behaviorResult == BehaviorResult.SkipAllRemainingBehaviors)
					{
						this.m_state = State.Cooldown;
						this.m_overrideCooldownTimer = 0f;
						return this.StepBehaviors();
					}

					if (behaviorResult == BehaviorResult.RunContinuous ||
						behaviorResult == BehaviorResult.RunContinuousInClass)
					{
						this.m_state = State.ContinuousUpdate;
						return true;
					}

					Debug.LogError("Unrecognized BehaviorResult " + behaviorResult);
					return false;

				case State.ContinuousUpdate:
					ContinuousBehaviorResult continuousBehaviorResult = this.m_currentBehavior.ContinuousUpdate();

					if (continuousBehaviorResult == ContinuousBehaviorResult.Finished)
					{
						this.m_currentBehavior.EndContinuousUpdate();
						this.m_state = State.Cooldown;
						this.m_overrideCooldownTimer = 0f;
						return this.StepBehaviors();
					}

					if (continuousBehaviorResult == ContinuousBehaviorResult.Continue)
					{
						return true;
					}

					Debug.LogError("Unrecognized BehaviorResult " + continuousBehaviorResult);
					return false;
			}
		}



		public override bool UpdateEveryFrame()
		{
			return this.m_currentIndex >= 0 && this.m_currentBehavior != null && this.m_currentBehavior.UpdateEveryFrame();
		}

		public override bool IsOverridable()
		{
			return (this.m_currentIndex >= 0 && this.m_currentBehavior == null) ? base.IsOverridable() : this.m_currentBehavior.IsOverridable();
		}
	}
}
