using Godot;
using Pinball.Utils;
using System.Collections.Generic;

public partial class Slingshot : ReboundBase {

	public readonly StringName CLICK = "Click";

	private Vector2 perfectDirection;
	private Vector2 horizontal;
	private Vector2 lastDirImpulso;
	private Vector2 lastHitVector;

	/// <summary>
	/// Determines if the hit power is distributed evenly over its surface or it hit harder at the center and softer at the ends.
	/// </summary>
	[Export]
	public bool EvenlyDistributedPower { get; set; }

	public override void _Ready () {
		base._Ready ();

		perfectDirection = Transform.BasisXform(Vector2.Right);

		//perfectDirection = new Vector2(Mathf.Sin(Rotation), -Mathf.Cos(Rotation));
		//lastHitVector = perfectDirection.Rotated(Mathf.Pi / 2);
		//perfectDirection = (perfectDirection + 2 * lastHitVector).Normalized();
	}

	public override void _Process (double delta) {
		QueueRedraw();
	}

	public override Vector2 CalculateImpulseDirection (Node2D hittedElement) {
		Vector2 hitVector = _collisionArea.GlobalPosition.DirectionTo(hittedElement.GlobalPosition);
		lastHitVector = hitVector;

		var vectorProximity = Mathf.Abs(perfectDirection.Dot(hitVector)); // [0, 1]

		Vector2 golpeRecto, golpeLateral;
		if (EvenlyDistributedPower) {
			golpeRecto = perfectDirection * 4f;
			golpeLateral = hitVector * 0.5f;
		} else {
			var forceDistributionPerc = Mathx.SlingshotDistribution(vectorProximity);
			golpeRecto = perfectDirection * forceDistributionPerc;
			golpeLateral = hitVector * (1 - forceDistributionPerc);
		}

		return (golpeRecto + golpeLateral) / 2f;
	}

	public override void EmitParticles (Node2D node) {
		particleSystem.Restart();
	}
}


