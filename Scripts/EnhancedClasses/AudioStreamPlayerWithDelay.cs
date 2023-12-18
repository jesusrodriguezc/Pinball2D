using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class AudioStreamPlayerWithDelay : AudioStreamPlayer2D {
	public StringName AudioName;
	public bool isTimerRunning { get { return timer.TimeLeft > 0; } }
	public Timer timer;
	public AudioStreamPlayerWithDelay () : base() {
		timer = new();
		timer.OneShot = true;
		AddChild(timer);
		timer.Timeout += () => { base.Play(); };
	}

	public new void Play (float delay = 0f) {
		if (delay > 0f) {
			timer.Start(delay);
			return;
		}

		base.Play();
	}

	public void SetStream(AudioStream stream, StringName name = null) {
		Stream = stream;
		AudioName = name;
	}
	
}
