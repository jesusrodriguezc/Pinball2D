using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class LayerSwapper : Node2D, IActionable {
	[Signal] public delegate void ActionedEventHandler ();
	[Export] public LayerId insideLayer;
	[Export] public LayerId outLayer;
	private PinballController pinballController;
	private EventManager eventManager;

	public bool IsCollisionEnabled { get; set; }

	public override void _Ready () {
		pinballController = GetNode<PinballController>("/root/Pinball");
		eventManager = GetNodeOrNull<EventManager>("/root/EventManager");
		LayerManager.UpdateActionables(GetTree().Root);

	}
	public void Action (EventData data) {
		Ball ball = pinballController.Ball;
		if (ball == null) {
			return;
		}

		bool isEntering = false;
		if (data.Parameters.TryGetValue(TriggerBase.ENTERING, out var obj) && obj is bool boolean) {
			isEntering = boolean;
		}

		var newLayer = isEntering ? insideLayer : outLayer;
		ball.SetLevel(newLayer);
		eventManager?.SendMessage(this, LayerManager.GetActionablesFromLayer(newLayer, GetTree().Root), EventManager.EventType.ENABLE, data.Parameters);
		eventManager?.SendMessage(this, LayerManager.GetActionablesOutOfLayer(newLayer, GetTree().Root), EventManager.EventType.DISABLE, data.Parameters);

	}

	public void EnableCollision (bool enable) {
		return;
	}
}

