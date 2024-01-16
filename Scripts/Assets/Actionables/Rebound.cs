using Godot;
using System;
using System.Collections.Generic;

public partial class Rebound : ReboundBase {
	public override Vector2 CalculateImpulseDirection (Node2D node) {
		return Transform.BasisXform(Vector2.Right).Normalized();
	}

	public override void EmitParticles (Node2D node) {
		var vectorToNode = (node.GlobalPosition - GlobalPosition).Normalized();

		particleSystem.GlobalPosition = GlobalPosition + vectorToNode * 6 * Transform.Scale;

		particleSystem.LookAt(node.GlobalPosition);

		particleSystem.Restart();
	}
}
