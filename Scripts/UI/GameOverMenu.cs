using Godot;
using System;

public partial class GameOverMenu : Control
{
	private SceneSwitcher sceneSwitcher;

	public override void _Ready()
	{
		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");

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



