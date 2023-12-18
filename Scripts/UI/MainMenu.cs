using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] PackedScene gameScene;
	[Export] PackedScene creditsScene;
	[Export] PackedScene optionsScene;
	private SceneSwitcher sceneSwitcher;

	public override void _Ready () {
		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");

	}
	private void OnStartButtonPressed () {
		sceneSwitcher?.GotoScene("res://Escenas/GameScene.tscn");
	}


	private void OnOptionsButtonPressed () {
		sceneSwitcher?.GotoScene("res://Escenas/SettingsMenu.tscn");
	}


	private void OnExitButtonPressed () {
		GetTree().Quit();
	}
	
	private void OnCreditsButtonPressed()
	{
		if (creditsScene == null) {
			return;
		}

		sceneSwitcher?.GotoScene("res://Escenas/CreditsScene.tscn");
	}
}






