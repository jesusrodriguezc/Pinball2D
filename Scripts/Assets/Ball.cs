using Godot;
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

	public override void _Ready () {

		var bumperGroup = GetNodeOrNull("/root/Main/Bumpers");
		if (bumperGroup != null) {
			foreach (var bumper in bumperGroup.GetChildren().OfType<Bumper>()) {
				bumper.Impulse += (nodeAffected, impulse) => { if (nodeAffected == this) ApplyImpulse(impulse); };
			}
		}

		var slingshotGroup = GetNodeOrNull("/root/Main/Slingshots");
		if (slingshotGroup != null) {
			foreach (var slingshot in slingshotGroup.GetChildren().OfType<Slingshot>()) {
				slingshot.Impulse += (nodeAffected, impulse) => { if (nodeAffected == this) ApplyImpulse(impulse); };
			}
		}

		var shooterLane = GetNodeOrNull<ShooterLane>("/root/Main/ShooterLane");
		if (shooterLane != null) {
			shooterLane.Impulse += (nodeAffected, impulse) => { if (nodeAffected == this) ApplyImpulse(impulse); };
		}

		var deathZone = GetNodeOrNull<Deathzone>("/root/Main/Deathzone");
		if (deathZone != null) {
			deathZone.BodyEntered += _OnDeathzoneBodyEntered;
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

		currentStatus = Status.DEAD;

		EmitSignal(SignalName.Death, this);


	}

	private void Reset () {
		LinearVelocity = Vector2.Zero;
		GlobalPosition = initialPosition;

	}

	public void SetLevel (int level) {
		var collisionMaskActual = CollisionMask;
		CollisionMask &= 0b0011;

		SetCollisionMaskValue(level, true);

		ZIndex = LayerManager.GetZLevel(level);


	}
}
