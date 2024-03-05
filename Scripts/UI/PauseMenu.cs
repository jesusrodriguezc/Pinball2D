using Godot;
using System;

public partial class PauseMenu : Control {
	private PinballController pinballController;
	private SceneSwitcher sceneSwitcher;
	private Label scoreLabel;

	public override void _Ready () {
		pinballController = GetNode<PinballController>("/root/Pinball");
		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");
		scoreLabel = GetNodeOrNull<Label>("CenterContainer/VBoxContainer/HBoxContainer/ScoreValue");
		VisibilityChanged += () => scoreLabel.Text = string.Format("{0:0000000}", pinballController?.GetScore());
	}

	
	private void OnResumeButtonPressed () {
		Hide();
		pinballController?.ResumeGame();
	}


	private void OnMenuButtonPressed () {
		pinballController?.ResumeGame();
		pinballController?.HideUI();
		sceneSwitcher?.GotoScene("res://Escenas/MainMenu.tscn");
	}


	private void OnExitButtonPressed () {
		GetTree().Quit();
	}

}
