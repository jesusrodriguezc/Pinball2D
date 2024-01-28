using Godot;
using Pinball.Scripts.Utils;
using Pinball.Utils;
using System;
using System.Timers;

public partial class Flipper : AnimatableBody2D {
	public enum Status {
		IDLE = 0,
		MOVING_UP = 1,
		STAY_UP = 2,
		MOVING_DOWN = 3,
		GAMEOVER = 4
	}

	[Signal] public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);

	[Export]
	public bool isLeftFlipper;
	[Export]
	public Status currentStatus;

	// Rotation
	[Export]
	public float RotationMax { get; set; }   // in degrees

	[Export]
	public float RotationFrameDuration { get; set; } // in frames

	[Export] public float HitBoostPower { get; set; } // in N*s

	public float rotationDuration;  // in seconds
	public float rotationSpeed;
	public float idleRotation;
	public float hitRotation;

	// Input

	public bool buttonPressed = false;

	private AudioComponent _audioComponent;

	public readonly StringName HIT = "Hit";

	public CollisionShape2D collisionShape;
	public RectangleShape2D collisionRectangle;

	private Vector2 idleCollisionPosition;
	private Vector2 idleCollisionShapeSize;

	public Area2D HitBooster { get; private set; }
	public bool IsBallOnTop { get; private set; }
	public Node2D ElementOnTop { get; private set; }

	public DateTime? timeCurrentKeyPress;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {
		//currentStatus = Status.IDLE;

		idleRotation = Rotation;
		rotationDuration = RotationFrameDuration / 120f;
		rotationSpeed = Mathf.DegToRad(RotationMax) / rotationDuration;

		if (isLeftFlipper) {
			hitRotation = Rotation - Mathf.DegToRad(RotationMax);
		} else {
			hitRotation = Rotation + Mathf.DegToRad(RotationMax);
		}

		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(HIT, ResourceLoader.Load<AudioStream>("res://SFX/flipper_press.wav"));

		}

		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		if (collisionShape.Shape is RectangleShape2D) {
			collisionRectangle = (RectangleShape2D)collisionShape.Shape;
		}
		idleCollisionPosition = collisionShape.Position;
		idleCollisionShapeSize = collisionRectangle.Size;

		HitBooster = GetNode<Area2D>("HitBooster");
		HitBooster.BodyEntered += (node) => { IsBallOnTop = true; ElementOnTop = node; };
		HitBooster.BodyExited += (node) => { IsBallOnTop = false; ElementOnTop = null; };

	}

	private void TryHitBoost (Node2D body, double delta) {
		// Pepinazo en la normal del area.


		if (body == null) {
			return;
		}
		if (body is not IActor actor) {
			return;
		}

		if (timeCurrentKeyPress == null) {
			return;
		}

		if (currentStatus == Status.MOVING_UP && IsBallOnTop) {
			//var normalVectorCurrentFrame = Transform.BasisXform(Vector2.Up);

			double timeFromStart = (DateTime.Now - timeCurrentKeyPress.Value).TotalSeconds - delta;

			var theoreticalAngleNextFrame = rotationSpeed * (float)timeFromStart;

			var theorethicalNormalVector = Vector2.Up;//.Rotated(theoreticalAngleNextFrame - Rotation);

			EmitSignal(SignalName.Impulse, body, theorethicalNormalVector * HitBoostPower);
			IsBallOnTop = false;
			timeCurrentKeyPress = null;
		}
		

	}

	public override void _Input (InputEvent @event) {
		if (currentStatus == Status.GAMEOVER) {
			return;
		}

		if (isLeftFlipper) {
			bool isJustPressed = Input.IsActionJustPressed("Left_Action");
			bool isJustReleased = Input.IsActionJustReleased("Left_Action");

			if (!isJustPressed && !isJustReleased) {
				return;
			}
			buttonPressed = isJustPressed;
			if (buttonPressed) {
				timeCurrentKeyPress = DateTime.Now;
				_audioComponent?.Play(HIT, AudioComponent.SFX_BUS);
			}
		}

		if (!isLeftFlipper) {
			bool isJustPressed = Input.IsActionJustPressed("Right_Action");
			bool isJustReleased = Input.IsActionJustReleased("Right_Action");

			if (!isJustPressed && !isJustReleased) {
				return;
			}
			buttonPressed = isJustPressed;
			if (buttonPressed) {
				timeCurrentKeyPress = DateTime.Now;
				_audioComponent?.Play(HIT, AudioComponent.SFX_BUS);
			}
		}

		//if (@event is InputEventKey key) {
		//	switch (key.Keycode) {
		//		case Key.Q:
		//			if (!isLeftFlipper) {
		//				break;
		//			}
		//			if (buttonPressed == key.IsPressed()) {
		//				break;
		//			}
		//			buttonPressed = key.IsPressed();
		//			if (buttonPressed) {
		//				timeCurrentKeyPress = DateTime.Now;
		//				_audioComponent?.Play(HIT, AudioComponent.SFX_BUS);
		//			}
		//			break;
		//		case Key.E:
		//			if (isLeftFlipper) {
		//				break;
		//			}
		//			if (buttonPressed == key.IsPressed()) {
		//				break;
		//			}
		//			buttonPressed = key.IsPressed();
		//			if (buttonPressed) {
		//				timeCurrentKeyPress = DateTime.Now;
		//				_audioComponent?.Play(HIT, AudioComponent.SFX_BUS);

		//			}
		//			break;
		//	}
		//}
	}

	public override void _Process (double delta) {

		switch (currentStatus) {
			case Status.IDLE:
				currentStatus = buttonPressed ? Status.MOVING_UP : Status.IDLE;
				collisionRectangle.Size = idleCollisionShapeSize;
				collisionShape.Position = idleCollisionPosition;
				break;
			case Status.MOVING_DOWN:
				currentStatus = buttonPressed ? Status.MOVING_UP : Status.MOVING_DOWN;
				PrepareForCollision();
				//TryHitBoost(ElementOnTop); 
				break;
			case Status.MOVING_UP:
				PrepareForCollision();
				break;
			case Status.STAY_UP:
				currentStatus = buttonPressed ? Status.STAY_UP : Status.MOVING_DOWN;
				collisionRectangle.Size = idleCollisionShapeSize;
				collisionShape.Position = idleCollisionPosition; 
				break;
		}

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess (double delta) {
		switch (currentStatus) {
			case Status.IDLE:
				IdlePhysicsProcess(delta);
				break;
			case Status.MOVING_UP:
				PhysicsProcess_StatusMovingUp(delta);
				break;
			case Status.STAY_UP:
				PhysicsProcess_StatusStayUp(delta);
				break;
			case Status.MOVING_DOWN:
				PhysicsProcess_StatusMovingDown(delta);
				break;
			case Status.GAMEOVER:
				break;
		}

		//QueueRedraw();
	}

	private void PhysicsProcess_StatusStayUp (double delta) {
		// No hacer nada
		return;
	}

	private void PhysicsProcess_StatusMovingDown (double delta) {

		float deltaRotation = isLeftFlipper ? rotationSpeed * (float)delta : -rotationSpeed * (float)delta;

		Rotation += deltaRotation;

		if (Mathx.IsAlmostEqualUnsigned(Rotation, idleRotation, deltaRotation)) {
			Rotation = idleRotation;
			currentStatus = Status.IDLE;
			return;
		}
	}

	private void PhysicsProcess_StatusMovingUp (double delta) {
		TryHitBoost(ElementOnTop, delta);
		float deltaRotation = isLeftFlipper ? -rotationSpeed * (float)delta : rotationSpeed * (float)delta;

		Rotation += deltaRotation;

		if (Mathx.IsAlmostEqualUnsigned(Rotation, hitRotation, deltaRotation)) {
			Rotation = hitRotation;
			currentStatus = Status.STAY_UP;
			return;
		}
	}

	private void IdlePhysicsProcess (double delta) {
		// No debe hacer nada.
		return;
	}

	public void PrepareForCollision () {
		collisionRectangle.Size = idleCollisionShapeSize + new Vector2(10, 10);
		collisionShape.Position = idleCollisionPosition + (isLeftFlipper? new Vector2(5, 5) : new Vector2(-5, 5));
	}

}
