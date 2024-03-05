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

	public readonly StringName PRESS = "Press";

	public DateTime? timeCurrentKeyPress;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {
		idleRotation = Rotation;
		rotationDuration = RotationFrameDuration / 120f;
		rotationSpeed = Mathf.DegToRad(RotationMax) / rotationDuration;

		if (isLeftFlipper) {
			hitRotation = Rotation - Mathf.DegToRad(RotationMax);
		} else {
			hitRotation = Rotation + Mathf.DegToRad(RotationMax);
		}

		_audioComponent = GetNode<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(PRESS, ResourceLoader.Load<AudioStream>("res://SFX/flipper_press.wav"));
		}
	}

	public override void _Input (InputEvent @event) {
		if (currentStatus == Status.GAMEOVER) {
			return;
		}

		if (isLeftFlipper) {
			bool isJustPressed = Input.IsActionJustPressed("Left_Flipper");
			bool isJustReleased = Input.IsActionJustReleased("Left_Flipper");

			if (!isJustPressed && !isJustReleased) {
				return;
			}
			buttonPressed = isJustPressed;
			if (buttonPressed) {
				timeCurrentKeyPress = DateTime.Now;
				_audioComponent?.Play(PRESS, AudioComponent.SFX_BUS);
			}
		}

		if (!isLeftFlipper) {
			bool isJustPressed = Input.IsActionJustPressed("Right_Flipper");
			bool isJustReleased = Input.IsActionJustReleased("Right_Flipper");

			if (!isJustPressed && !isJustReleased) {
				return;
			}
			buttonPressed = isJustPressed;
			if (buttonPressed) {
				timeCurrentKeyPress = DateTime.Now;
				_audioComponent?.Play(PRESS, AudioComponent.SFX_BUS);
			}
		}
	}

	public override void _Process (double delta) {

		Status lastStatus = currentStatus;
		switch (currentStatus) {
			case Status.IDLE:
				currentStatus = buttonPressed ? Status.MOVING_UP : Status.IDLE;
				break;
			case Status.MOVING_DOWN:
				currentStatus = buttonPressed ? Status.MOVING_UP : Status.MOVING_DOWN;
				break;
			case Status.MOVING_UP:
				break;
			case Status.STAY_UP:
				currentStatus = buttonPressed ? Status.STAY_UP : Status.MOVING_DOWN;
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

}