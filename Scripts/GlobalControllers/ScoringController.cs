using Godot;
using Pinball.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

public partial class ScoringController : Node {

	public double Score { get; private set; } = 0;
	private List<ScoreComponent> scoreComponents = new();
	private SettingsManager settings;
	private Node silentWolf;

	public override void _Ready () {
		// Prepare();	
		settings = GetNode<SettingsManager>("/root/SettingsManager");

		silentWolf = GetNode<Node>("/root/SilentWolf");

		silentWolf.Set("config", new Godot.Collections.Dictionary<string, Variant>() {
			{ "api_key", "Pe1K8VGY3N4yPmIc7WxhbBWYNh9fQjT6hAFsL4J3" },
			{ "game_id", "ShadedPinball" },
			{ "log_level", 1 }
		});

		silentWolf.Call("configure_scores_open_scene_on_close", "res://Escenas/MainMenu.tscn");
	}

	public void Prepare () {
		scoreComponents = Nodes.findByClass<ScoreComponent>(GetTree().Root);
		scoreComponents.ForEach(scoreComponent => { scoreComponent.Score += AddScore; });
	}
	public void ResetScore () {
		GD.Print("ResetScore()");
		Score = 0;
	}

	public void AddScore (double points) {
		Score += points;
	}

	public void SaveScore () {

		var silentWolf = GetNode<Node>("/root/SilentWolf");
		silentWolf.PrintTree();
		var silentWolfScores = silentWolf.GetNode<Node>("Scores");
		silentWolfScores.Call("save_score", settings.settingsData.PlayerName, Score);
		silentWolfScores.Call("get_high_scores");
	}

}

