using Godot;
using System;
using System.Linq;

public partial class GameUI : VBoxContainer
{
	Label fpsLabel;
	Label scoreLabel;
	Label lifesLabel;

	// Debug
	HSlider gravitySlider;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fpsLabel = GetNodeOrNull<Label>("FPS");
		scoreLabel = GetNodeOrNull<Label>("Score");
		lifesLabel = GetNodeOrNull<Label>("Lifes");
		gravitySlider = GetNode<HSlider>("GravitySlider");

		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		gravitySlider.Value = PinballController.Instance?.Ball.GravityScale ?? 0f;

		fpsLabel.Text = $"FPS {Engine.GetFramesPerSecond()}";
		scoreLabel.Text = string.Format("Score: {0:0000000}", PinballController.Instance.GetScore());
		lifesLabel.Text = string.Format("Lifes: {0:00}", PinballController.Instance.LivesLeft);

	}
	
	private void OnGravityValueChanged(bool value_changed)
	{
		if (!value_changed) { return; }
		GD.Print($"Cambiando valor de la gravedad a {gravitySlider.Value}");
		PinballController.Instance.Ball.GravityScale = (float)gravitySlider.Value;
	}
}
