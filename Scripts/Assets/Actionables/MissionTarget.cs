using Godot;
using System;

public partial class MissionTarget : Target {
	[Export] public int MissionId { get; set; }
	public DateTime PressedTime;

	public override void Collision (Node2D node) {
		base.Collision(node);
		PressedTime = DateTime.Now;
	}
}
