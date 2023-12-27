using Godot;
using Pinball.Utils;
using System;
using System.Collections.Generic;

public partial class Shooter : Area2D, IActionable {

	public enum InputType {
		WAIT_KEY,
		PRESS_KEY,
		HOLD_KEY,
		PRESS_AND_WAIT,
		HOLD_AND_WAIT
	}
	private Ball currBall;

	#region Audio
	private AudioComponent audioComponent;
	private TriggerArea triggerArea;
	public readonly StringName HIT = "Hit";
	public readonly StringName MAX_POWER = "MAX_POWER";

	[Export] AudioStream HitAudio;
	[Export] AudioStream MaxPowerAudio;

	#endregion
	[Export] public int MaxHitPower { get; set; }

	[Export] public Vector2 ShootDirection;
	bool shootIntoTarget;

	[Signal]
	public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);

	[Export] public double HoldingForMaxPowerSound;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {

		audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		triggerArea = GetNode<TriggerArea>("../");

		if (HitAudio != null) audioComponent?.AddAudio(HIT, HitAudio);
		if (MaxPowerAudio != null) audioComponent?.AddAudio(MAX_POWER, MaxPowerAudio);
		shootIntoTarget = (ShootDirection == Vector2.Zero);

		if (HoldingForMaxPowerSound <= 0f) HoldingForMaxPowerSound = triggerArea?.HoldingTime ?? 0d;

		QueueRedraw();
	}

	public override void _Draw () {
		DrawLine(Vector2.Zero, ShootDirection * MaxHitPower, Colors.Yellow, 4f);
	}

	public void Action(EventData data) {

		//object holdPercObj;
		float holdPerc = 1f;
		if (data.Parameters != null && data.Parameters.TryGetValue(ITrigger.HoldPercentage, out var holdPercObj)) {
			holdPerc = (float)holdPercObj;
		} 

		if (shootIntoTarget) {
			ShootDirection = GlobalPosition - triggerArea.Target.Position;
		}

		if (triggerArea.Target is not RigidBody2D TargetPhys) {
			return;
		}

		float CurrentHitPower = MaxHitPower * Mathx.FuncSmooth(holdPerc);

		if (holdPerc >= HoldingForMaxPowerSound) {
			audioComponent?.Play(MAX_POWER, 0.1f, AudioComponent.SFX_BUS);
		}

		TargetPhys.ApplyCentralImpulse(ShootDirection * CurrentHitPower);
		audioComponent?.Play(HIT, AudioComponent.SFX_BUS);


	}
}
