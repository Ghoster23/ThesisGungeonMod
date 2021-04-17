using Dungeonator;
using GungeonAPI;


namespace DynamicRandomness
{
    public static class ThesisFloorGenerator
    {
		public static bool Enabled = false;

		public static void OnPreDungeonGen(LoopDungeonGenerator generator, Dungeon dungeon, DungeonFlow flow, int dungeonSeed)
		{
			bool notFoyer = flow.name != "Foyer Flow" && !GameManager.IsReturningToFoyerWithPlayer;

			if (notFoyer && ThesisFloorGenerator.Enabled)
			{
				flow = ThesisFloorGenerator.CreateThesisFlow(dungeon);
				generator.AssignFlow(flow);
			}

			dungeon = null;
		}

		public static DungeonFlow CreateThesisFlow(Dungeon dungeon)
        {
			DungeonFlow dungeonFlow = SampleFlow.CreateEntranceExitFlow(dungeon);

			dungeonFlow.name = "thesis_flow";

			DungeonFlowNode dungeonFlowNode = new DungeonFlowNode(dungeonFlow)
			{
				overrideExactRoom = RoomFactory.CreateEmptyRoom(12, 12)
			};

			DungeonFlowNode parent = dungeonFlowNode;
			dungeonFlow.AddNodeToFlow(dungeonFlowNode, dungeonFlow.FirstNode);

			parent = ThesisFloorGenerator.AppendRoom(dungeonFlow, parent, "a1_legendarychests.room");

			parent = ThesisFloorGenerator.AppendRoom(dungeonFlow, parent, "b1_bossroom_gullA_test.room");

			parent = ThesisFloorGenerator.AppendRoom(dungeonFlow, parent, "b1_bossroom_gullA_test.room");

			parent = ThesisFloorGenerator.AppendRoom(dungeonFlow, parent, "b1_bossroom_gullA_test.room");

			dungeon = null;
			return dungeonFlow;
        }

		private static DungeonFlowNode AppendRoom(DungeonFlow dungeonFlow, DungeonFlowNode parentNode, string roomName)
        {
			if (!RoomFactory.rooms.TryGetValue(roomName, out RoomFactory.RoomData roomData))
			{
				Tools.LogToConsole("Could not find room with key " + roomName);

				return parentNode;
			}

			PrototypeDungeonRoom room = roomData.room;
			
			DungeonFlowNode dungeonFlowNode2 = new DungeonFlowNode(dungeonFlow)
			{
				overrideExactRoom = room
			};

			dungeonFlow.AddNodeToFlow(dungeonFlowNode2, parentNode);

			var dungeonFlowNode = new DungeonFlowNode(dungeonFlow)
			{
				overrideExactRoom = RoomFactory.CreateEmptyRoom(12, 12)
			};

			dungeonFlow.AddNodeToFlow(dungeonFlowNode, dungeonFlowNode2);

			return dungeonFlowNode;
		}
    }
}
