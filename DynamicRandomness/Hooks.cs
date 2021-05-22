using System.Reflection;
using System.Collections;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace DynamicRandomness
{
    class Hooks
    {
        private static int s_savedSeed = 0;

        public static void Init()
        {
            Hook characterSelectHook = new Hook(
                typeof(FoyerCharacterSelectFlag).GetMethod("Start", BindingFlags.Public | BindingFlags.Instance),
                typeof(Hooks).GetMethod("FoyerCharacterDestroyHook")
            );

            Debug.Log("Hooked -> FoyerCharacterDestroyHook");

            Hook forceTutorialHook = new Hook(
                typeof(FoyerGungeonDoor).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(Hooks).GetMethod("ForceTutorialHook")
            );

            Debug.Log("Hooked -> ForceTutorialHook");

            Hook forceGungeonHook = new Hook(
                typeof(FoyerGungeonDoor).GetMethod("OnTriggered", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(Hooks).GetMethod("TutorialEndRedirect")
            );

            Debug.Log("Hooked -> TutorialEndRedirect");

            Hook tutorialElevatorHook = new Hook(
                typeof(ElevatorDepartureController).GetMethod("TransitionToDepart", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(Hooks).GetMethod("TransitionToDepartHook")
            );

            Debug.Log("Hooked -> TransitionToDepartHook");

            Hook teleporterHook = new Hook(
                typeof(TeleporterController).GetMethod("Activate", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(Hooks).GetMethod("ResetPlayerStats")
            );

            Debug.Log("Hooked -> ResetPlayerStats");

            Hook dungeonSeedHook = new Hook(
                typeof(Dungeonator.Dungeon).GetMethod("GetDungeonSeed", BindingFlags.Public | BindingFlags.Instance),
                typeof(Hooks).GetMethod("KeepSeed")
            );

            Debug.Log("Hooked -> KeepSeed");

            Hook deathRestartHook = new Hook(
                typeof(GameManager).GetMethod("DoGameOver", BindingFlags.Public | BindingFlags.Instance),
                typeof(Hooks).GetMethod("RestartOnDeath")
            );

            Debug.Log("Hooked -> RestartOnDeath");

            Hook playerStatsHook = new Hook(
                typeof(GameStatsManager).GetMethod("SetStat", BindingFlags.Public | BindingFlags.Instance),
                typeof(Hooks).GetMethod("SetPlayerStats")
            );

            Debug.Log("Hooked -> SetPlayerStats");
        }


        #region Hook Methods
        public static void FoyerCharacterDestroyHook(System.Action<FoyerCharacterSelectFlag> orig, FoyerCharacterSelectFlag self)
        {
            orig(self);

            if (!self.CharacterPrefabPath.Contains("PlayerRogue"))
            {
                //Module.Log("Destroying character select flag for: " + self.CharacterPrefabPath);

                Object.Destroy(self.gameObject);
            }
        }


        public static void ForceTutorialHook(System.Action<FoyerGungeonDoor> orig, FoyerGungeonDoor self)
        {
            orig(self);

            if (self.LevelNameToLoad != "tt_tutorial" && !Module.TutorialDone)
            {
                self.LevelNameToLoad = "tt_tutorial";
                self.LoadsCustomLevel = true;
            }
        }


        public static void TutorialEndRedirect(System.Action<FoyerGungeonDoor, SpeculativeRigidbody, SpeculativeRigidbody, CollisionData> orig,
            FoyerGungeonDoor self,
            SpeculativeRigidbody specRigidbody, SpeculativeRigidbody sourceSpecRigidbody, CollisionData collisionData)
        {
            if (self.ReturnToFoyerFromTutorial)
            {
                Module.TutorialDone = true;

                GameManager.Instance.QuickRestart();
            }
            else
            {
                orig(self, specRigidbody, sourceSpecRigidbody, collisionData);
            }
        }


        public static void TransitionToDepartHook(System.Action<ElevatorDepartureController, tk2dSpriteAnimator, tk2dSpriteAnimationClip> orig,
            ElevatorDepartureController self, tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
        {
            if (self.ReturnToFoyerWithNewInstance)
            {
                Module.TutorialDone = true;

                GameManager.Instance.QuickRestart();
            }
            else
            {
                orig(self, animator, clip);
            }
        }


        public static void ResetPlayerStats(System.Action<TeleporterController> orig, TeleporterController self)
        {
            orig(self);

            var player = GameManager.Instance.PrimaryPlayer;

            player.healthHaver.FullHeal();
            player.healthHaver.Armor = 0;

            foreach (var gun in player.inventory.AllGuns)
                gun.GainAmmo(gun.AdjustedMaxAmmo);

            player.Blanks = 2;
        }


        public static int KeepSeed(System.Action<Dungeonator.Dungeon> orig, Dungeonator.Dungeon self)
        {
            if (Hooks.s_savedSeed == 0)
            {
                orig(self);

                Hooks.s_savedSeed = GameManager.Instance.CurrentRunSeed;

                Module.Log("Run Seed Saved: " + Hooks.s_savedSeed);

                return Hooks.s_savedSeed;
            }
            else
            {
                self.DungeonSeed = Hooks.s_savedSeed;

                orig(self);

                Module.Log("Run Seed Used: " + GameManager.Instance.CurrentRunSeed);

                return Hooks.s_savedSeed;
            }
        }


        public static void RestartOnDeath(System.Action<GameManager, string> orig, GameManager self, string gameOverSource = "") {
            self.QuickRestart();
        }


        public static void SetPlayerStats(System.Action<GameStatsManager, TrackedStats, float> orig,
            GameStatsManager self, TrackedStats trackedStats, float value)
        {
            orig(self, trackedStats, value);

            if(trackedStats == TrackedStats.TIME_PLAYED && value == 0f)
                GameManager.Instance.PrimaryPlayer.GiveItem("heart_holster");
        }
        #endregion
    }
}
