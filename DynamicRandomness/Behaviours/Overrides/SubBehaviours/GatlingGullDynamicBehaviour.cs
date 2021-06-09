using System.Collections.Generic;

namespace DynamicRandomness.Behaviours.Overrides
{
    class GatlingGullDynamicBehaviour : DynamicAttackBehaviourGroup, IAttackBehaviorGroup
    {
        /* Attack Behaviours:
        * 0 - Walk and Spray
        * 1 - Fan Spray
        * 2 - Big Shot
        * 3 - Waves
        * 4 - Leap
        * 5 - Rockets (has Leap embedded)
        */

        public override void Start()
        {
            base.Start();

            this.AttackSequence = new List<int>()
            {
                0, 2, 3, 0, 5
            };
        }
        
        protected override void ActivateBalancedState()
        {
            this.AttackBehaviors[0].Probability = 3;
            this.AttackBehaviors[1].Probability = 0;
            this.AttackBehaviors[2].Probability = 1.5f;
            this.AttackBehaviors[3].Probability = 1.5f;
            this.AttackBehaviors[4].Probability = 0;
            this.AttackBehaviors[5].Probability = 3;
        }

        protected override void ActivateWinningState()
        {
            return;
        }

        protected override void ActivateLosingState()
        {
            foreach (var attackBehaviour in this.AttackBehaviors)
                attackBehaviour.Probability = 1;

            this.AttackBehaviors[1].Probability = 0;
            this.AttackBehaviors[4].Probability = 0;

            if (this.m_previousBehaviour != null)
                this.m_previousBehaviour.Probability = 0;
        }
    }
}
