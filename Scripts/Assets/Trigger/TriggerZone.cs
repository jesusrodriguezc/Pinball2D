using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static EventManager;
using static TriggerBase;

public partial class TriggerZone : TriggerBase{
	[Export] public TriggerZone[] IgnoredZones { get; private set; }
	[Export] public double Cooldown { get; set; }

	public List<Node2D> NodesInside;
	public EventManager eventManager;
	[Export] private Node2D[] triggeredNodes;
	[Export] private Color PaintingColor;
	private CooldownComponent cooldownComponent;

	public override void _Ready () {
		eventManager = GetNode<EventManager>("/root/EventManager");

		NodesInside = new List<Node2D>();
		BodyEntered += TriggerZone_BodyEntered;
		BodyExited += TriggerZone_BodyExited;

		cooldownComponent = GetNode<CooldownComponent>("CooldownComponent");
		cooldownComponent.SetCooldown(Cooldown);

		if (triggeredNodes != null && triggeredNodes.Length != 0) {
			return;
		}

		triggeredNodes = GetChildren().OfType<Node2D>().ToArray();
		if (triggeredNodes.Length == 0) {
			GD.PushWarning($"TriggerArea ({Name}) has no triggered elements defined");
		}

		triggeredNodes = triggeredNodes.Where(node => node is IActionable).ToArray();
		if (triggeredNodes.Length == 0) {
			GD.Print($"TriggerArea ({Name}) has no triggered actionables defined.");
		}

		if (PaintingColor.A <= 255f) {
			var mapGenerator = new MapGenerator {
				paintingColor = PaintingColor,
				collisionShapes = GetChildren().OfType<CollisionShape2D>().ToArray(),
				collisionPolygons = GetChildren().OfType<CollisionPolygon2D>().ToArray()
			};
			AddChild(mapGenerator);
			mapGenerator.EnableCollision(false);
		}
	}

	private void TriggerZone_BodyEntered (Node2D body) {
		if (body is not IActor actor) {
			return;
		}
		IsTriggered = true;
		if (!NodesInside.Contains(body)) {
			NodesInside.Add(body);
		}

		List<TriggerZone> currentZones = actor.CurrentTriggerZones;
		if (currentZones.Any(zone => zone.IgnoredZones.Contains(this))) {
			return;
		}

		actor.CurrentTriggerZones.Add(this);
		Trigger(new EventData() {
			Sender = body,
			Parameters = new Dictionary<StringName, object>() { { ACTIVATOR, body }, { ENTERING, true } }
		});
	}

	private void TriggerZone_BodyExited (Node2D body) {

		if (body is not IActor actor) {
			return;
		}

		Trigger(new EventData() { 
			Sender = body,
			Parameters = new Dictionary<StringName, object>() { { ACTIVATOR, body }, { ENTERING, false } } 
		});

		IsTriggered = false;
		NodesInside.Remove(body);
		actor.CurrentTriggerZones.Remove(this);

	}

	public override void Trigger (EventData data) {

		if (cooldownComponent.IsOnCooldown) {
			return;
		}


		if (data.Parameters == null) {
			return;
		}

		if (!data.Parameters.TryGetValue(ACTIVATOR, out var obj) || obj is not Node2D currentNode) {
			return;
		}

		if (!NodesInside.Contains(currentNode)) {
			GD.PushWarning($" [TriggerZone.Trigger()]: Target {currentNode.Name} is not inside TriggerZone {Name} anymore.");
			return;
		}

		base.Trigger(data);

		if (triggeredNodes != null && triggeredNodes.Length > 0) {
			eventManager.SendMessage(this, triggeredNodes, EventType.TRIGGER, data.Parameters);
		}

		IsTriggered = true;
	}
}

