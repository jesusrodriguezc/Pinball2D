using Godot;
using System;

public partial class Flipper : AnimatableBody2D
{
	public enum Status {
		IDLE = 0,
		MOVING_UP = 1,
		STAY_UP = 2,
		MOVING_DOWN = 3,
		GAMEOVER = 4
	}

	[Export]
	public bool isLeftFlipper;
	[Export]
	public Status currentStatus;

	// Rotation
	[Export]
	public float RotationMax { get; set; }   // in degrees

	[Export]
	public float RotationFrameDuration { get; set; } // in frames

	public float rotationDuration;  // in seconds
	public float rotationSpeed;
	public float idleRotation;
	public float hitRotation;

	// Input

	public bool buttonPressed = false;

	public override void _Input (InputEvent @event) {
		if (currentStatus == Status.GAMEOVER) {
			return;
		}
		//if (@event is InputEventMouseButton mouseButton) {
		//	if (isLeftFlipper && mouseButton.ButtonIndex == MouseButton.Left) {
		//		buttonPressed = mouseButton.IsPressed();
		//	}

		//	if (!isLeftFlipper && mouseButton.ButtonIndex == MouseButton.Right) {
		//		buttonPressed = mouseButton.IsPressed();
		//	}
		//}

		if (@event is InputEventKey key) {
			switch (key.Keycode) {
				case Key.Q:
					if (isLeftFlipper) {
						buttonPressed = key.IsPressed();
					}
					break;
				case Key.E:
					if (!isLeftFlipper) {
						buttonPressed = key.IsPressed();
					}
					break;
			}
		}

		//if (@event is InputEventKey keyButton) {

		//	if (keyButton.Keycode == Key.Q && isLeftFlipper) {
		//		buttonPressed = keyButton.IsPressed();
		//		//GD.Print(string.Format("Space key {0}", spacePressed ? "pressed" : "released"));
		//		//currentStatus = Status.MOVING_UP;
		//	}
		//	if (keyButton.Keycode == Key.E && !isLeftFlipper) {
		//		buttonPressed = keyButton.IsPressed();
		//		//GD.Print(string.Format("Space key {0}", spacePressed ? "pressed" : "released"));
		//		//currentStatus = Status.MOVING_UP;
		//	}
		//}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//currentStatus = Status.IDLE;

		idleRotation = Rotation;
		rotationDuration = RotationFrameDuration / 120f;
		rotationSpeed = Mathf.DegToRad(RotationMax) / rotationDuration;

		if (isLeftFlipper) {
			hitRotation = Rotation - Mathf.DegToRad(RotationMax);
		}
		else {
			hitRotation = Rotation + Mathf.DegToRad(RotationMax);
		}

	}

	public override void _Process (double delta) {
		
		switch(currentStatus) {
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
	public override void _PhysicsProcess(double delta)
	{
		// Cada frame fisico, el flipper tendra que moverse:
		// + deltaSpeed si esta en HITING
		// - deltaSpeed si esta en RETURNING
		// Si esta cerca de alguna de las posiciones finales, se fijara la rotación y cambiara a la siguiente  IDLE -> MOVING_UP -> STAY_UP -> MOVING_DOWN -> IDLE

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

		if (IsAlmostEqualUnsigned(Rotation, idleRotation, deltaRotation)) {
			Rotation = idleRotation;
			currentStatus = Status.IDLE;
			return;
		}
	}

	private void PhysicsProcess_StatusMovingUp (double delta) {

		float deltaRotation = isLeftFlipper? -rotationSpeed * (float)delta : rotationSpeed * (float)delta;

		Rotation += deltaRotation;

		if (IsAlmostEqualUnsigned(Rotation, hitRotation, deltaRotation)) {
			Rotation = hitRotation;
			currentStatus = Status.STAY_UP;
			return;
		}
	}

	private void IdlePhysicsProcess (double delta) {
		// No debe hacer nada.
		return;
	}

	public bool IsAlmostEqualUnsigned(float x, float y, float maxDiff = 0.5f) {
		return Mathf.Abs(x - y) <= Mathf.Abs(maxDiff);
	}

	public bool IsAlmostEqualAngleUnsigned (float x, float y, float maxDiff = 0.5f) {
		float fixedX = x % 360;
		float fixedY = y % 360;

		return Mathf.Abs(x - y) <= Mathf.Abs(maxDiff);
	}

}
