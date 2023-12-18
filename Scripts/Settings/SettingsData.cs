using Godot;
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
		{0,  new Vector2I(640, 480)},
		{1, new Vector2I(1024, 768)},
		{2, new Vector2I(1280, 720)},
		{3, new Vector2I(1366, 768)},
		{4, new Vector2I(1920, 1080)}
	};
	#endregion

	#region Shader Palettes
	public static readonly Dictionary<long, StringName> PaletteDict = new() {
		{0, "res://Palettes/pink_palette.png"},
		{1, "res://Palettes/blue_palette.png"},
		{2, "res://Palettes/gray_palette.png"}
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
	public long ShaderPalette { get; set; }
	public double MasterVolume { get; set; }
	public double MusicVolume { get; set; }
	public double SfxVolume { get; set; }

	#endregion

	public string GetPaletteURL () {
		return PaletteDict[ShaderPalette];
	}
}

