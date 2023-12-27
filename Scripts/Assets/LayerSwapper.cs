using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class LayerSwapper : Node2D, IActionable {

	[Export]
	public int outLayer;

	// Traer aqui la parte de TriggerArea de cambio de layer. Sera parametro de los TriggerArea.
	public override void _Ready () {
		if (outLayer < 0 || outLayer > 32) {
			throw new Exception($"[{Name}] Outlayer param should have a value between 1 and 32. ({outLayer})");
		}
	}
	public void Action (EventData data) {
		GD.Print($"{Name}.Action()");
		Ball ball = PinballController.Instance.Ball;
		if (ball == null) {
			return;
		}

		GD.Print($"ball.SetLevel({outLayer})");

		ball.SetLevel(outLayer);
	}
}

