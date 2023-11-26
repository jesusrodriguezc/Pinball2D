using Godot;
using Godot.Collections;
using System;

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
	Ball = 1,
	Map_Border = 1 << 2,
	Level0 = 1 << 3,
	Level1 = 1 << 4,
	SkillShootBarrier = 1 << 5
}
public static class LayerManager{
	public static Dictionary<LayerId, ZLevel> LayerZLevel = new Dictionary<LayerId, ZLevel>() 
	{
		{ LayerId.Ball, ZLevel.Level0_Ball},
		{ LayerId.Map_Border, ZLevel.Level0_Ball},
		{ LayerId.Level0, ZLevel.Level0_Ball},
		{ LayerId.Level1, ZLevel.Level1_Ball},
		{ LayerId.SkillShootBarrier, ZLevel.Level0_Ball}
	};

	public static int GetZLevel(int layerId) {
		var layer = (LayerId)Enum.ToObject(typeof(LayerId), 1 << layerId);

		if (!LayerZLevel.TryGetValue(layer, out ZLevel result)) {
			throw new Exception($"Layer {layerId} no definida.");
		}
			
		return (int)result;
	}
}

