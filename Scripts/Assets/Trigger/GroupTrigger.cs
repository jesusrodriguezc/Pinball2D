using Godot;
using Pinball.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GroupTrigger : Node2D, ITrigger {

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
	[Export] private bool canShiftWhenButtonPressed;

	[ExportGroup ("Group effect properties")]
	[Export] public double ScoreAddition { get; set; }
	[Export] public double BonusMultiplier { get; set; }
	[Export] public double Duration { get; set; }
	public bool Triggered { get; set; }

	public override void _Ready () {

		groupableArray = GetChildren().OfType<IGroupable>().ToArray();

		timer = GetNodeOrNull<Timer>("Timer");
		timer.Timeout += Off;

		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(POWERUP, AudioStream);
		}

	}

	public override void _Input (InputEvent @event) {
		if (!canShiftWhenButtonPressed) {
			return;
		}
		if (Input.IsActionJustPressed("Left_Action")) {
			ShiftLeft();
		}

		if (Input.IsActionJustPressed("Right_Action")) {
			ShiftRight();
		}
	}

	private void ShiftRight () {
		bool lastValue = groupableArray[groupableArray.Length - 1].Active;

		// 1 2 3 ->  3 1 2
		for (int i = groupableArray.Length - 1; i > 0; i--) {
			groupableArray[i].Active = groupableArray[i - 1].Active;
		}
		groupableArray.First().Active = lastValue;
	}

	private void ShiftLeft () {
		bool firstValue = groupableArray[0].Active;		

		for (int i = 0; i < groupableArray.Length - 1; i++) {
			groupableArray[i].Active = groupableArray[i + 1].Active;
		}
		groupableArray.Last().Active = firstValue;
		
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
		allActive = groupableArray.All(l => l.Active);

		if (allActive && timer.TimeLeft == 0) {
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
		Triggered = true;
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
		timer.Stop();
		Triggered = false;
	}

	public void Trigger (Dictionary<StringName, object> args = null) {
	}
}
