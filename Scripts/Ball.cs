using Godot;
using Pinball.Utils;
using System;
using System.Linq;
public partial class Ball : RigidBody2D
{
	public enum Status {
		GAMEOVER = -2,
		DEAD = -1,
		ON_SHOOTER_LANE = 0,
		IDLE = 1,
		MOVING = 2

	}


	[Signal]
	public delegate void ScoreEventHandler (double score);

	[Export]
	public int DebugPower { get; set; } = 3000;
	private bool DebugTeleport = false;


	public Status currentStatus;

	private Vector2 initialPosition;

	public override void _Ready () {

		var bumperGroup = GetNodeOrNull("/root/Main/Bumpers");
		if (bumperGroup != null) {
			foreach(var bumper in bumperGroup.GetChildren().OfType<Bumper>()) {
				bumper.Impulse += (impulse) => _OnBumperImpulse(impulse, bumper);
			}
		}

		var slingshotGroup = GetNodeOrNull("/root/Main/Slingshots");
		if (slingshotGroup != null) {
			foreach (var slingshot in slingshotGroup.GetChildren().OfType<Slingshot>()) {
				slingshot.Impulse += (impulse) => _OnSlingshotImpulse(impulse, slingshot);
			}
		}

		var shooterLane = GetNodeOrNull<ShooterLane>("/root/Main/ShooterLane");
		if (shooterLane != null) {
			shooterLane.Impulse += (impulse) => _OnShooterLaneImpulse(impulse, shooterLane);
		}

		

		initialPosition = GlobalPosition;
		currentStatus = Status.IDLE;
	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey key) {
			switch(key.Keycode) {
				case Key.Enter:
					if (key.Pressed && PinballController.Instance.Debug) {
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
	public override void _PhysicsProcess(double delta)
	{
		if (currentStatus == Status.DEAD && PinballController.Instance.LivesLeft > 0) {
			Reset();
			currentStatus = Status.ON_SHOOTER_LANE;
		}

		GD.Print($"Velocidad de la bola: {LinearVelocity}");
		if (LinearVelocity.LengthSquared() > 4000000f) {
			GD.Print("Te pasaste de pinche lanza. Bajamos revoluciones.");
			LinearVelocity = LinearVelocity.LimitLength(2000f);
		}
	}

	private void _OnBumperImpulse (Vector2 impulse, Bumper bumper) {

		ApplyImpulse(impulse);
		EmitSignal(SignalName.Score, bumper.BaseScore);

	}

	private void _OnSlingshotImpulse (Vector2 impulse, Slingshot slingshot) {

		ApplyImpulse(impulse);
		EmitSignal(SignalName.Score, slingshot.BaseScore);
	}

	private void _OnShooterLaneImpulse (Vector2 impulse, ShooterLane shooterLane) {

		ApplyImpulse(impulse);
	}

	private void ApplyImpulse(Vector2 impulse) {
		LinearVelocity = new Vector2(0f, 0f);
		//GD.Print($"Pepinazo de {impulse}");
		ApplyCentralImpulse(impulse);
	}

	public void Move ()
	{
		GD.Print($"Golpeamos la bola");

		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 forceDirection = (mousePosition - GlobalPosition).Normalized();
		
		ApplyCentralImpulse(forceDirection * DebugPower); //, Position - (Vector2) result["position"]);

	}
	
	private void _OnDeathzoneBodyEntered (Node2D body) {
		currentStatus = Status.DEAD;
	}

	private void Reset() {
		GD.Print($"Reset -  {GlobalPosition} -> {initialPosition}");
		LinearVelocity = Vector2.Zero;
		GlobalPosition = initialPosition;
		
	}

	public void SetLevel (int level) {
		// Nos cargamos todas las collision Mask excepto la fija.
		CollisionMask &= 0b0010;

		SetCollisionMaskValue(level, true);
	}
}



