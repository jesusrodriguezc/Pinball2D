using Godot;
using System;

public partial class GameUI : VBoxContainer
{
	Label fpsLabel;
	Label scoreLabel;
	Label lifesLabel;

	// Debug
	HSlider lightSlider;
	DirectionalLight2D mainLight;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fpsLabel = GetNodeOrNull<Label>("FPS");
		scoreLabel = GetNodeOrNull<Label>("Score");
		lifesLabel = GetNodeOrNull<Label>("Lifes");

		// Debug
		lightSlider = GetNodeOrNull<HSlider>("Light");
		mainLight = GetNodeOrNull<DirectionalLight2D>("/root/Main/MainLight");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		fpsLabel.Text = $"FPS {Engine.GetFramesPerSecond()}";
		scoreLabel.Text = string.Format("Score: {0:0000000}", PinballController.Instance.GetScore());
		lifesLabel.Text = string.Format("Score: {0:00}", PinballController.Instance.LivesLeft);

	}
	
	private void OnLightValueChanged(bool value_changed)
	{
		if (!value_changed) { return; }
		GD.Print($"Cambiando valor de la luz de {mainLight.Energy} a {lightSlider.Value}");
		mainLight.Energy = (float)lightSlider.Value;
	}
}



