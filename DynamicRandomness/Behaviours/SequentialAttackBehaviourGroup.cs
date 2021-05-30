using UnityEngine;
using EnemyAPI;
using System.Collections.Generic;

namespace DynamicRandomness.Behaviours
{
    class SequentialAttackBehaviourGroup : AttackBehaviorGroup, IAttackBehaviorGroup
    {
		private int m_currentIndex = -1;

		private int m_sequenceLength
        {
			get
            {
				if (this.IsOverriden) return this.SequenceOverride.Count;
				else return this.AttackBehaviors.Count;
            }
        }

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
				if(this.IsOverriden)
                {
					var ind = m_currentIndex % this.SequenceOverride.Count;
					return this.AttackBehaviors[this.SequenceOverride[ind]].Behavior;
                }
				else
					return this.AttackBehaviors[m_currentIndex].Behavior;
			}
		}


		public override BehaviorResult Update()
		{
			if(m_currentIndex >= m_sequenceLength) m_currentIndex = 0;

			m_currentIndex = (m_currentIndex + 1) % m_sequenceLength;

			while(!m_currentBehavior.IsReady())
            {
				m_currentIndex = (m_currentIndex + 1) % m_sequenceLength;
			}

			return m_currentBehavior.Update();
		}


		public override ContinuousBehaviorResult ContinuousUpdate()
		{
			if (this.m_currentBehavior == null)
			{
				ETGModConsole.Log("ERROR -> Sequential Attack Behaviour had a null current behaviour", true);

				return ContinuousBehaviorResult.Finished;
			}
			return this.m_currentBehavior.ContinuousUpdate();
		}

		public override void EndContinuousUpdate()
		{ 
			if ((this.IsOverriden && this.m_currentIndex < this.SequenceOverride.Count) ||
				(!this.IsOverriden && this.m_currentIndex < this.AttackBehaviors.Count))
			{
				if(this.m_currentBehavior != null) this.m_currentBehavior.EndContinuousUpdate();
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
