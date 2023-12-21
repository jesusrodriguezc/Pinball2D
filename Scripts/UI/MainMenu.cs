using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] PackedScene gameScene;
	[Export] PackedScene creditsScene;
	[Export] PackedScene optionsScene;
	private SceneSwitcher sceneSwitcher;
	private SettingsMenu settingsMenu;
	public override void _Ready () {
		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");
		settingsMenu = GetNodeOrNull<SettingsMenu>("SettingsMenu");

	}
	private void OnStartButtonPressed () {
		sceneSwitcher?.GotoScene("res://Escenas/GameScene.tscn");
	}


	private void OnOptionsButtonPressed () {
		settingsMenu.Show();
		//sceneSwitcher?.GotoScene("res://Escenas/SettingsMenu.tscn");
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






