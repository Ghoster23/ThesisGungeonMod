using UnityEngine;
using DynamicRandomness.Behaviours;

namespace DynamicRandomness
{
    public class DynamicRandomnessController : MonoBehaviour
    {
        private HealthHaver m_currentBoss;

        private float m_battleStart;

        private HealthHaver m_player
        {
            get
            {
                return GameManager.Instance.PrimaryPlayer.healthHaver;
            }
        }


        private void Start()
        {
            
        }

        private void Update()
        {
            if(this.m_currentBoss != null &&
               (this.m_currentBoss.GetCurrentHealth() <= 0f ||
                GameManager.Instance.PrimaryPlayer.healthHaver.IsDead))
            {
                this.BattleEnded();
            }
        }


        public void BattleStarted(HealthHaver currentBoss)
        {
            this.m_currentBoss = currentBoss;

            this.m_battleStart = Time.time;

            Module.Data.StartBattleLog(GetVariantId(this.m_currentBoss.gameActor));
        }

        public void BattleEnded()
        {
            var playerHealth = this.m_player.GetCurrentHealth();

            var bossHealth = this.m_currentBoss.GetCurrentHealth();

            var duration = Time.time - this.m_battleStart;

            Module.Data.EndBattleLog(duration, playerHealth, bossHealth);

            this.m_currentBoss = null;

            if (playerHealth <= 0)
                Module.Data.IncrementDeathCount();

            if (bossHealth <= 0)
                this.ResetPlayerStats();
        }

        public void TutorialComplete()
        {
            Module.Data.TutorialComplete();
        }


        private void ResetPlayerStats()
        {
            var player = GameManager.Instance.PrimaryPlayer;

            player.healthHaver.FullHeal();
            player.healthHaver.Armor = 0;

            foreach (var gun in player.inventory.AllGuns)
                gun.GainAmmo(gun.AdjustedMaxAmmo);

            player.Blanks = 2;
        }


        public static int GetVariantId(GameActor boss)
        {
            var attackGroup = boss.behaviorSpeculator.AttackBehaviorGroup;

            if (attackGroup is SequentialAttackBehaviourGroup)
            {
                if(Module.Debug) Module.Log("Starting Battle -> Sequential Variant");
                return 1;
            }

            if (attackGroup is DynamicAttackBehaviourGroup)
            {
                if (Module.Debug) Module.Log("Starting Battle -> Dynamic Variant");
                return 2;
            }

            if (Module.Debug) Module.Log("Starting Battle -> Random Variant");
            return 3;
        }
    }
}
