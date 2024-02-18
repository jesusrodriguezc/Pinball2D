using Godot;
using Pinball.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GroupTrigger : TriggerBase {

	public const string CHANGE_UPGRADE_LEVEL = "change_upgrade";
	public const string SCORE_ADD = "score_add";
	public const string ACTION_DONE = "action_done";

	[Signal] public delegate void GlobalEffectEventHandler (string type, Node2D element);

	[Export] public StringName GroupName;
	[Export] public AudioStream AudioStream { get; set; }

	[Export] private Node2D[] actionables;
	private IGroupable[] groupableArray;


	private AudioComponent _audioComponent;

	public readonly StringName POWERUP = "Powerup";

	public bool Active { get; set; }
	private bool allReady = false;
	[Export] private bool canShiftWhenButtonPressed;
	private CooldownComponent cooldownComponent;

	[ExportGroup ("Group effect properties")]
	[Export] public double ScoreAddition { get; set; }
	[Export] public UPGRADE_LEVEL CurrentUpgradeLevel { get; set; }
	[Export] public bool isUpgradingActionable { get; set; }
	[Export] public double EffectDuration { get; set; }
	public Timer EffectTimer { get; set; }
	[Export] public double Cooldown { get; set; }
	[Export] public double DisableDuration { get; set; }
	public override void _Ready () {

		groupableArray = GetChildren().OfType<IGroupable>().ToArray();

		EffectTimer = GetNodeOrNull<Timer>("Timer");
		EffectTimer.Timeout += EventOff;

		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(POWERUP, AudioStream);
		}

		cooldownComponent = GetNode<CooldownComponent>("CooldownComponent");
		cooldownComponent.SetCooldown(Cooldown);
	}

	public override void _Input (InputEvent @event) {
		if (!canShiftWhenButtonPressed) {
			return;
		}
		if (Input.IsActionJustPressed("Left_Flipper")) {
			ShiftLeft();
		}

		if (Input.IsActionJustPressed("Right_Flipper")) {
			ShiftRight();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
		allReady = groupableArray.All(l => l.Active && !l.Blocked);

		if (allReady) {
			Enable();
		}
	}

	public void Enable (double? duration = null) {
		if (!EffectTimer.IsStopped()) {
			EffectTimer.Stop();
		}

		foreach (var groupable in groupableArray) {
			groupable.Blocked = true;
			groupable.OnCompleted(DisableDuration);
		}
		if (CurrentUpgradeLevel != UPGRADE_LEVEL.THIRD && isUpgradingActionable) {
			CurrentUpgradeLevel += 1;
		}
		foreach (var actionable in actionables) {
			if (isUpgradingActionable) {
				EmitSignal(SignalName.GlobalEffect, CHANGE_UPGRADE_LEVEL, actionable);
			}
			if (ScoreAddition > 0) {
				EmitSignal(SignalName.GlobalEffect, SCORE_ADD, actionable);
			}
		}

		EmitSignal(SignalName.GlobalEffect, ACTION_DONE, this);

		_audioComponent.Play(POWERUP, AudioComponent.SFX_BUS);
		EffectTimer.Start(duration ?? EffectDuration);
		IsTriggered = true;
	}

	public void EventOff () {
		if (!isUpgradingActionable) {
			return;
		}

		if (CurrentUpgradeLevel > UPGRADE_LEVEL.BASIC) {
			CurrentUpgradeLevel -= 1;
		}

		foreach (var actionable in actionables) {

			if (CurrentUpgradeLevel >= UPGRADE_LEVEL.BASIC) {
				EmitSignal(SignalName.GlobalEffect, CHANGE_UPGRADE_LEVEL, actionable);
			}
		}

		IsTriggered = false;

		if (CurrentUpgradeLevel > UPGRADE_LEVEL.BASIC) {
			EffectTimer.Start();
		}
	}

	public override void Trigger (EventData data) {

		if (cooldownComponent.IsOnCooldown) {
			return;
		}

		base.Trigger(data);


		// TODO: Debe generar un evento.

	}

	#region BonusLanes
	private void ShiftRight () {
		bool lastValue = groupableArray[groupableArray.Length - 1].Active;

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

	#endregion

}
