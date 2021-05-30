using EnemyAPI;
using GungeonAPI;
using UnityEngine;
using DynamicRandomness.Data_Logging;

namespace DynamicRandomness
{
    public class Module : ETGModule
    {
        public static bool Debug = false;

        public static readonly string MOD_NAME = "DR Mod";
        public static readonly string VERSION = "0.9.1";
        public static readonly string TEXT_COLOR = "#7851A9";

        public static int BossClone = 0;
        public static readonly int Order = Random.Range(0,3);

        public static DynamicRandomnessController Controller;

        public static PlaythroughData Data;

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
            ETGModConsole.Commands.AddGroup("thesis", args =>
            {
                Log("DR Mod is running and the experiment scenario will be created.\n"+
                    "\tThe following sub-commands are available:\n"+
                    "\t\tdebug - toggles debug messages for the experiment;\n");
            });

            ETGModConsole.Commands.GetGroup("thesis").AddUnit("debug", args =>
            {
                Debug = !Debug;

                Log("DR Mod - Debug messages " + (Debug ? "activated" : "deactivated"));
            });
            #endregion

            this.OverrideQuickStart();

            Controller = ETGModMainBehaviour.Instance
                .gameObject.AddComponent<DynamicRandomnessController>();

            Data = new PlaythroughData(Order, Time.time);

            Log($"{MOD_NAME} v{VERSION} started successfully.", TEXT_COLOR);

            Log($"Experiment Variant -> {Order}");
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
