using Godot;
using System.Collections.Generic;
using System.Xml.Linq;

public partial class ScoreComponent : Node {

	[Signal]
	public delegate void ScoreEventHandler (double score);

	[Export]
	public double BaseScore { get; set; }
	public double Multiplier {
		get {
			double ret = 1f;
			foreach (var bonus in currentBonus) {
				ret *= bonus.Value;
			}
			return ret;
		}
	}
	public double CurrentScore { get { return BaseScore * Multiplier; } }
	public Dictionary<StringName, double> currentBonus = new Dictionary<StringName, double>();

	public void AddScore() {
		EmitSignal(SignalName.Score, CurrentScore);

	}

	public void ChangeUpgradeBonus (StringName bonusName, UPGRADE_LEVEL bonusLevel) {
		if (!ScoreDataValues.upgradeLevelMultiplierDict.TryGetValue(bonusLevel, out var bonusMultiplier)) {
			GD.PrintErr($"Upgrade level {bonusLevel} does not have assigned a bonus multiplier");
			return;
		}
		currentBonus[bonusName] = bonusMultiplier;
	}

}

