using Godot;
using System;

public partial class GameOverMenu : Control
{
	[Signal] public delegate void RetryEventHandler ();

	private ScoringController scoringController;
	private SceneSwitcher sceneSwitcher;
	private Label ScoreLabel;

	private bool isGameOver = false;
	public override void _Ready()
	{
		scoringController  = GetNode<ScoringController>("/root/ScoringController");
		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");
		ScoreLabel = GetNodeOrNull<Label>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/ScoreLabelValue");

		VisibilityChanged += OnVisibilityChanged;
	}

	private void OnRetryButtonPressed () {
		sceneSwitcher?.GotoScene("res://Escenas/GameScene.tscn");
		EmitSignal(SignalName.Retry);
	}


	private void OnMenuButtonPressed () {
		sceneSwitcher?.GotoScene("res://Escenas/MainMenu.tscn");
	}


	private void OnExitButtonPressed () {
		GetTree().Quit();
	}

	private void OnVisibilityChanged () {
		isGameOver = Visible;
		if (isGameOver) {
			ScoreLabel.Text = string.Format("{0:0000000}", scoringController?.Score ?? 0);
			scoringController?.SaveScore();

		}
	}

	private void OnShowLeaderboard () {
		sceneSwitcher?.GotoScene("res://addons/silent_wolf/Scores/Leaderboard.tscn");
	}
}



