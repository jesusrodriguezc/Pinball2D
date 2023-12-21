using Godot;
using System;

public partial class GameOverMenu : Control
{
	private SceneSwitcher sceneSwitcher;
	private Label ScoreLabel;
	public override void _Ready()
	{
		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");
		ScoreLabel = GetNodeOrNull<Label>("ScoreLabel");

		ScoreLabel.Text = string.Format("SCORE: {0:00000}", PinballController.Instance?.GetScore() ?? 0);

	}

	private void OnRetryButtonPressed () {
		sceneSwitcher?.GotoScene("res://Escenas/GameScene.tscn");
	}


	private void OnMenuButtonPressed () {
		sceneSwitcher?.GotoScene("res://Escenas/MainMenu.tscn");
	}


	private void OnExitButtonPressed () {
		GetTree().Quit();
	}
}



