using System.Collections.Generic;

namespace DynamicRandomness.Behaviours.Overrides
{
    class GatlingGullDynamicBehaviour : DynamicAttackBehaviourGroup, IAttackBehaviorGroup
    {
        public override void Start()
        {
            base.Start();

            this.AttackSequence = new List<int>()
            {
                0, 2, 2, 3, 4, 1, 2, 2, 3, 5, 2, 2, 3
            };
        }
        
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
            return;
        }

        protected override void ActivateLosingState()
        {
            foreach (var attackBehaviour in this.AttackBehaviors)
                attackBehaviour.Probability = 1;
        }
    }
}
