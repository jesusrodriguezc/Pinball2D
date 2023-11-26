using Godot;
using System;

public partial class GameOverMenu : Control
{
	[Export] PackedScene gameScene;
	[Export] PackedScene menuScene;
	public override void _Ready()
	{
		GetNode<Button>("VBox/RetryButton").GrabFocus();
	}

	private void OnRetryButtonPressed () {
		if (gameScene == null) {
			return;
		}
		
		GetTree().ChangeSceneToPacked(gameScene);
	}


	private void OnMenuButtonPressed () {
		if (gameScene == null) {
			return;
		}
		GetTree().ChangeSceneToPacked(menuScene);
	}


	private void OnExitButtonPressed () {
		GetTree().Quit();

	}
}



