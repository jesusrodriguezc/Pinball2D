using Godot;
using Godot.Collections;
using Pinball.Components;
using System;
using System.Text.Json;

public partial class SettingsMenu : Node
{
	private SceneSwitcher sceneSwitcher;
	private SettingsManager settingsManager;
	private OptionButton displayModeButton;
	private CheckButton vsyncButton;
	private OptionButton resolutionButton;
	private OptionButton shaderPaletteButton;

	private HSlider masterVolumeSlider;
	private HSlider musicVolumeSlider;
	private HSlider sfxVolumeSlider;

	private Button applyButton;
	private Button backButton;

	private Vector2I currentResolution;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");
		settingsManager = GetNode<SettingsManager>("/root/SettingsManager");

		// Video
		displayModeButton = GetNodeOrNull<OptionButton>("TabContainer/_GRAPHICS_/MarginContainer/GridContainer/DisplayModeOpt");
		vsyncButton = GetNodeOrNull<CheckButton>("TabContainer/_GRAPHICS_/MarginContainer/GridContainer/VSyncOpt");
		resolutionButton = GetNodeOrNull<OptionButton>("TabContainer/_GRAPHICS_/MarginContainer/GridContainer/WindowResolutionOpt");
		shaderPaletteButton = GetNodeOrNull<OptionButton>("TabContainer/_GRAPHICS_/MarginContainer/GridContainer/ShaderPaletteOpt");

		// Audio
		masterVolumeSlider = GetNodeOrNull<HSlider>("TabContainer/_SOUND_/MarginContainer/GridContainer/MasterVolumeSlider");
		musicVolumeSlider = GetNodeOrNull<HSlider>("TabContainer/_SOUND_/MarginContainer/GridContainer/MusicVolumeSlider");
		sfxVolumeSlider = GetNodeOrNull<HSlider>("TabContainer/_SOUND_/MarginContainer/GridContainer/SFXVolumeSlider");

		// Gameplay

		applyButton = GetNodeOrNull<Button>("HBoxContainer/ApplyButton");
		backButton = GetNodeOrNull<Button>("HBoxContainer/BackButton");

		UpdateValues();

		displayModeButton.ItemSelected += settingsManager.DisplayModeSelected;
		displayModeButton.ItemSelected += (value) => { SetDisabledResolutionSelection((value & 1) > 0); };
		vsyncButton.Toggled += settingsManager.VSyncButtonToggled;
		resolutionButton.ItemSelected += settingsManager.WindowResolutionSelected;
		shaderPaletteButton.ItemSelected += settingsManager.ShaderPaletteSelected;



		masterVolumeSlider.ValueChanged += settingsManager.MasterVolumeSelected;
		musicVolumeSlider.ValueChanged += settingsManager.MusicVolumeSelected;
		sfxVolumeSlider.ValueChanged += settingsManager.SfxVolumeSelected;

	

		if (DisplayServer.WindowGetMode() != DisplayServer.WindowMode.Windowed) {
			SetDisabledResolutionSelection(true);
		}

		applyButton.Pressed += settingsManager.Save;
		applyButton.Pressed += () => sceneSwitcher?.GotoScene("res://Escenas/MainMenu.tscn"); 
		
		backButton.Pressed += () => sceneSwitcher?.GotoScene("res://Escenas/MainMenu.tscn");



	}

	private void UpdateValues () {
		displayModeButton.Select((int)settingsManager.settingsData.DisplayMode);
		vsyncButton.SetPressedNoSignal(settingsManager.settingsData.VSync);
		resolutionButton.Select((int)settingsManager.settingsData.WindowResolution);
		shaderPaletteButton.Select((int)settingsManager.settingsData.ShaderPalette);

		masterVolumeSlider.SetValueNoSignal(settingsManager.settingsData.MasterVolume);
		musicVolumeSlider.SetValueNoSignal(settingsManager.settingsData.MusicVolume);
		sfxVolumeSlider.SetValueNoSignal(settingsManager.settingsData.SfxVolume);
	}

	public void SetDisabledResolutionSelection(bool disabled) {
		resolutionButton.Selected = disabled? -1: resolutionButton.Selected;
		resolutionButton.Disabled = disabled;
	}
	
	private void OnBackButtonPressed()
	{
		sceneSwitcher?.GotoScene("res://Escenas/MainMenu.tscn");
	}

}







