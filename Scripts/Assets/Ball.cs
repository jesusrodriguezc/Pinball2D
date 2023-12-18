using Godot;
using Pinball.Scripts.Utils;
using System.Linq;
using static PinballController;

public partial class Ball : RigidBody2D {
	public enum Status {
		GAMEOVER = -2,
		DEAD = -1,
		ON_SHOOTER_LANE = 0,
		IDLE = 1,
		MOVING = 2

	}

	[Signal]
	public delegate void DeathEventHandler (Ball ball);

	[Export]
	public int DebugPower { get; set; } = 3000;
	private bool DebugTeleport = false;

	[Export]
	public float MaxVelocity { get; set; }


	public Status currentStatus;

	private Vector2 initialPosition;
	private AudioComponent _audioComponent;
	private readonly StringName DEAD = "Dead";

	public override void _Ready () {

		var bumpers = Nodes.findByClass<Bumper>(GetTree().Root);
		foreach (var bumper in bumpers) {
			bumper.Impulse += (nodeAffected, impulse) => { if (nodeAffected == this) ApplyImpulse(impulse); };
		}

		var slingshots = Nodes.findByClass<Slingshot>(GetTree().Root);
		foreach (var slingshot in slingshots) {
			slingshot.Impulse += (nodeAffected, impulse) => { if (nodeAffected == this) ApplyImpulse(impulse); };
		}

		var shooterLanes = Nodes.findByClass<ShooterLane>(GetTree().Root);
		foreach (var shooterLane in shooterLanes) {
			shooterLane.Impulse += (nodeAffected, impulse) => { if (nodeAffected == this) ApplyImpulse(impulse); };
		}

		var deathZones = Nodes.findByClass<Deathzone>(GetTree().Root);
		foreach (var deathZone in deathZones) {
			deathZone.BodyEntered += _OnDeathzoneBodyEntered;
		}

		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(DEAD, ResourceLoader.Load<AudioStream>("res://SFX/deathzone_enter.wav"));

		}

		initialPosition = GlobalPosition;
		currentStatus = Status.IDLE;
	}
	public override void _Input (InputEvent @event) {
		if (@event is InputEventKey key) {
			switch (key.Keycode) {
				case Key.Enter:
					if (key.Pressed && Instance.Debug) {
						Reset();
					}
					break;
			}
		}

		if (@event is InputEventMouseButton mouseButton) {
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.IsPressed()) {
				Move();
			}
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess (double delta) {
		if (currentStatus == Status.DEAD && Instance.LivesLeft > 0) {
			Reset();
			currentStatus = Status.ON_SHOOTER_LANE;
		}

		if (!GetCollisionMaskValue(5) && LinearVelocity.LengthSquared() > (MaxVelocity * MaxVelocity)) {
			LinearVelocity = LinearVelocity.LimitLength(MaxVelocity);
		}
	}

	private void ApplyImpulse (Vector2 impulse) {
		LinearVelocity = new Vector2(0f, 0f);
		ApplyCentralImpulse(impulse);
	}

	public void Move () {
		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 forceDirection = (mousePosition - GlobalPosition).Normalized();

		ApplyCentralImpulse(forceDirection * DebugPower); //, Position - (Vector2) result["position"]);

	}

	private void _OnDeathzoneBodyEntered (Node2D body) {
		if (body != this) return;

		_audioComponent?.Play(DEAD, AudioComponent.SFX_BUS);

		currentStatus = Status.DEAD;

		EmitSignal(SignalName.Death, this);


	}

	private void Reset () {
		LinearVelocity = Vector2.Zero;
		GlobalPosition = initialPosition;

	}

	public void SetLevel (int level) {
		if (level == 0) { return; }

		GD.Print($"[Pre-Ball.SetLevel({level})] CollisionMask = {CollisionMask} - ZIndex = {ZIndex}");

		//var collisionMaskActual = CollisionMask;
		CollisionMask &= 0b0011;

		SetCollisionMaskValue(level, true);

		ZIndex = LayerManager.GetZLevel(level);

		GD.Print($"[Post-Ball.SetLevel({level})] CollisionMask = {CollisionMask} - ZIndex = {ZIndex}");


	}
}
