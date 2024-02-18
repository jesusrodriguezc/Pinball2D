using Godot;
using Pinball.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

public enum ZLevel {
	Background = 0,
	Level0_Floor = 25,
	Level0_FloorAsset = 30,
	Level0_Ball = 35,
	Level0_WallAsset = 40,
	Level0_Wall = 45,
	Level1_Floor = 50,
	Level1_FloorAsset = 55,
	Level1_Ball = 60,
	Level1_WallAsset = 65,
	Level1_Wall = 70,
	Shader = 100,
	UI = 500
}

public enum LayerId {
	Ball_Level0 = 1,
	Map_Border = 1 << 1,
	Level0 = 1 << 2,
	Level1 = 1 << 3,
	SkillShootBarrier = 1 << 4,
	LeftShootBarrier = 1 << 5,
	Ball_Level1 = 1 << 6,
}
public static class LayerManager {
	public static Dictionary<LayerId, ZLevel> LayerZLevel = new Dictionary<LayerId, ZLevel>()
	{
		{ LayerId.Ball_Level0, ZLevel.Level0_Ball},
		{ LayerId.Map_Border, ZLevel.Level0_Ball},
		{ LayerId.Level0, ZLevel.Level0_Ball},
		{ LayerId.Level1, ZLevel.Level1_Ball},
		{ LayerId.SkillShootBarrier, ZLevel.Level0_Ball},
		{ LayerId.LeftShootBarrier, ZLevel.Level0_Ball}
	};

	public static Dictionary<LayerId, LayerId> BallLayer = new Dictionary<LayerId, LayerId>()
{
		{ LayerId.Ball_Level0, LayerId.Ball_Level0},
		{ LayerId.Map_Border, LayerId.Ball_Level0},
		{ LayerId.Level0, LayerId.Ball_Level0},
		{ LayerId.Level1, LayerId.Ball_Level1},
		{ LayerId.SkillShootBarrier, LayerId.Ball_Level0},
		{ LayerId.LeftShootBarrier, LayerId.Ball_Level1}
	};

	public static Dictionary<LayerId, Node2D[]> ActionablesInLayer = new();

	public static int GetZLevel (int layerId) {
		var layer = (LayerId)Enum.ToObject(typeof(LayerId), 1 << (layerId - 1));

		if (!LayerZLevel.TryGetValue(layer, out ZLevel result)) {
			throw new Exception($"Layer {layerId} no definida.");
		}

		return (int)result;
	}

	public static uint GetBallLayer(int layerId) {
		var layer = (LayerId)Enum.ToObject(typeof(LayerId), 1 << (layerId - 1));

		if (!BallLayer.TryGetValue(layer, out LayerId result)) {
			throw new Exception($"Layer {layerId} no definida.");
		}

		return (uint)result;
	}

	public static void UpdateActionables (Node startingNode, LayerId layer = 0, bool forceUpdate = false) {

		if (layer > 0) {
			ActionablesInLayer[layer] = Nodes
				.findByClass<CollisionObject2D>(startingNode)
				.Where(node => (node is IActionable || node is TriggerBase) && (node.CollisionLayer & (uint)layer) == (uint)layer)
				.ToArray();
			return;
		}

		if (ActionablesInLayer.Count > 0 && !forceUpdate) {
			return;
		}

		foreach (LayerId id in Enum.GetValues(typeof(LayerId))) {
			ActionablesInLayer[id] = Nodes
			.findByClass<CollisionObject2D>(startingNode)
			.Where(node => (node is IActionable || node is TriggerBase) && (node.CollisionLayer & (uint)id) == (uint)id)
			.ToArray();
		}

		//PrintActionables();
	}

	private static void PrintActionables () {
		foreach(var actionables in ActionablesInLayer) {
			foreach(var actionable in actionables.Value) {
				GD.Print($"Actionable {actionable.Name} in layer {actionables.Key}");
			}
		}
	}

	public static Node2D[] GetActionablesFromLayer (LayerId layerId, Node startingNode, bool forceUpdate = false) {

		if ((int)layerId <= 0) {
			return null;
		}

		if (forceUpdate) {
			UpdateActionables(startingNode, layerId);
		}

		ActionablesInLayer.TryGetValue(layerId, out Node2D[] nodes);

		return nodes;
	}

	public static Node2D[] GetActionablesOutOfLayer (LayerId layerId, Node startingNode, bool forceUpdate = false) {

		if (forceUpdate) {
			UpdateActionables(startingNode);
		}

		if ((int)layerId == -1) {
			GD.PrintErr($"No valid layer when calling GetActionablesOutOfLayer({layerId}, {startingNode.Name})");
			return ActionablesInLayer.SelectMany(actionLayer => actionLayer.Value).ToArray();
		}

		return ActionablesInLayer.Where(actionLayer => actionLayer.Key != layerId).SelectMany(actionLayer => actionLayer.Value).ToArray(); ;
	}

	public static LayerId Int2LayerId (int layer) {
		if (!Enum.IsDefined(typeof(LayerId), 1 << (layer - 1))) {
			return 0;
		}
		return (LayerId)(1 << (layer - 1));
	}

	public static int LayerId2Int (LayerId layer) {

		foreach (LayerId layerId in Enum.GetValues(typeof(LayerId))) {
			if (layerId == layer) {
				return (int)Math.Log2((int)layer) + 1;
			};
		}

		return -1;
	}
}

