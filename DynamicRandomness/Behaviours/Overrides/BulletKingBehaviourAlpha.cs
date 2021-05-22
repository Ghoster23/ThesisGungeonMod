using System;
using System.Collections.Generic;
using EnemyAPI;

namespace DynamicRandomness.Behaviours.Overrides
{
    class BulletKingBehaviourAlpha : OverrideBehavior
    {
        public override string OverrideAIActorGUID => "ffca09398635467da3b1f4a54bcfda80";

        public override void DoOverride()
        {
            Tools.DebugInformation(this.behaviorSpec, "./BehaviourLogs/BulletKing.txt");
        }
    }
}
