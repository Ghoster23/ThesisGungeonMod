using EnemyAPI;
using DynamicRandomness.Behaviours.Attacks;

namespace DynamicRandomness.Behaviours.Overrides
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
                    this.SequentialOverride();
                    break;

                case 2: // Dynamic
                    this.DynamicOverride();
                    break;

                case 3: // Chaotic
                    foreach (var attackBehaviour in behaviorSpec.AttackBehaviorGroup.AttackBehaviors)
                        attackBehaviour.Probability = 1;
                    break;
            }

            Module.BossClone++;
        }


        private void SequentialOverride()
        {
            var origAttackGroup = behaviorSpec.AttackBehaviorGroup;

            var newBehaviour = new GatlingGullSequentialBehaviour
            {
                AttackBehaviors = origAttackGroup.AttackBehaviors
            };

            /*
            newBehaviour.AttackBehaviors[0] = new AttackBehaviorGroup.AttackGroupItem()
            {
                Behavior = new GatlingGullWalkAndFanSpray(),
                NickName = "Walk And Fan Spray",
                Probability = 3f
            };
            */

            for (var i = 0; i < behaviorSpec.AttackBehaviors.Count; i++)
            {
                var attackBehaviour = behaviorSpec.AttackBehaviors[i];

                if (attackBehaviour is AttackBehaviorGroup)
                {
                    behaviorSpec.AttackBehaviors[i] = newBehaviour;
                }
            }

            newBehaviour.Init(behaviorSpec.gameObject, behaviorSpec.aiActor, behaviorSpec.aiShooter);
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
