using System.Collections.Generic;

namespace DynamicRandomness.Behaviours.Overrides
{
    class GatlingGullSequentialBehaviour : SequentialAttackBehaviourGroup, IAttackBehaviorGroup
    {
        public override void Start()
        {
            base.Start();

            /* Attack Behaviours:
             * 0 - Walk and Spray
             * 1 - Fan Spray
             * 2 - Big Shot
             * 3 - Waves
             * 4 - Leap
             * 5 - Rockets (has Leap embedded)
             */

            this.SequenceOverride = new List<int>()
            {
                0, 2, 3, 0, 5
            };
        }
    }
}
