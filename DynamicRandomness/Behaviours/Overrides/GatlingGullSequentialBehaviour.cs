using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicRandomness.Behaviours.Overrides
{
    class GatlingGullSequentialBehaviour : SequentialAttackBehaviourGroup, IAttackBehaviorGroup
    {
        protected new List<int> SequenceOverride = new List<int>()
        {
            0, 1, 0, 4, 2, 1, 4, 3, 4, 3, 1, 4, 5
        };
    }
}
