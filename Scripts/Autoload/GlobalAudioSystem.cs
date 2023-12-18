using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class GlobalAudioSystem : Node 
{
	private bool noMusic;
	private AudioComponent _audioComponent;
	private readonly StringName MUSIC = "Music";


	public override void _Ready () {
		noMusic = false;

		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			var audioStream = (AudioStreamMP3)ResourceLoader.Load<AudioStream>("res://SFX/pinball_music.mp3");
			audioStream.Loop = true;
			audioStream.Bpm = 117;

			_audioComponent.AddAudio(MUSIC, audioStream);

		}

		_audioComponent.Play(MUSIC, AudioComponent.MUSIC_BUS);
	}


	public override void _Process (double delta) {
	}
}

