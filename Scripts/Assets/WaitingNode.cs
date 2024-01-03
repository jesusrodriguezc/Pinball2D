using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static EventManager;

public partial class WaitingNode : Node2D, IActionable, ITrigger
{

	[Export] public double waitingTime;
	private EventManager eventManager;
	private Timer waitingTimer;
	private Node2D[] triggeredNodes;
	private EventData currentEvent;
	public bool IsCollisionEnabled { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		if (waitingTime <= 0) {
			GD.Print($"[WARNING] WaitingNode {Name} has a value of waitingTime <= 0 ({waitingTime}).");
		} else {
			waitingTimer = new Timer {
				OneShot = true,
				WaitTime = waitingTime
			};
			AddChild(waitingTimer);

			waitingTimer.Timeout += () => Trigger();
		}

		eventManager = GetNode<EventManager>("/root/EventManager");

		if (triggeredNodes == null || triggeredNodes.Length == 0) {
			triggeredNodes = GetChildren().OfType<Node2D>().ToArray();
			if (triggeredNodes.Length == 0) {
				GD.PrintErr($"WaitingNode ({Name}) has no triggered elements defined");
			}

			triggeredNodes = triggeredNodes.Where(node => node is IActionable).ToArray();
			if (triggeredNodes.Length == 0) {
				GD.PrintErr($"WaitingNode ({Name}) has no triggered actionables defined.");
			}
		}

	}
	public void Action (EventData data) {
		GD.Print($"Starting {Name} timer of {waitingTime} seconds.");
		currentEvent = data;
		if (waitingTime > 0) {
			waitingTimer.Start();
		} else {
			Trigger();
		}
	}

	public void EnableCollision (bool enable) {
		return;
	}

	public void Trigger (Dictionary<StringName, object> args = null) {
		eventManager.SendMessage(this, triggeredNodes, EventType.TRIGGER, currentEvent.Parameters);

		currentEvent = null;
	}

	public void OnTargetStopped (Node2D node) {
		return;
	}
}
