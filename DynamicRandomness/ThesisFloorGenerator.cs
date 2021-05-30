using Dungeonator;
using GungeonAPI;


namespace DynamicRandomness
{
    public static class ThesisFloorGenerator
    {
		public static void OnPreDungeonGen(LoopDungeonGenerator generator, Dungeon dungeon, DungeonFlow flow, int dungeonSeed)
		{
			bool notFoyer = flow.name != "Foyer Flow" && !GameManager.IsReturningToFoyerWithPlayer;

			if (notFoyer && flow.name != "Tutorial Flow")
			{
				Module.BossClone = 0;
				dungeonSeed = Module.Order;
				dungeon.DungeonSeed = Module.Order;
				GameManager.Instance.CurrentRunSeed = Module.Order;

				flow = ThesisFloorGenerator.CreateBranchingThesisFlow(dungeon);
				generator.AssignFlow(flow);
			}

			dungeon = null;
		}

		public static DungeonFlow CreateSequentialThesisFlow(Dungeon dungeon)
        {
			DungeonFlow dungeonFlow = CreateEntranceFlow(dungeon);

			dungeonFlow.name = "thesis_flow_sequential";

			DungeonFlowNode parent = dungeonFlow.FirstNode;
			
			ThesisFloorGenerator.AppendRoom(dungeonFlow, parent, "a1_legendarychests.room");

			parent = ThesisFloorGenerator.AppendRoom(dungeonFlow, parent, "b1_bossroom_gullA_test.room");

			parent = ThesisFloorGenerator.AppendRoom(dungeonFlow, parent, "c1_bufferroom.room");

			parent = ThesisFloorGenerator.AppendRoom(dungeonFlow, parent, "b1_bossroom_gullA_test.room");

			parent = ThesisFloorGenerator.AppendRoom(dungeonFlow, parent, "c1_bufferroom.room");

			parent = ThesisFloorGenerator.AppendRoom(dungeonFlow, parent, "b1_bossroom_gullA_test.room");

			parent = ThesisFloorGenerator.AppendExitElevator(dungeonFlow, parent);

			dungeon = null;
			return dungeonFlow;
        }

		public static DungeonFlow CreateBranchingThesisFlow(Dungeon dungeon)
		{
			DungeonFlow dungeonFlow = SampleFlow.CreateNewFlow(dungeon);

			dungeonFlow.name = "thesis_flow_branching";

			var parent = MakeNode(LoadRoom("lobby.room"), dungeonFlow);

			dungeonFlow.FirstNode = parent;
			dungeonFlow.AddNodeToFlow(parent, null);

			AppendBossRoom(dungeonFlow, parent);

			if(Module.Debug) ThesisFloorGenerator.AppendRoom(dungeonFlow, parent, "a1_legendarychests.room");

			AppendBossRoom(dungeonFlow, parent);

			AppendBossRoom(dungeonFlow, parent);

			dungeon = null;
			return dungeonFlow;
		}

		public static PrototypeDungeonRoom LoadRoom(string roomName)
        {
			if (!RoomFactory.rooms.TryGetValue(roomName, out RoomFactory.RoomData roomData))
			{
				Tools.LogToConsole("Could not find room with key " + roomName);

				return null;
			}

			return roomData.room;
		}

		public static DungeonFlowNode MakeNode(PrototypeDungeonRoom room, DungeonFlow dungeonFlow)
        {
			DungeonFlowNode dungeonFlowNode = new DungeonFlowNode(dungeonFlow)
			{
				overrideExactRoom = room
			};

			return dungeonFlowNode;
        }

		private static DungeonFlowNode AppendRoom(DungeonFlow dungeonFlow, DungeonFlowNode parentNode, string roomName)
        {
			PrototypeDungeonRoom room = LoadRoom(roomName);

			if (room is null) return parentNode;

			DungeonFlowNode dungeonFlowNode = MakeNode(room, dungeonFlow);

			dungeonFlow.AddNodeToFlow(dungeonFlowNode, parentNode);

			return dungeonFlowNode;
		}


		private static void AppendBossRoom(DungeonFlow dungeonFlow, DungeonFlowNode parentNode)
        {
			DungeonFlowNode dungeonFlowNode = SampleFlow.NodeFromAssetName(dungeonFlow, "GatlingGullRoom02");

			dungeonFlowNode.overrideExactRoom.subCategoryBoss = PrototypeDungeonRoom.RoomBossSubCategory.MINI_BOSS;

			dungeonFlow.AddNodeToFlow(dungeonFlowNode, parentNode);
		}


		private static DungeonFlowNode AppendExitElevator(DungeonFlow dungeonFlow, DungeonFlowNode parentNode)
        {
			var dungeonFlowNode = SampleFlow.NodeFromAssetName(dungeonFlow, "exit_room_basic");
			
			dungeonFlow.AddNodeToFlow(dungeonFlowNode, parentNode);

			return dungeonFlowNode;
        }


		private static DungeonFlow CreateEntranceFlow(Dungeon dungeon)
		{
			DungeonFlow dungeonFlow = SampleFlow.CreateNewFlow(dungeon);
			dungeonFlow.name = "entrance_flow";
			DungeonFlowNode dungeonFlowNode = SampleFlow.NodeFromAssetName(dungeonFlow, "elevator entrance");
			dungeonFlow.FirstNode = dungeonFlowNode;
			dungeonFlow.AddNodeToFlow(dungeonFlowNode, null);
			dungeon = null;
			return dungeonFlow;
		}
	}
}
