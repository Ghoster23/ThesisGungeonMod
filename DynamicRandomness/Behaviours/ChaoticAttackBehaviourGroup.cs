using UnityEngine;
using EnemyAPI;
using System.Collections.Generic;

namespace DynamicRandomness.Behaviours
{
    class ChaoticAttackBehaviourGroup : AttackBehaviorGroup, IAttackBehaviorGroup
    {
		protected AttackGroupItem m_previousBehaviour;


		public override void Start()
		{
			for (int i = 0; i < this.AttackBehaviors.Count; i++)
			{
				var groupItem = this.AttackBehaviors[i];

				if (groupItem.Behavior != null)
				{
					groupItem.Behavior.Start();

					groupItem.Probability = 1;
				}
			}
		}


		public override void EndContinuousUpdate()
		{
			for (var i = 0; i < this.AttackBehaviors.Count; i++)
				if (this.AttackBehaviors[i].Behavior == this.CurrentBehavior)
					this.m_previousBehaviour = this.AttackBehaviors[i];

			base.EndContinuousUpdate();

			this.ConstraintChaos();
		}

		public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
		{
			base.Init(gameObject, aiActor, aiShooter);

			this.ConstraintChaos();
		}


		protected virtual void ConstraintChaos()
        {
			foreach (var behavior in this.AttackBehaviors)
				behavior.Probability = 1;
        }
	}
}
