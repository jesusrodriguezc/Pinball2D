using Godot;
using System;
using System.Collections.Generic;

public partial class Wormhole : Area2D, IActionable {
	[Signal] public delegate void ActionedEventHandler ();

	private PinballController pinballController;


	#region Audio
	private AudioComponent audioComponent;
	public readonly StringName IN_AUDIO = "IN";
	public readonly StringName OUT_AUDIO = "OUT";

	[Export] AudioStream InAudio;
	[Export] AudioStream OutAudio;

	#endregion
	[Export] public double Score { get; private set; }

	[Export] public Vector2 ShootDirection;
	[Export] public float ShootPower;
	private GpuParticles2D particleSystem;
	private ScoreComponent scoreComponent;
	private Timer exitTimer;
	[Export] public double TimeInsideAfterEntering;
	[Export] public Node2D finalPosition;
	private RigidBody2D CurrentTarget = null;
	private CooldownComponent cooldownComponent;

	private AnimationPlayer animationPlayer;

	[Export] public double Cooldown { get; set; }
	public bool IsCollisionEnabled { get; set; }

	[Signal] public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);
	public override void _Ready () {
		base._Ready();
		pinballController = GetNode<PinballController>("/root/Pinball");

		audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (audioComponent != null) {
			if (InAudio != null) audioComponent.AddAudio(IN_AUDIO, InAudio);
			if (OutAudio != null) audioComponent.AddAudio(OUT_AUDIO, OutAudio);
		}

		ShootDirection = ShootDirection.Normalized();

		particleSystem = GetNodeOrNull<GpuParticles2D>("ParticleSystem");

		BodyEntered += (node) => Action(new EventData() { Sender = node, Parameters = new Dictionary<StringName, object>() { { TriggerBase.ENTERING, true }, { TriggerBase.ACTIVATOR, node } } });

		scoreComponent = GetNodeOrNull<ScoreComponent>("ScoreComponent");

		if (scoreComponent != null) {
			scoreComponent.BaseScore = Score;
		}

		exitTimer = new();
		exitTimer.OneShot = true;
		AddChild(exitTimer);
		exitTimer.Timeout += () => { Teleport(); };

		cooldownComponent = GetNode<CooldownComponent>("CooldownComponent");
		cooldownComponent.SetCooldown(Cooldown);
		cooldownComponent.Timer.Timeout += () => { EnableCollision(true); };

		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

	}
	public void Action (EventData data) {

		if (data == null) {
			return; 
		}
		if (exitTimer.TimeLeft > 0) { 
			return; 
		}


		if (cooldownComponent.IsOnCooldown) {
			return;
		}

		if (!data.Parameters.TryGetValue(TriggerBase.ACTIVATOR, out var activator) || activator is not RigidBody2D TargetBody) {
			return;
		}
		
		if (!IsCollisionEnabled) {
			return;
		}

		CurrentTarget = TargetBody;
		Enter();
	}

	public void Enter() {
		audioComponent.Play(IN_AUDIO, AudioComponent.SFX_BUS);
		animationPlayer.Play("enter");

		pinballController.Ball.EnteringWormhole();
		pinballController.DisableAllFor<Wormhole>(1.5f + TimeInsideAfterEntering);

		exitTimer.Start(TimeInsideAfterEntering);
	}

	public void Teleport () {

		if (finalPosition is Wormhole wormhole) {
			wormhole.OnExit();
		}
		audioComponent.Play(OUT_AUDIO, AudioComponent.SFX_BUS);

		pinballController.Ball.ExitingWormhole(finalPosition.GlobalPosition);

		CurrentTarget.ApplyCentralImpulse(ShootDirection * ShootPower);
		CurrentTarget = null;


		cooldownComponent.ApplyCooldown();

		EmitSignal(SignalName.Actioned);
	}

	private void OnExit () {
		// Animación "exit"
		animationPlayer.Play("exit");
	}

	public void EnableCollision (bool enable) {
		IsCollisionEnabled = enable;
	}
}

