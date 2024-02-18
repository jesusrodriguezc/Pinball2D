using Godot;
using Pinball.Utils;
using System;
using System.Collections.Generic;

public partial class Shooter : Node2D, IActionable {
	[Signal] public delegate void ActionedEventHandler ();
	public enum InputType {
		WAIT_KEY,
		PRESS_KEY,
		HOLD_KEY,
		PRESS_AND_WAIT,
		HOLD_AND_WAIT
	}
	#region Audio
	private AudioComponent audioComponent;
	public readonly StringName HIT = "Hit";
	public readonly StringName MAX_POWER = "MAX_POWER";

	[Export] AudioStream HitAudio;
	[Export] AudioStream MaxPowerAudio;

	#endregion
	[Export] public int MaxHitPower { get; set; }
	public bool IsCollisionEnabled { get; set; }

	[Export] public bool MantainDirection;
	[Export] public Vector2 ShootDirection;
	bool shootIntoTarget;

	[Signal]
	public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);

	[Export] public double HoldingForMaxPowerSound;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {

		audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");

		if (HitAudio != null) audioComponent?.AddAudio(HIT, HitAudio);
		if (MaxPowerAudio != null) audioComponent?.AddAudio(MAX_POWER, MaxPowerAudio);
		ShootDirection = ShootDirection.Normalized();
		shootIntoTarget = (ShootDirection == Vector2.Zero);
	}

	public void Action(EventData data) {

		RigidBody2D target = (RigidBody2D)data.Parameters[TriggerBase.ACTIVATOR];
		float holdPerc = 1f;
		if (data.Parameters != null && data.Parameters.TryGetValue(TriggerBase.HOLD_PERCENTAGE, out var holdPercObj)) {
			holdPerc = (float)holdPercObj;
		} 

		if (shootIntoTarget && !MantainDirection) {
			ShootDirection = (GlobalPosition - target.GlobalPosition).Normalized();
		}

		if (MantainDirection) {
			ShootDirection = target.LinearVelocity.Normalized();
		}

		if (target is not RigidBody2D TargetPhys) {
			return;
		}

		float CurrentHitPower = MaxHitPower * Mathx.FuncSmooth(holdPerc);

		if (holdPerc >= HoldingForMaxPowerSound) {
			audioComponent?.Play(MAX_POWER, 0.1f, AudioComponent.SFX_BUS);
		}

		TargetPhys.ApplyCentralImpulse(ShootDirection * CurrentHitPower);
		audioComponent?.Play(HIT, AudioComponent.SFX_BUS);

		EmitSignal(SignalName.Actioned);
	}

	public void EnableCollision (bool enable) {
		return;
	}
}
