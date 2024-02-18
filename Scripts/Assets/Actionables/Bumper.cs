using Godot;
using System.Collections.Generic;

public partial class Bumper : ReboundBase  {

	[Export] public UPGRADE_LEVEL CurrentLevel = UPGRADE_LEVEL.BASIC;

	public override void _Ready () {
		base._Ready();
		upgradeComponent.ChangeLevel(CurrentLevel);
	}
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

