using Godot;
using Pinball.Components;
using System;

public partial class MainMenu : Control
{
	[Export] PackedScene gameScene;
	[Export] PackedScene creditsScene;
	[Export] PackedScene optionsScene;
	private SceneSwitcher sceneSwitcher;
	private SettingsMenu settingsMenu;
	private ShaderPlaceholder shaderPlaceholder;
	private AudioComponent globalAudioSystem;

	private Label VersionLabel;

	private readonly StringName MUSIC = "Music";
	private readonly string GAME_VERSION = "v.0.0.4";


	public override void _Ready () {
		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");
		settingsMenu = GetNodeOrNull<SettingsMenu>("SettingsMenu");
		shaderPlaceholder = GetNodeOrNull<ShaderPlaceholder>("/root/ShaderPlaceholder");
		shaderPlaceholder.Apply();
		globalAudioSystem = GetNodeOrNull<AudioComponent>("/root/GlobalAudioSystem");
		VersionLabel = GetNodeOrNull<Label>("VersionLabel");
		VersionLabel.Text = GAME_VERSION;

		if (globalAudioSystem != null && !globalAudioSystem.IsPlayingOrQueued(MUSIC)) {
			var audioStream = (AudioStreamMP3)ResourceLoader.Load<AudioStream>("res://SFX/pinball_music.mp3");
			audioStream.Loop = true;
			audioStream.Bpm = 117;

			globalAudioSystem.AddAudio(MUSIC, audioStream);
			globalAudioSystem.Play(MUSIC, AudioComponent.MUSIC_BUS);

		}


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
		//sceneSwitcher?.GotoScene("res://Escenas/CreditsScene.tscn");
		sceneSwitcher?.GotoScene("res://Escenas/IntroScreen.tscn");
	}
}






