using Godot;
using Godot.Collections;
using System;
//using System.Collections.Generic;

public partial class TriggerArea : Area2D
{
	public enum TriggerType {
		SHOW_HIDE,
		COLLISION
	}

	[Export]
	public TriggerType Type;
	[Export]
	public int inLayer;
	[Export]
	public int outLayer;
	[Export]
	public float timeoutToTrigger;

	// Called when the node enters the scene tree for the first time.

	public override void _Ready () {
		if (inLayer <= 0 || inLayer > 32) throw new Exception($"[{Name}] inLayer debe estar entre 1 y 32. Su valor es {inLayer}");
		if (outLayer <= 0 || outLayer > 32) throw new Exception($"[{Name}] outLayer debe estar entre 1 y 32. Su valor es {outLayer}");

		BodyExited += ChangeLayer;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
	}

	public void ChangeLayer (Node2D node) {
		if (!node.GetType().Equals(typeof(Ball))) {
			return;
		}

		Ball ball = (Ball)node;

		if (ball.GetCollisionMaskValue(inLayer)) 
		{
			GD.Print($"Entramos a la zona [{Name}]");
			((Ball)node).SetLevel(outLayer);
		}
		else 
		{
			GD.Print($"Salimos de la zona [{Name}]");
			((Ball)node).SetLevel(outLayer);
		}


	}
}
