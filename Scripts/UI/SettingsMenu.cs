using Godot;
using Godot.Collections;
using Pinball.Components;
using Pinball.Scripts.Utils;
using System;
using System.Linq;
using System.Text.Json;

public partial class SettingsMenu : Control
{
	private SceneSwitcher sceneSwitcher;
	private SettingsManager settingsManager;

	private Label settingsTitle;

	#region Graphics Settings
	private OptionButton displayModeButton;
	private CheckButton vsyncButton;
	private OptionButton resolutionButton;
	private OptionButton shaderPaletteButton;
	#endregion

	#region Sound Settings
	private HSlider masterVolumeSlider;
	private HSlider musicVolumeSlider;
	private HSlider sfxVolumeSlider;
	#endregion

	#region Gameplay
	private OptionButton languageButton;
	private LineEdit playerNameEdit;
	#endregion

	private Button applyButton;

	private Vector2I currentResolution;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");
		settingsManager = GetNode<SettingsManager>("/root/SettingsManager");

		if (settingsManager.IsNodeReady()) {
			PrepareSettingsMenu();
		}
	}

	private void PrepareSettingsMenu () {

		// Video
		displayModeButton = GetNodeOrNull<OptionButton>("VBoxContainer/TabContainer/_GRAPHICS_/MarginContainer/GridContainer/DisplayModeOpt");
		vsyncButton = GetNodeOrNull<CheckButton>("VBoxContainer/TabContainer/_GRAPHICS_/MarginContainer/GridContainer/VSyncOpt");
		resolutionButton = GetNodeOrNull<OptionButton>("VBoxContainer/TabContainer/_GRAPHICS_/MarginContainer/GridContainer/WindowResolutionOpt");
		shaderPaletteButton = GetNodeOrNull<OptionButton>("VBoxContainer/TabContainer/_GRAPHICS_/MarginContainer/GridContainer/ShaderPaletteOpt");

		// Audio
		masterVolumeSlider = GetNodeOrNull<HSlider>("VBoxContainer/TabContainer/_SOUND_/MarginContainer/GridContainer/MasterVolumeSlider");
		musicVolumeSlider = GetNodeOrNull<HSlider>("VBoxContainer/TabContainer/_SOUND_/MarginContainer/GridContainer/MusicVolumeSlider");
		sfxVolumeSlider = GetNodeOrNull<HSlider>("VBoxContainer/TabContainer/_SOUND_/MarginContainer/GridContainer/SFXVolumeSlider");

		// Gameplay

		playerNameEdit = GetNodeOrNull<LineEdit>("VBoxContainer/TabContainer/_GAMEPLAY_/MarginContainer/GridContainer/PlayerNameEdit");
		languageButton = GetNodeOrNull<OptionButton>("VBoxContainer/TabContainer/_GAMEPLAY_/MarginContainer/GridContainer/LanguageOpt");

		applyButton = GetNodeOrNull<Button>("VBoxContainer/PanelContainer/ApplyButton");

		UpdateValues();

		displayModeButton.ItemSelected += settingsManager.DisplayModeSelected;
		displayModeButton.ItemSelected += (value) => { SetDisabledResolutionSelection((value & 1) > 0); };
		vsyncButton.Toggled += settingsManager.VSyncButtonToggled;
		resolutionButton.ItemSelected += settingsManager.WindowResolutionSelected;

		foreach(var paletteName in SettingsData.PaletteDict.Keys) {
			shaderPaletteButton.AddItem(paletteName);
		}

		shaderPaletteButton.ItemSelected += (paletteId) => settingsManager.ShaderPaletteSelected(shaderPaletteButton.GetItemText((int)paletteId));

		masterVolumeSlider.ValueChanged += settingsManager.MasterVolumeSelected;
		musicVolumeSlider.ValueChanged += settingsManager.MusicVolumeSelected;
		sfxVolumeSlider.ValueChanged += settingsManager.SfxVolumeSelected;

		playerNameEdit.TextChanged += settingsManager.PlayerNameSubmitted;
		languageButton.ItemSelected += settingsManager.LanguageSelected;


		if (DisplayServer.WindowGetMode() != DisplayServer.WindowMode.Windowed) {
			SetDisabledResolutionSelection(true);
		}

		applyButton.Pressed += settingsManager.Save;
		applyButton.Pressed += () => Hide();

	}

	private void UpdateValues () {
		displayModeButton.Select((int)settingsManager.settingsData.DisplayMode);
		vsyncButton.SetPressedNoSignal(settingsManager.settingsData.VSync);
		resolutionButton.Select((int)settingsManager.settingsData.WindowResolution);
		shaderPaletteButton.Select(shaderPaletteButton.GetItemByText(settingsManager.settingsData.ShaderPalette));

		masterVolumeSlider.SetValueNoSignal(settingsManager.settingsData.MasterVolume);
		musicVolumeSlider.SetValueNoSignal(settingsManager.settingsData.MusicVolume);
		sfxVolumeSlider.SetValueNoSignal(settingsManager.settingsData.SfxVolume);

		playerNameEdit.Text = settingsManager.settingsData.PlayerName;
		languageButton.Select((int)settingsManager.settingsData.Language);
	}

	public void SetDisabledResolutionSelection(bool disabled) {
		resolutionButton.Disabled = disabled;

		if (disabled) {
			var screenSize = DisplayServer.ScreenGetSize();
			var resolutionId = SettingsData.ResolutionDict
				.Where(res => res.Value == screenSize)
				.Select(p => new { Key = (int)p.Key, Value = p.Value })
				.FirstOrDefault();

			resolutionButton.Selected = resolutionId?.Key ?? -1;
			return;
		}

		resolutionButton.Selected = (int)settingsManager.settingsData.WindowResolution;
	}

	public override void _Input (InputEvent inputEvent) {
		if (Input.IsActionJustPressed("Back")) {
			sceneSwitcher?.GotoScene("res://Escenas/MainMenu.tscn");
		}
	}

}