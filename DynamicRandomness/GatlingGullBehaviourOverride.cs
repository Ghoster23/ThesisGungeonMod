using EnemyAPI;

namespace DynamicRandomness
{
    class GatlingGullBehaviourOverride : OverrideBehavior
    {
        public override string OverrideAIActorGUID => "ec6b674e0acd4553b47ee94493d66422";

        private readonly int[][] _sequences =
        {
            new int[]{ 1, 3, 2 },
            new int[]{ 3, 1, 2 },
            new int[]{ 2, 3, 1 }
        };

        public override void DoOverride()
        {
            var sequence = _sequences[Module.Order];

            var pattern = sequence[Module.BossClone];

            switch(pattern)
            {
                default:
                case 1: // Patterned
                    break;

                case 2: // Dynamic
                    this.DynamicOverride();
                    break;

                case 3: // Chaotic
                    foreach (var attackBehaviour in behaviorSpec.AttackBehaviorGroup.AttackBehaviors)
                        attackBehaviour.Probability = 1;
                    break;
            }

            Module.Log("Overriding Gatling Gull behaviour with pattern " + pattern);

            Tools.DebugInformation(this.behaviorSpec, "./BehaviourLogs/GatlingGull_" + Module.BossClone + ".txt");

            Module.BossClone++;
        }


        private void DynamicOverride()
        {
            var origAttackGroup = behaviorSpec.AttackBehaviorGroup;

            var newBehaviour = new GatlingGullDynamicBehaviour
            {
                AttackBehaviors = origAttackGroup.AttackBehaviors
            };

            for (var i = 0; i < behaviorSpec.AttackBehaviors.Count; i++)
            {
                var attackBehaviour = behaviorSpec.AttackBehaviors[i];

                if (attackBehaviour is AttackBehaviorGroup)
                {
                    behaviorSpec.AttackBehaviors[i] = newBehaviour;
                }
            }

            newBehaviour.Init(behaviorSpec.gameObject, behaviorSpec.aiActor, behaviorSpec.aiShooter);

            newBehaviour.ShareCooldowns = origAttackGroup.ShareCooldowns;
        }
    }
}
