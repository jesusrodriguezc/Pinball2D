using Godot;
using Pinball.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

public partial class ScoringController : Node {

	public double Score { get; private set; } = 0;
	private List<ScoreComponent> scoreComponents = new();
	public override void _Ready () {

		scoreComponents = Nodes.findByClass<ScoreComponent>(GetTree().Root);
		scoreComponents.ForEach(scoreComponent => { scoreComponent.Score += AddScore; });
	}

	public void ResetScore () {
		Score = 0;
	}

	public void AddScore (double points) {
		Score += points;
	}

}

