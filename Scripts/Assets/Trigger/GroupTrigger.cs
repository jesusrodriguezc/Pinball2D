using Godot;
using Pinball.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;

public partial class GroupTrigger : Node2D {

	public const string BONUS_ON = "bonus_on";
	public const string BONUS_OFF = "bonus_off";
	public const string SCORE_ADD = "score_add";

	[Signal] public delegate void GlobalEffectEventHandler (string type, Node2D element);

	[Export] public StringName GroupName;
	[Export] public AudioStream AudioStream { get; set; }

	[Export] private Node2D[] actionables;
	private IGroupable[] groupableArray;
	public Timer timer { get; set; }

	private AudioComponent _audioComponent;

	public readonly StringName POWERUP = "Powerup";

	public bool Active { get; set; }
	private bool allActive = false;

	[ExportGroup ("Group effect properties")]
	[Export] public double ScoreAddition { get; set; }
	[Export] public double BonusMultiplier { get; set; }
	[Export] public double Duration { get; set; }

	public override void _Ready () {

		groupableArray = GetChildren().OfType<IGroupable>().ToArray();

		timer = GetNodeOrNull<Timer>("Timer");
		timer.Timeout += Off;

		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(POWERUP, AudioStream);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
		allActive = groupableArray.All(l => l.Active);

		if (allActive && timer.TimeLeft == 0) {
			GD.Print("Hola");
			On();
		}
	}

	public void On (double? duration = null) {
		foreach (var groupable in groupableArray) {
			groupable.Blocked = true;
		}
		foreach (var actionable in actionables) {
			if (BonusMultiplier != 1) {
				EmitSignal(SignalName.GlobalEffect, BONUS_ON, actionable);
			}

			if (ScoreAddition > 0) {
				EmitSignal(SignalName.GlobalEffect, SCORE_ADD, actionable);
			}

		}
		_audioComponent.Play(POWERUP, AudioComponent.SFX_BUS);
		timer.Start(duration ?? Duration);
	}

	public void Off () {
		foreach (var actionable in actionables) {
			if (BonusMultiplier != 1) {
				EmitSignal(SignalName.GlobalEffect, BONUS_OFF, actionable);
			}
		}
		foreach (var groupable in groupableArray) {
			groupable.Reset();
		}
		Active = false;
		timer.Stop();
	}

}
