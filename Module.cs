using EnemyAPI;
using GungeonAPI;
using UnityEngine;

namespace DynamicRandomness
{
    public class Module : ETGModule
    {
        public static readonly string MOD_NAME = "Dynamic Randomness Mod";
        public static readonly string VERSION = "0.6.0";
        public static readonly string TEXT_COLOR = "#7851A9";

        public static int BossClone = 0;
        public static readonly int Order = Random.Range(0,3);

        public static bool TutorialDone = false;

        public override void Start()
        {
            if (!BraveRandom.IsInitialized()) BraveRandom.InitializeRandom();

            // Init DRM Hooks
            Hooks.Init();

            // Init EnemyAPI
            EnemyAPI.Hooks.Init();
            EnemyAPI.Tools.Init();

            // Set Up Commands
            #region Commands
            /*
            ETGModConsole.Commands.AddGroup("thesis", args =>
            {
                ThesisFloorGenerator.Enabled = !ThesisFloorGenerator.Enabled;

                Log("Thesis floor generation is " +
                    (ThesisFloorGenerator.Enabled ? "enabled" : "disabled"));
            });

            ETGModConsole.Commands.GetGroup("thesis").AddUnit("npcs", args =>
            {
                var stats = GameStatsManager.Instance;

                stats.SetFlag(GungeonFlags.DAISUKE_ACTIVE_IN_FOYER, true);
            });
            */
            #endregion

            this.OverrideQuickStart();

            var drController = ETGModMainBehaviour.Instance
                .gameObject.AddComponent<DynamicRandomnessController>();

            drController.Init();            

            Log($"{MOD_NAME} v{VERSION} started successfully.", TEXT_COLOR);
        }

        public static void Log(string text, string color="#FFFFFF")
        {
            if(color != "#FFFFFF")
                ETGModConsole.Log($"<color={color}>{text}</color>");
            else
                ETGModConsole.Log($"<color={TEXT_COLOR}>{text}</color>");
        }

        public override void Exit() { }

        public override void Init() { }


        private void OverrideQuickStart()
        {
            GameManager.Instance.InjectedLevelName = "tt_tutorial";
            ETGMod.Player.QuickstartReplacement = "PlayerRogue";
            ETGMod.Player.PlayerReplacement = "PlayerRogue";

            DungeonHooks.OnPreDungeonGeneration += ThesisFloorGenerator.OnPreDungeonGen;

            Foyer.DoMainMenu = false;
        }
    }
}
