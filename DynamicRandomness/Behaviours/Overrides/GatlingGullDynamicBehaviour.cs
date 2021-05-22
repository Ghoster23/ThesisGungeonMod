using UnityEngine;
using EnemyAPI;

namespace DynamicRandomness.Behaviours.Overrides
{
    class GatlingGullDynamicBehaviour : DynamicAttackBehaviourGroup, IAttackBehaviorGroup
    {

        protected override void ActivateBalancedState()
        {
            this.AttackBehaviors[0].Probability = 2;
            this.AttackBehaviors[1].Probability = 1;
            this.AttackBehaviors[2].Probability = 2;
            this.AttackBehaviors[3].Probability = 2;
            this.AttackBehaviors[4].Probability = 1.5f;
            this.AttackBehaviors[5].Probability = 1.5f;
        }

        protected override void ActivateWinningState()
        {
            this.AttackBehaviors[0].Probability = 3;
            this.AttackBehaviors[1].Probability = 2;
            this.AttackBehaviors[2].Probability = 1.5f;
            this.AttackBehaviors[3].Probability = 1.5f;
            this.AttackBehaviors[4].Probability = 1;
            this.AttackBehaviors[5].Probability = 3;
        }

        protected override void ActivateLosingState()
        {
            foreach (var attackBehaviour in this.AttackBehaviors)
                attackBehaviour.Probability = 1;
        }
    }
}
