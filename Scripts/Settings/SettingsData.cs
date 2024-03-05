using Godot;
//using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SettingsData {

	#region Display Mode Values
	public const long WINDOWED_MODE = 0;
	public const long FULLSCREEN_MODE = 1;
	public const long BORDERLESS_WINDOW_MODE = 2;
	public const long BORDERLESS_FULLSCREEN_MODE = 3;
	#endregion

	#region Resolution Values
	public static readonly Dictionary<long, Vector2I> ResolutionDict = new() {
		{0,  new Vector2I(854, 480)},
		{1, new Vector2I(1280, 720)},
		{2, new Vector2I(1366, 768)},
		{3, new Vector2I(1600, 900)},
		{4, new Vector2I(1920, 1080)},
		{5, new Vector2I(2560, 1440)},
		{6, new Vector2I(3840, 2160)}

	};
	#endregion

	#region Shader Palettes
	public static readonly Dictionary<StringName, StringName> PaletteDict = new() {
		{"PINK", "res://Palettes/pink_palette.png"},
		{"BLUE", "res://Palettes/blue_palette.png"},
		{"GRAYSCALE", "res://Palettes/gray_palette.png"},
		{"GREEN", "res://Palettes/ammo-8-1x.png"},
		{"PHOENIX", "res://Palettes/st-8-phoenix-1x.png"}
	};

	#endregion

	#region Language Values
	public static Dictionary<long, string> LanguageDict = new () {
		{0, "en"},
		{1, "es"}
	};

	#endregion

	#region Audio Buses
	public const int MASTER_AUDIO_BUS = 0;
	public const int MUSIC_AUDIO_BUS = 1;
	public const int SOUND_FX_AUDIO_BUS = 2;

	#endregion

	#region Data
	public long DisplayMode { get; set; }
	public bool VSync { get; set; }
	public long WindowResolution { get; set; }
	public string ShaderPalette { get; set; }
	public double MasterVolume { get; set; }
	public double MusicVolume { get; set; }
	public double SfxVolume { get; set; }
	public string PlayerName { get; set; }
	public long Language { get; set; }

	#endregion

	public string GetPaletteURL () {
		if (string.IsNullOrEmpty(ShaderPalette) || !PaletteDict.ContainsKey(ShaderPalette)) {
			return null;
		}
		return PaletteDict[ShaderPalette];
	}
}

