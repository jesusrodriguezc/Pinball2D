using Godot;
using System;

public partial class Camera : Camera2D
{
	bool followingBall = false;

	private Vector2 BasePosition;
	private RandomNumberGenerator randomNumberGenerator;
	private float shakeStrength = 0f;
	
	[Export] private float randomStrength;
	[Export] private float shakeFade;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		randomNumberGenerator = new RandomNumberGenerator();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (shakeStrength > 0) {
			shakeStrength = Mathf.Lerp(shakeStrength, 0f, shakeFade * (float)delta);
			Offset = GetRandomShakeStrength();
		}
	}

	public void ApplyShake () {
		shakeStrength = randomStrength;
	}

	public Vector2 GetRandomShakeStrength () {
		return new Vector2(randomNumberGenerator.RandfRange(-shakeStrength, shakeStrength), randomNumberGenerator.RandfRange(-shakeStrength, shakeStrength));
	}



}
