using ItemAPI;
using EnemyAPI;
using GungeonAPI;
using UnityEngine;
using System.Net;

namespace DynamicRandomness
{
    public class Module : ETGModule
    {
        public static readonly string MOD_NAME = "Dynamic Randomness Mod";
        public static readonly string VERSION = "0.1.0";
        public static readonly string TEXT_COLOR = "#7851A9";

        public static int BossClone = 0;
        public static readonly int Order = Random.Range(0,2);

        public override void Start()
        {
            Hooks.Init();
            EnemyAPI.Tools.Init();

            ETGModConsole.Commands.AddGroup("thesis", args =>
            {
                ThesisFloorGenerator.Enabled = !ThesisFloorGenerator.Enabled;

                Log("Thesis floor generation is " +
                    (ThesisFloorGenerator.Enabled ? "enabled" : "disabled"));
            });

            ETGModConsole.Commands.GetGroup("thesis").AddUnit("upload", args =>
            {
                // Create a new WebClient instance.
                WebClient myWebClient = new WebClient();

                // Upload the file to the URI.
                // The 'UploadFile(uriString,fileName)' method implicitly uses HTTP POST method.
                byte[] responseArray = myWebClient.UploadFile(
                    "http://web.tecnico.ulisboa.pt/ist186470/thesis/testupload.html",
                    "./BehaviourLogs/GatlingGull_0.txt");

                var responseString = System.Text.Encoding.ASCII.GetString(responseArray);

                ETGModDebugLogMenu.Log(responseString);
                System.IO.File.WriteAllText("upload_log.txt", responseString);
            });

            DungeonHooks.OnPreDungeonGeneration += ThesisFloorGenerator.OnPreDungeonGen;

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
    }
}
