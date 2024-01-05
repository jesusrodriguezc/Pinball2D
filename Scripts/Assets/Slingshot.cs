using Godot;
using Pinball.Utils;
using System.Collections.Generic;

[Tool]
public partial class Slingshot : ReboundBase {

	public readonly StringName CLICK = "Click";

	private Vector2 perfectDirection;
	private Vector2 horizontal;
	private Vector2 lastDirImpulso;

	/// <summary>
	/// Determines if the hit power is distributed evenly over its surface or it hit harder at the center and softer at the ends.
	/// </summary>
	[Export]
	public bool EvenlyDistributedPower { get; set; }

	public override void _Ready () {
		base._Ready ();
		
		perfectDirection = Transform.BasisXform(Vector2.Up);
		horizontal = Transform.BasisXform(Vector2.Left);
	}

	public override Vector2 CalculateImpulseDirection (Node2D hittedElement) {
		Vector2 hitVector = (hittedElement.GlobalPosition - _collisionArea.GlobalPosition).Normalized();
		var vectorProximity = perfectDirection.Dot(hitVector); // [0, 1]

		Vector2 golpeRecto, golpeLateral;
		if (EvenlyDistributedPower) {
			golpeRecto = perfectDirection * 4f;
			golpeLateral = hitVector * 0.5f;
		} 
		else {
			var forceDistributionPerc = Mathx.SlingshotDistribution(vectorProximity);
			golpeRecto = perfectDirection * forceDistributionPerc;
			golpeLateral = hitVector * (1 - forceDistributionPerc) * 0.5f;
		}

		return (golpeRecto + golpeLateral) / 2f;
	}

	public override void EmitParticles (Node2D node) {
		particleSystem.Restart();
	}
}


