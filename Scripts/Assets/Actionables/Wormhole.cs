using Godot;
using System.Collections.Generic;

public partial class Wormhole : Area2D, IActionable {

	#region Audio
	private AudioComponent audioComponent;
	public readonly StringName IN_AUDIO = "IN";
	public readonly StringName OUT_AUDIO = "OUT";

	[Export] AudioStream InAudio;
	[Export] AudioStream OutAudio;

	#endregion
	public bool IsCollisionEnabled { get; set; } = true;
	[Export] public double Score { get; private set; }

	[Export] public Vector2 ShootDirection;
	[Export] public float ShootPower;
	private GpuParticles2D particleSystem;
	private ScoreComponent scoreComponent;
	private Timer exitTimer;
	[Export] public double TimeInsideAfterEntering;
	[Export] public Node2D finalPosition;
	private RigidBody2D CurrentTarget = null;

	[Signal] public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);
	public override void _Ready () {
		audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (audioComponent != null) {
			if (InAudio != null) audioComponent.AddAudio(IN_AUDIO, InAudio);
			if (OutAudio != null) audioComponent.AddAudio(OUT_AUDIO, OutAudio);
		}

		ShootDirection = ShootDirection.Normalized();

		particleSystem = GetNodeOrNull<GpuParticles2D>("ParticleSystem");

		BodyEntered += (node) => Action(new EventData() { Sender = node, Parameters = new Dictionary<StringName, object>() { { ITrigger.ENTERING, true }, { ITrigger.ACTIVATOR, node } } });

		scoreComponent = GetNodeOrNull<ScoreComponent>("ScoreComponent");

		if (scoreComponent != null) {
			scoreComponent.BaseScore = Score;
		}

		exitTimer = new();
		exitTimer.OneShot = true;
		AddChild(exitTimer);
		exitTimer.Timeout += () => { Exit(); };

	}
	public void Action (EventData data) {

		if (data == null) {
			return; 
		}
		if (exitTimer.TimeLeft > 0) { 
			return; 
		}

		if (!data.Parameters.TryGetValue(ITrigger.ACTIVATOR, out var activator) || activator is not RigidBody2D TargetBody) {
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

		PinballController.Instance.DisableAll<Wormhole>(0.5f + TimeInsideAfterEntering);

		PinballController.Instance.Ball.IgnoreCollision(true);
		PinballController.Instance.Ball.Teleport(new Vector2(10000, 10000), Vector2.Zero);
		PinballController.Instance.Ball.Hide();

		exitTimer.Start(TimeInsideAfterEntering);
	}

	public void Exit () {
		audioComponent.Play(OUT_AUDIO, AudioComponent.SFX_BUS);

		PinballController.Instance.Ball.Show();
		PinballController.Instance.Ball.IgnoreCollision(false);
		PinballController.Instance.Ball.Teleport(finalPosition.GlobalPosition, Vector2.Zero);

		CurrentTarget.ApplyCentralImpulse(ShootDirection * ShootPower);
		CurrentTarget = null;
	}

	public void EnableCollision (bool enable) {
		IsCollisionEnabled = enable;
	}

}

