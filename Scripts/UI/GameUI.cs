using Godot;
using System;
using System.Linq;

public partial class GameUI : VBoxContainer
{
	Label fpsLabel;
	Label scoreLabel;
	Label lifesLabel;

	// Debug
	//Label ballPosition;
	//Label ballVelocity;
	//Label cameraPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fpsLabel = GetNodeOrNull<Label>("FPS");
		scoreLabel = GetNodeOrNull<Label>("Score");
		lifesLabel = GetNodeOrNull<Label>("Lifes");
		//ballPosition = GetNode<Label>("BallPosition");
		//ballVelocity = GetNode<Label>("BallVelocity");
		//cameraPosition = GetNode<Label>("CameraPosition");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		fpsLabel.Text = $"FPS {Engine.GetFramesPerSecond()}";
		scoreLabel.Text = string.Format("Score: {0:0000000}", PinballController.Instance.GetScore());
		lifesLabel.Text = string.Format("Lifes: {0:00}", PinballController.Instance.LivesLeft);

		//ballPosition.Text = string.Format("Ball position: ({0})", PinballController.Instance?.Ball.GlobalPosition.ToString());
		//ballVelocity.Text = string.Format("Ball velocity: ({0})", PinballController.Instance?.Ball.LinearVelocity.ToString());
		//cameraPosition.Text = string.Format("Camera position: ({0})", PinballController.Instance?.CurrentCamera.Position.ToString());

	}

}
