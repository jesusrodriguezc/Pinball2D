using Godot;
using System;
using System.Linq;

public partial class GameUI : PanelContainer
{
	private PinballController pinballController;
	Label scoreLabel;
	Label lifesLabel;

	public override void _Ready()
	{
		pinballController = GetNode<PinballController>("/root/Pinball");
		scoreLabel = GetNodeOrNull<Label>("GridContainer/ScoreValue");
		lifesLabel = GetNodeOrNull<Label>("GridContainer/LifesValue");
	}

	public override void _Process(double delta)
	{
		scoreLabel.Text = string.Format("{0:0000000}", pinballController.GetScore());
		lifesLabel.Text = string.Format("{0:00}", pinballController.LivesLeft);
	}

}
