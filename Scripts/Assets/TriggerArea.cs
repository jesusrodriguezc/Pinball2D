using Godot;
using System;
//using System.Collections.Generic;

public partial class TriggerArea : Area2D {
	public enum TriggerType {
		ON_ENTER,
		ON_EXIT
	}

	[Export]
	public TriggerType Type;
	[Export]
	public int outLayer;
	[Export]
	public float timeoutToTrigger;

	[ExportGroup("Sound")]
	[Export]
	public AudioStream audio;
	private AudioStreamPlayer2D audioStreamPlayer;

	[ExportGroup("Debug")]
	[Export] bool showBallSpeed;


	// Called when the node enters the scene tree for the first time.

	public override void _Ready () {
		if (outLayer < 0 || outLayer > 32) throw new Exception($"[{Name}] outLayer debe estar entre 1 y 32. Su valor es {outLayer}");

		switch (Type) {
			case TriggerType.ON_ENTER:
				BodyEntered += ChangeLayer;
				break;
			case TriggerType.ON_EXIT:
				BodyExited += ChangeLayer;
				break;

		}

		audioStreamPlayer = GetNodeOrNull<AudioStreamPlayer2D>("AudioStreamPlayer");
		audioStreamPlayer?.Set("stream", audio);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
	}

	public void ChangeLayer (Node2D node) {
		if (!node.GetType().Equals(typeof(Ball))) {
			return;
		}

		Ball ball = (Ball)node;
		ball.SetLevel(outLayer);
		audioStreamPlayer?.Play();

		if (showBallSpeed) {
			GD.Print($"Ball speed: {ball.LinearVelocity.Length()}");
		}

	}
}
