using Godot;
using System;
using System.Linq;

public partial class GameUI : VBoxContainer
{
	Label scoreLabel;
	Label lifesLabel;

	public override void _Ready()
	{
		scoreLabel = GetNodeOrNull<Label>("Score");
		lifesLabel = GetNodeOrNull<Label>("Lifes");
	}

	public override void _Process(double delta)
	{
		scoreLabel.Text = string.Format("Score: {0:0000000}", PinballController.Instance.GetScore());
		lifesLabel.Text = string.Format("Lifes: {0:00}", PinballController.Instance.LivesLeft);
	}

}
