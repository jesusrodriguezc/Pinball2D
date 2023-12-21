using Godot;
using System;

public partial class Camera : Camera2D
{
	bool followingBall = false;

	private Vector2 BasePosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BasePosition = GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (followingBall) {
			GlobalPosition = PinballController.Instance.Ball.GlobalPosition;
		}
	}

	public override void _Input (InputEvent @event) {
		if (@event is InputEventKey key) {
			if (!key.Pressed) {
				return;
			}
			switch (key.Keycode) {
				case Key.Enter:
					if (followingBall) {
						GlobalPosition = BasePosition;
					}

					followingBall = !followingBall;
					break;
					
			}
		}

	}


}
