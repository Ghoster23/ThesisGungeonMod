using System.Reflection;
using MonoMod.RuntimeDetour;
using UnityEngine;
using System.IO;

namespace DynamicRandomness
{
    class Hooks
    {

        public static void Init()
        {
            // Remove other Playable Characters from the Breach
            Hook characterSelectHook = new Hook(
                typeof(FoyerCharacterSelectFlag).GetMethod("Start", BindingFlags.Public | BindingFlags.Instance),
                typeof(Hooks).GetMethod("FoyerCharacterDestroyHook")
            );

            Debug.Log("Hooked -> FoyerCharacterDestroyHook");

            // Override Breach doors to lead to the Tutorial
            Hook forceTutorialHook = new Hook(
                typeof(FoyerGungeonDoor).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(Hooks).GetMethod("ForceTutorialHook")
            );

            Debug.Log("Hooked -> ForceTutorialHook");

            // Override the Tutorial's stairs to lead into the Test Level (Tutorial Skip)
            Hook forceGungeonHook = new Hook(
                typeof(FoyerGungeonDoor).GetMethod("OnTriggered", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(Hooks).GetMethod("TutorialEndRedirect")
            );

            Debug.Log("Hooked -> TutorialEndRedirect");

            // Override the Tutorial's elevator to lead into the Test Level (Tutorial complete)
            Hook tutorialElevatorHook = new Hook(
                typeof(ElevatorDepartureController).GetMethod("TransitionToDepart", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(Hooks).GetMethod("TransitionToDepartHook")
            );

            Debug.Log("Hooked -> TransitionToDepartHook");

            // Automatically re-start on Player death
            Hook deathRestartHook = new Hook(
                typeof(GameManager).GetMethod("DoGameOver", BindingFlags.Public | BindingFlags.Instance),
                typeof(Hooks).GetMethod("RestartOnDeath")
            );

            Debug.Log("Hooked -> RestartOnDeath");

            // Set Player Loadout to the desired state at the start of the Level
            Hook playerStatsHook = new Hook(
                typeof(GameStatsManager).GetMethod("SetStat", BindingFlags.Public | BindingFlags.Instance),
                typeof(Hooks).GetMethod("SetPlayerStats")
            );

            Debug.Log("Hooked -> SetPlayerStats");

            // Signal the start of a Boss Battle
            Hook battleStarted = new Hook(
                typeof(GatlingGullIntroDoer).GetMethod("TriggerSequence", BindingFlags.Public | BindingFlags.Instance),
                typeof(Hooks).GetMethod("BattleStarted")
            );

            Debug.Log("Hooked -> BattleStarted");

            // Save the Playthrough Log
            Hook saveLog = new Hook(
                typeof(GameManager).GetMethod("OnApplicationQuit", BindingFlags.Public | BindingFlags.Instance),
                typeof(Hooks).GetMethod("SaveLog")
            );

            Debug.Log("Hooked -> SaveLog");
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
                GameManager.Instance.QuickRestart();

                Module.Controller.TutorialComplete();

                Module.TutorialDone = true;
            }
            else
            {
                orig(self, animator, clip);
            }
        }


        public static void RestartOnDeath(System.Action<GameManager, string> orig, GameManager self, string gameOverSource = "") {
            self.QuickRestart();
        }


        public static void SetPlayerStats(System.Action<GameStatsManager, TrackedStats, float> orig,
            GameStatsManager self, TrackedStats trackedStats, float value)
        {
            orig(self, trackedStats, value);

            if (trackedStats == TrackedStats.TIME_PLAYED && value == 0f)
            {
                GameManager.Instance.PrimaryPlayer.GiveItem("heart_holster");
            }
        }


        public static void BattleStarted(System.Action<GatlingGullIntroDoer, PlayerController> orig,
            GatlingGullIntroDoer self, PlayerController player)
        {
            orig(self, player);

            Module.Controller.BattleStarted(self.healthHaver);
        }


        public static void SaveLog(System.Action<GameManager> orig, GameManager self)
        {
            orig(self);

            File.WriteAllText("log.json", Module.Data.GetJSON());
        }
        #endregion
    }
}
