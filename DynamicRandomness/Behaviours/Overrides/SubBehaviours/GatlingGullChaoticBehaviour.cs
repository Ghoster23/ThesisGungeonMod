using System.Collections.Generic;

namespace DynamicRandomness.Behaviours.Overrides
{
    class GatlingGullChaoticBehaviour : ChaoticAttackBehaviourGroup, IAttackBehaviorGroup
    {
        protected override void ConstraintChaos()
        {
            base.ConstraintChaos();

            this.AttackBehaviors[1].Probability = 0;
            this.AttackBehaviors[4].Probability = 0;

            if (this.m_previousBehaviour != null)
                this.m_previousBehaviour.Probability = 0;
        }
    }
}
