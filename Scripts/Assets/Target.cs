using Godot;
using System;

public partial class Target : ReboundBase {

	public override Vector2 CalculateImpulseDirection (Node2D node) {
		return Transform.BasisXform(Vector2.Left).Normalized();
	}

	public override void EmitParticles (Node2D node) {
		particleSystem.Restart();
	}
}
