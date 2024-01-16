using Godot;
using System;
using System.Collections.Generic;

public partial class Teleport : Node2D, IActionable {

	[Export] public bool StoppedAfterTeleport;

	public bool IsCollisionEnabled { get; set; } = true;
	private EventData currentEvent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentEvent = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess (double delta) {
		if (currentEvent != null) {
			RigidBody2D activador = (RigidBody2D)currentEvent.Parameters[ITrigger.ACTIVATOR];
			Vector2 position = (Vector2)currentEvent.Parameters[ITrigger.POSITION];
			Vector2 velocity = (Vector2)currentEvent.Parameters[ITrigger.VELOCITY];

			((IActor)activador).Teleport(position, velocity);
			currentEvent = null;
		}
	}

	public void Action (EventData data) {

		if (!data.Parameters.TryGetValue(ITrigger.ACTIVATOR, out var activator) || activator is not RigidBody2D node || activator is not IActor) {
			GD.PrintErr("[Teleport.Action()] There is no specified element to teleport.");
			return;
		}

		if (!data.Parameters.TryGetValue(ITrigger.VELOCITY, out var velocity) || velocity is not Vector2 finalVelocity) {
			GD.Print($"[Teleport.Action()] The linear velocity to apply to {node.Name} is not specified.");
			data.Parameters[ITrigger.VELOCITY] = StoppedAfterTeleport ? Vector2.Zero : node.LinearVelocity;
		} else {
			data.Parameters[ITrigger.VELOCITY] = finalVelocity;
		}

		if (!data.Parameters.TryGetValue(ITrigger.POSITION, out var position)) {
			GD.Print($"[Teleport.Action()] The position to apply to {node.Name} is not specified.");
			data.Parameters[ITrigger.POSITION] = GlobalPosition;
		} else {
			if (position is Vector2 finalPosition) {
				data.Parameters[ITrigger.POSITION] = finalPosition;
			} else {
				data.Parameters[ITrigger.POSITION] = GlobalPosition;
			}
		}

		currentEvent = data;
	}

	public void EnableCollision (bool enable) {
		return;
	}
}
