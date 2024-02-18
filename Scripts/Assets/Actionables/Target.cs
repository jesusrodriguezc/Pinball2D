using Godot;
using System;

public partial class Target : ReboundBase, IGroupable {
	public bool Active { get; set; }
	public bool Blocked { get; set; }
	public Timer DisableTimer { get; private set; }

	public override void _Ready () {
		base._Ready();
		Reset();
		DisableTimer = new Timer() {
			Autostart = false,
			OneShot = true
		};

		AddChild(DisableTimer);
		DisableTimer.Timeout += () => {
			_lightAnimationPlayer.Stop();
			Active = false;
			Blocked = false;
		};
	}
	public override void Collision(Node2D node) {
		base.Collision(node);
		Active = true;
	}
	public override Vector2 CalculateImpulseDirection (Node2D node) {
		return Transform.BasisXform(Vector2.Left).Normalized();
	}

	public override void EmitParticles (Node2D node) {
		particleSystem.Restart();
	}

	public void Reset () {
		Active = false;
		Blocked = false;
	}
	public void OnCompleted (double duration) {
		DisableTimer.Start(duration);
		Reset();
	}
}
