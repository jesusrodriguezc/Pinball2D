using Godot;
using System;
using System.Linq;

public partial class GameUI : VBoxContainer
{
	private PinballController pinballController;
	Label scoreLabel;
	Label lifesLabel;
	Label ballPosition;

	public override void _Ready()
	{
		pinballController = GetNode<PinballController>("/root/Pinball");
		scoreLabel = GetNodeOrNull<Label>("Score");
		lifesLabel = GetNodeOrNull<Label>("Lifes");
		ballPosition = GetNodeOrNull<Label>("BallPosition");

	}

	public override void _Process(double delta)
	{
		scoreLabel.Text = string.Format("Score: {0:0000000}", pinballController.GetScore());
		lifesLabel.Text = string.Format("Lifes: {0:00}", pinballController.LivesLeft);
		ballPosition.Text = string.Format("Ball position: {0}", pinballController.Ball.GlobalPosition);

	}

}
