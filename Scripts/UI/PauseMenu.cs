using Godot;
using System;

public partial class PauseMenu : Control, IPausable {
	private SceneSwitcher sceneSwitcher;

	public override void _Ready () {
		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");
	}

	private void OnResumeButtonPressed () {
		Hide();
		PinballController.Instance.ResumeGame();
	}


	private void OnMenuButtonPressed () {
		PinballController.Instance.ResumeGame();
		sceneSwitcher?.GotoScene("res://Escenas/MainMenu.tscn");
	}


	private void OnExitButtonPressed () {
		GetTree().Quit();
	}

	public void Pause () {
		Show();
	}

	public void Resume () {
		Hide();
	}
}
