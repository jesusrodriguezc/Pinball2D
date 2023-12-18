using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AudioComponent : Node
{
	public static readonly int MAX_AUDIO_STREAM_PLAYERS = 16;

	public static string MASTER_BUS = "master";
	public static string MUSIC_BUS = "Music";
	public static string SFX_BUS = "sfx";


	private List<AudioStreamPlayerWithDelay> audioStreamPlayerPool;
	private Dictionary<StringName, AudioStream> AudioDictionary;

	public bool Loop { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (AudioDictionary == null) AudioDictionary = new Dictionary<StringName, AudioStream>();
		audioStreamPlayerPool = new List<AudioStreamPlayerWithDelay>();
	}

	public AudioStreamPlayerWithDelay GetPlayer() {
		bool maxPlayersReached = audioStreamPlayerPool.Count >= MAX_AUDIO_STREAM_PLAYERS;

		AudioStreamPlayerWithDelay availablePlayer = audioStreamPlayerPool
			.FirstOrDefault(player => !player.Playing && (!player.isTimerRunning || maxPlayersReached));

		if (availablePlayer == null) {

			availablePlayer = new();
			AddChild(availablePlayer);
			audioStreamPlayerPool.Add(availablePlayer);
		}
		return availablePlayer;
	}
	public void AddAudio(StringName name, AudioStream stream) {
		if (stream == null) {
			GD.PrintErr($"[{Owner.Name}.{Name}]: No se ha podido agregar el sonido {name}");
		}

		AudioDictionary.TryAdd(name, stream);
	}
	public void Play(StringName name = null, StringName audioBusName = null)  => Play(name, 0f, audioBusName);
	public void Play (StringName name, float delay = 0f, StringName audioBusName = null) {
		var audioPlayer = GetPlayer();

		if (name == null || !AudioDictionary.ContainsKey(name)) {
			return;
		} 
		
		audioPlayer.SetStream(AudioDictionary[name], name);
		audioPlayer.Bus = audioBusName;
		GD.Print($"[Audio bus for {name}]: {audioBusName} -> {audioPlayer.Bus}");
		audioPlayer.Play(delay);

	}

	public bool IsPlayingOrQueued(StringName name) {
		if (name == null || !AudioDictionary.TryGetValue(name, out var audio)) {
			return false;
		}
		
		var currentPlayer = audioStreamPlayerPool
			.FirstOrDefault(player => player.AudioName == name);

		if (currentPlayer == null) {
			return false;
		}
		return currentPlayer.Playing;
	}
}
