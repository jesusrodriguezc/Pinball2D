using Godot;
using Pinball.Utils;
using System;
using System.Collections.Generic;

public partial class Shooter : Area2D {

	public enum InputType {
		PRESS_KEY,
		HOLD_KEY,
		PRESS_AND_WAIT,
		HOLD_AND_WAIT
	}
	private Ball currBall;
	private Area2D collisionArea;

	#region Audio
	private AudioComponent audioComponent;
	public readonly StringName HIT = "Hit";
	public readonly StringName MAX_POWER = "MAX_POWER";

	[Export] AudioStream HitAudio;
	[Export] AudioStream MaxPowerAudio;

	#endregion
	[Export] public int MaxHitPower { get; set; }
	[Export] public Node2D Target;
	private List<Node2D> NodesInside = new List<Node2D>();
	[Export] public Vector2 ShootDirection;
	bool showIntoTarget;

	[Signal]
	public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);

	[ExportGroup("Input")]

	[Export] public InputType inputType;
	[Export] public Key inputButton;
	private bool isButtonHolded;  

	private Timer holdTimer;
	[Export] public double HoldTime;
	private Timer waitTimer;
	[Export] public double WaitTime;
	[Export] public double HoldingForMaxPowerSound;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {

		audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		//collisionArea = GetNodeOrNull<Area2D>("CollisionArea");

		if (HitAudio != null) audioComponent?.AddAudio(HIT, HitAudio);
		if (MaxPowerAudio != null) audioComponent?.AddAudio(MAX_POWER, MaxPowerAudio);
		showIntoTarget = (ShootDirection == Vector2.Zero);

		if (HoldingForMaxPowerSound <= 0f) HoldingForMaxPowerSound = HoldTime;

		BodyEntered += OnObjectEntered;
		BodyExited += OnObjectExited;

		PrepareTimers();
		QueueRedraw();
	}

	private void PrepareTimers () {
		holdTimer = new Timer {
			OneShot = true
		};
		AddChild(holdTimer);

		waitTimer = new Timer() {
			OneShot = true
		};
		AddChild(waitTimer);

		switch (inputType) {
			case InputType.PRESS_KEY: break;
			case InputType.HOLD_KEY:
				holdTimer.Timeout += () => Action();
				break;
			case InputType.PRESS_AND_WAIT:
				waitTimer.Timeout += () => Action();
				break;
			case InputType.HOLD_AND_WAIT:
				holdTimer.Timeout += () => { if (WaitTime >= 0f) waitTimer.Start(WaitTime); else { Action(); } };
				waitTimer.Timeout += () => Action();
				break;

		}
	}

	public override void _Input (InputEvent @event) {
		if (@event is not InputEventKey key) {
			return;
		}
		if (key.Keycode != inputButton) {
			return;
		}

		if (Target == null || !NodesInside.Contains(Target)) {
			return;
		}

		switch (inputType) {
			case InputType.PRESS_KEY:
				ProcessPress(key.Pressed);
				break;
			case InputType.HOLD_KEY:
				ProcessHold(key.Pressed);
				break;
			case InputType.PRESS_AND_WAIT: 
				ProcessPressWait(key.Pressed);
				break;
			case InputType.HOLD_AND_WAIT:
				ProcessHoldWait(key.Pressed);
				break;
		}
	}



	private void ProcessPress (bool isPressed) {
		if (isPressed) {
			Action();
		}
	}

	private void ProcessHold (bool isPressed) {
		bool timeRunning = holdTimer.TimeLeft > 0 && !holdTimer.IsStopped();

		if (isPressed && !timeRunning) {
			GD.Print($"Empieza el contador a {HoldTime}");
			holdTimer.Start(HoldTime); 
		}

		if (!isPressed ) {
			if (timeRunning) {
				double timeLeft = holdTimer.TimeLeft;
				holdTimer.Stop();

				GD.Print($"Parado antes de tiempo: {timeLeft}");
				float holdPerc = (float)Mathf.Min(1, (HoldTime - timeLeft) / HoldTime);
				float CurrentHitPower = MaxHitPower * Mathx.FuncSmooth(holdPerc);

				Action(CurrentHitPower);
				if (holdPerc >= HoldingForMaxPowerSound) {
					audioComponent?.Play(MAX_POWER, 0.1f);
				}
			}
		}
	}
	private void ProcessPressWait (bool isPressed) {
		if (isPressed && waitTimer.TimeLeft == 0) {
			waitTimer.Start(HoldTime);
		}
	}
	private void ProcessHoldWait (bool isPressed) {
		ProcessHold(isPressed);
	}

	private void OnObjectEntered (Node2D node) {

		if (Target != null && Target != node) {
			return;
		}

		NodesInside.Add(node);
	}

	private void OnObjectExited (Node2D node) {

		if (Target != null && Target != node) {
			return;
		}

		holdTimer.Stop();
		waitTimer.Stop();

		NodesInside.Remove(node);
	}

	// Se ejecutará cuando la acción esté completada.
	public void Action(float? hitPower = null) {
		
		if (showIntoTarget) {
			ShootDirection = GlobalPosition - Target.Position;
		}

		if (Target is not RigidBody2D TargetPhys) {
			return;
		}
		GD.Print($"Shooter.Action({ShootDirection * (hitPower ?? MaxHitPower)})");

		TargetPhys.ApplyCentralImpulse(ShootDirection * (hitPower ?? MaxHitPower));

		
	}

	public override void _Draw () {
		DrawLine(Vector2.Zero, ShootDirection * MaxHitPower, Colors.Yellow, 4f);
	}
}
