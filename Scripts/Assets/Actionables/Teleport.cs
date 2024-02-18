using Godot;
using System;
using System.Collections.Generic;

public partial class Teleport : Node2D, IActionable {
	[Signal] public delegate void ActionedEventHandler ();

	[Export] public bool StoppedAfterTeleport;

	private EventData currentEvent;

	public bool IsCollisionEnabled { get; set; }

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
			RigidBody2D activador = (RigidBody2D)currentEvent.Parameters[TriggerBase.ACTIVATOR];
			Vector2 position = (Vector2)currentEvent.Parameters[TriggerBase.POSITION];
			Vector2 velocity = (Vector2)currentEvent.Parameters[TriggerBase.VELOCITY];

			((IActor)activador).Teleport(position, velocity);
			currentEvent = null;
		}
	}

	public void Action (EventData data) {

		if (!data.Parameters.TryGetValue(TriggerBase.ACTIVATOR, out var activator) || activator is not RigidBody2D node || activator is not IActor) {
			GD.PrintErr("[Teleport.Action()] There is no specified element to teleport.");
			return;
		}

		if (!data.Parameters.TryGetValue(TriggerBase.VELOCITY, out var velocity) || velocity is not Vector2 finalVelocity) {
			GD.PushWarning($"[Teleport.Action()] The linear velocity to apply to {node.Name} is not specified.");
			data.Parameters[TriggerBase.VELOCITY] = StoppedAfterTeleport ? Vector2.Zero : node.LinearVelocity;
		} else {
			data.Parameters[TriggerBase.VELOCITY] = finalVelocity;
		}

		if (!data.Parameters.TryGetValue(TriggerBase.POSITION, out var position)) {
			GD.PushWarning($"[Teleport.Action()] The position to apply to {node.Name} is not specified.");
			data.Parameters[TriggerBase.POSITION] = GlobalPosition;
		} else {
			if (position is Vector2 finalPosition) {
				data.Parameters[TriggerBase.POSITION] = finalPosition;
			} else {
				data.Parameters[TriggerBase.POSITION] = GlobalPosition;
			}
		}

		currentEvent = data;
	}

	public void EnableCollision (bool enable) {
		return;
	}
}
