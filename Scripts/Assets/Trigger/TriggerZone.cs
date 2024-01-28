using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TriggerBehaviour;
using static ITrigger;
using static EventManager;

public partial class TriggerZone : Area2D, ITrigger{
	public bool Triggered { get; set; }
	public List<Node2D> NodesInside;
	public EventManager eventManager;
	[Export] private Node2D[] triggeredNodes;
	[Export] private Color PaintingColor; 

	public override void _Ready () {
		eventManager = GetNode<EventManager>("/root/EventManager");

		NodesInside = new List<Node2D>();
		BodyEntered += TriggerZone_BodyEntered;
		BodyExited += TriggerZone_BodyExited;

		if (triggeredNodes == null || triggeredNodes.Length == 0) {
			triggeredNodes = GetChildren().OfType<Node2D>().ToArray();
			if (triggeredNodes.Length == 0) {
				GD.PrintErr($"TriggerArea ({Name}) has no triggered elements defined");
			}

			triggeredNodes = triggeredNodes.Where(node => node is IActionable).ToArray();
			if (triggeredNodes.Length == 0) {
				GD.PrintErr($"TriggerArea ({Name}) has no triggered actionables defined.");
			}
		}
	}

	private void TriggerZone_BodyEntered (Node2D body) {
		GD.Print("Hola");
		Triggered = true;
		if (!NodesInside.Contains(body)) {
			NodesInside.Add(body);
		}
		Trigger(new Dictionary<StringName, object>() { { ACTIVATOR, body }, { ENTERING, true } });
	}

	private void TriggerZone_BodyExited (Node2D body) {

		Trigger(new Dictionary<StringName, object>() { { ACTIVATOR, body }, { ENTERING, false } });
		Triggered = false;
		NodesInside.Remove(body);
	}

	public void Trigger (Dictionary<StringName, object> args = null) {

		if (args == null) {
			return;
		}

		if (!args.TryGetValue(ACTIVATOR, out var obj) || obj is not Node2D currentNode) {
			return;
		}

		if (!NodesInside.Contains(currentNode)) {
			GD.Print($" [TriggerZone.Trigger()]: Target {currentNode.Name} is not inside TriggerZone {Name} anymore.");
			return;
		}

		GD.Print($" -- {Name} TRIGGERED by {((Node2D)args[ACTIVATOR]).Name} -- ");
		eventManager.SendMessage(this, triggeredNodes, EventType.TRIGGER, args);

		Triggered = true;
	}
}

