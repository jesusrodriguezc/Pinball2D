using Godot;
using Pinball.Components;
using Pinball.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public partial class SettingsManager : Node {

	public const string SETTINGS_DATA_PATH = @"user://SettingsData.ini";
	private SaveManager saveManager;
	public SettingsData settingsData;
	private ShaderPlaceholder shaderPlaceholder;
	public override void _Ready () {
		saveManager = GetNode<SaveManager>("/root/SaveManager");
		
		if (!saveManager.Load(SETTINGS_DATA_PATH, out settingsData)) {
			CreateSettingsFile();
		}

		ApplySettings();

		shaderPlaceholder = GetNodeOrNull<ShaderPlaceholder>("/root/ShaderPlaceholder");
	}

	private void ApplySettings () {
		DisplayModeSelected(settingsData.DisplayMode);
		VSyncButtonToggled(settingsData.VSync);
		WindowResolutionSelected(settingsData.WindowResolution);
		ShaderPaletteSelected(settingsData.ShaderPalette);

		MasterVolumeSelected(settingsData.MasterVolume);
		MusicVolumeSelected(settingsData.MusicVolume);
		SfxVolumeSelected(settingsData.SfxVolume);

		PlayerNameSubmitted(settingsData.PlayerName);
		LanguageSelected(settingsData.Language);
	}

	private void CreateSettingsFile () {

		bool isFullscreen = DisplayServer.WindowGetMode() >= DisplayServer.WindowMode.Fullscreen;
		bool isBorderless = DisplayServer.WindowGetFlag(DisplayServer.WindowFlags.Borderless);
		var resolution = DisplayServer.WindowGetSize();
		long? resolutionId = SettingsData.ResolutionDict.Where(res => res.Value == resolution).FirstOrDefault().Key;

		settingsData = new SettingsData() {
			DisplayMode = (isFullscreen ? 1 : 0) + (isBorderless ? 2 : 0),
			VSync = DisplayServer.WindowGetVsyncMode() != DisplayServer.VSyncMode.Disabled,
			ShaderPalette = 0,
			WindowResolution = resolutionId ?? 0L,
			MasterVolume = .5,
			MusicVolume = .5,
			SfxVolume = .5,
			Language = 0
		};

		Save();
	}

	public void Save () {
		saveManager.Save(settingsData, SETTINGS_DATA_PATH);
	}

	public void DisplayModeSelected (long index) {
		bool windowed = (index & 1) == 0;
		bool borderless = (index & 2) > 0;

		DisplayServer.WindowSetMode(windowed ? DisplayServer.WindowMode.Windowed : DisplayServer.WindowMode.Fullscreen);
		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, borderless);
		settingsData.DisplayMode = index;
	}
	public void VSyncButtonToggled (bool button_pressed) {
		DisplayServer.WindowSetVsyncMode(button_pressed ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
		settingsData.VSync = button_pressed;

	}

	public void WindowResolutionSelected (long index) {
		if (!SettingsData.ResolutionDict.TryGetValue(index, out Vector2I resolution)) {
			throw new Exception($"[OnWindowResolutionSelected({index})]: Incorrect option");
		}

		settingsData.WindowResolution = index;
		DisplayServer.WindowSetSize(resolution);
	}


	public void ShaderPaletteSelected (long index) {
		settingsData.ShaderPalette = index;
		if (shaderPlaceholder == null) {
			return;
		}
		if (SettingsData.PaletteDict.TryGetValue(index, out var palettePath))
			shaderPlaceholder.Palette = GD.Load<CompressedTexture2D>(palettePath);

	}



	public void MasterVolumeSelected (double value) {
		AudioServer.SetBusVolumeDb(SettingsData.MASTER_AUDIO_BUS, (float)Mathx.Linear2Db(value));
		settingsData.MasterVolume = value;
	}

	public void MusicVolumeSelected (double value) {
		AudioServer.SetBusVolumeDb(SettingsData.MUSIC_AUDIO_BUS, (float)Mathx.Linear2Db(value));
		settingsData.MusicVolume = value;

	}

	public void SfxVolumeSelected (double value) {
		AudioServer.SetBusVolumeDb(SettingsData.SOUND_FX_AUDIO_BUS, (float)Mathx.Linear2Db(value));
		settingsData.SfxVolume = value;

	}
	public void PlayerNameSubmitted (string newText) {
		settingsData.PlayerName = newText;
	}

	public void LanguageSelected (long index) {
		if (!SettingsData.LanguageDict.TryGetValue(index, out string language)) {
			throw new Exception($"[LanguageSelected({index})]: Incorrect option");
		}
		TranslationServer.SetLocale(language);
		settingsData.Language = index;
	}


}

