using Godot;
using System;

public partial class ScoreModifier : Node2D, IActionable 
{
	[Signal] public delegate void ActionedEventHandler ();
	private ScoreComponent scoreComponent;

	[Export] public double BaseScore;

	public bool IsCollisionEnabled { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scoreComponent = GetNode<ScoreComponent>("ScoreComponent");
		scoreComponent.BaseScore = BaseScore;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Action (EventData data) {
		scoreComponent.AddScore();
	}

	public void EnableCollision (bool enable) {
		return;
	}
}
