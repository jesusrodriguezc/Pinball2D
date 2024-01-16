using Godot;
using System.Collections.Generic;

public partial class Bumper : ReboundBase  {

	public override Vector2 CalculateImpulseDirection (Node2D node) {
		return (node.GlobalPosition - GlobalPosition).Normalized();
	}

	public override void EmitParticles (Node2D node) {
		var vectorToNode = (node.GlobalPosition - GlobalPosition).Normalized();

		particleSystem.GlobalPosition = GlobalPosition + vectorToNode * 24 * Transform.Scale;

		particleSystem.LookAt(node.GlobalPosition);

		particleSystem.Restart();
	}
}

