using Godot;
using System;

public partial class IntroScreen : Control
{
	private AnimationPlayer animationPlayer;
	private Timer timeoutTimer;
	private SceneSwitcher sceneSwitcher;

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		timeoutTimer = new Timer {
			OneShot = true
		};
		AddChild(timeoutTimer);

		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("fade_in");
		await ToSignal(animationPlayer, "animation_finished");

		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");
		sceneSwitcher?.GotoScene("res://Escenas/MainMenu.tscn");



	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
