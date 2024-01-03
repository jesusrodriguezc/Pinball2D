using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class LayerSwapper : Node2D, IActionable {

	[Export]
	public LayerId outLayer;
	private EventManager eventManager;

	public bool IsCollisionEnabled { get; set; } = true;

	public override void _Ready () {
		eventManager = GetNodeOrNull<EventManager>("/root/EventManager");
		LayerManager.UpdateActionables(GetTree().Root);

	}
	public void Action (EventData data) {
		Ball ball = PinballController.Instance.Ball;
		if (ball == null) {
			return;
		}

		GD.Print($" -- {Name} CHANGING LAYER OF {((Node2D)data.Parameters[ITrigger.ACTIVATOR]).Name} TO {outLayer} -- ");

		ball.SetLevel(outLayer);
		eventManager?.SendMessage(this, LayerManager.GetActionablesFromLayer(outLayer, GetTree().Root), EventManager.EventType.ENABLE, data.Parameters);
		eventManager?.SendMessage(this, LayerManager.GetActionablesOutOfLayer(outLayer, GetTree().Root), EventManager.EventType.DISABLE, data.Parameters);

	}

	public void EnableCollision (bool enable) {
		//IsCollisionEnabled = enable;
	}
}

