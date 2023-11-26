using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] PackedScene gameScene;
	[Export] PackedScene optionsScene;

	public override void _Ready () {
		GetNode<Button>("VBox/StartButton").GrabFocus();
	}
	private void OnStartButtonPressed () {
		if (gameScene == null) {
			return;
		}
		GetTree().ChangeSceneToFile("res://Escenas/MainScene.tscn");
		//GetTree().ChangeSceneToPacked(gameScene);
		
	}


	private void OnOptionsButtonPressed () {
		if (optionsScene == null) {
			return;
		}
		//var optionsScene = ResourceLoader.Load<PackedScene>(optionsScenePath).Instantiate();
		GetTree().CurrentScene.AddChild(optionsScene.Instantiate());
	}


	private void OnExitButtonPressed () {
		GetTree().Quit();
	}
}



