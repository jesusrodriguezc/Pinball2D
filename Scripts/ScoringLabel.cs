using Godot;
using System;

public partial class ScoringLabel : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Text = string.Format("Score: {0:0000000}", PinballController.Instance.Score);
	}
}
