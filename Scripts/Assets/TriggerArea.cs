using Godot;
using Pinball.Scripts.Utils;
using Pinball.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static EventManager;
using static ITrigger;
using static Shooter;
//using System.Collections.Generic;

public partial class TriggerArea : Area2D, ITrigger {


	[Export] public EventType Type;

	[ExportGroup("Input")]
	[Export] public Key inputButton;
	private bool isButtonHolded;

	[ExportGroup("Trigger")]
	[Export] public TriggerBehaviour Behaviour;
	[Export] public Node2D Target;
	private bool preassignedTarget;
	[Export] public Node2D[] triggeredNodes;
	[Export] public bool triggerOnEnter;
	private EventManager eventManager;
	private List<Node2D> NodesInside = new();


	private Timer holdTimer;
	[Export] public double HoldingTime;
	private Timer waitTimer;
	[Export] public double WaitingTime;

	// Called when the node enters the scene tree for the first time.

	public override void _Ready () {

		if (Type == EventType.ERROR) {
			throw new Exception($" TriggerArea with name {Name} of type ERROR");
		}
		preassignedTarget = Target != null;
		eventManager = GetNode<EventManager>("/root/EventManager");
		if (GetChildren().Count(child => (child is CollisionPolygon2D) || (child is CollisionShape2D)) == 0 ) {
			GD.PrintErr($"TriggerArea ({Name}) has no collision area defined");
		}

		if (triggeredNodes == null || triggeredNodes.Length == 0) {
			triggeredNodes = GetChildren().OfType<Node2D>().ToArray();
			if (triggeredNodes.Length == 0) {
				GD.PrintErr($"TriggerArea ({Name}) has no triggered elements defined");
			}

			foreach(var node in triggeredNodes) {
				GD.Print($"{Name} of type {node.GetType()}");
			}
			triggeredNodes = triggeredNodes.Where(node => node is IActionable).ToArray();
			if (triggeredNodes.Length == 0) {
				GD.PrintErr($"TriggerArea ({Name}) has no triggered elements defined 124321");
			}
			foreach (var node in triggeredNodes) {
				GD.Print($"Nodo {node.Name} enlazado a {Name}");
			}
		}

		PrepareEvents();
	}

	#region Inputs
	public override void _Input (InputEvent @event) {
		if (@event is not InputEventKey key) {
			return;
		}
		if (key.Keycode != inputButton) {
			return;
		}

		if (Target == null || (triggerOnEnter && !NodesInside.Contains(Target))) {
			return;
		}

		switch (Behaviour) {
			case TriggerBehaviour.STAY_AND_PRESS_KEY:
				ProcessPress(key.Pressed);
				break;
			case TriggerBehaviour.STAY_AND_HOLD_KEY:
				ProcessHold(key.Pressed);
				break;
			case TriggerBehaviour.STAY_PRESS_AND_WAIT:
				ProcessPressWait(key.Pressed);
				break;
			case TriggerBehaviour.STAY_HOLD_AND_WAIT:
				ProcessHoldWait(key.Pressed);
				break;
			default:
				GD.PrintErr($"Should not be here. BehaviourType {Behaviour} in _Input()");
				break;
		}
	}

	private void OnObjectEntered (Node2D node) {

		GD.Print($"{node.Name} : Hi {Name}. I want to enter ");

		Target ??= node;

		if (Target != node) {
			return;
		}


		NodesInside.Add(node);

		GD.Print($"{node.Name} just entered {Name} area. Nodes inside: {NodesInside.Count}");

		if (!triggerOnEnter) {
			return;
		}

		switch (Behaviour) {
			case TriggerBehaviour.INSTANTANEOUS:
				ProcessInstantaneous();
				break;
			case TriggerBehaviour.STAY_AND_WAIT:
				ProcessWait();
				break;
		}
	}

	private void OnObjectExited (Node2D node) {

		if (Target != null && preassignedTarget && Target != node) {
			return;
		}

		holdTimer?.Stop();
		waitTimer?.Stop();

		GD.Print($"{node.Name} just exited {Name} area");

		NodesInside.Remove(node);

		if (!preassignedTarget && Behaviour != TriggerBehaviour.INSTANTANEOUS) {
			Target = null;
		}

		if (triggerOnEnter) {
			return;
		}

		switch (Behaviour) {
			case TriggerBehaviour.INSTANTANEOUS:
				ProcessInstantaneous();
				break;
			case TriggerBehaviour.STAY_AND_WAIT:
				ProcessWait();
				break;
		}
	}

	#endregion Inputs
	#region Behaviour
	public void PrepareEvents() {

		BodyEntered += OnObjectEntered;
		BodyExited += OnObjectExited;

		if (HoldingTime > 0) {
			holdTimer = new Timer {
				OneShot = true,
				WaitTime = HoldingTime
			};
			AddChild(holdTimer);
		}
		else {
			if (Behaviour == TriggerBehaviour.STAY_HOLD_AND_WAIT) Behaviour = TriggerBehaviour.STAY_PRESS_AND_WAIT;
			if (Behaviour == TriggerBehaviour.STAY_AND_HOLD_KEY) Behaviour = TriggerBehaviour.STAY_AND_PRESS_KEY;
		}

		if (WaitingTime > 0) {
			waitTimer = new Timer() {
				OneShot = true,
				WaitTime = WaitingTime
			};
			AddChild(waitTimer);
		}
		else {
			if (Behaviour == TriggerBehaviour.STAY_HOLD_AND_WAIT) Behaviour = TriggerBehaviour.STAY_AND_HOLD_KEY;
			if (Behaviour == TriggerBehaviour.STAY_PRESS_AND_WAIT) Behaviour = Behaviour = TriggerBehaviour.STAY_AND_PRESS_KEY;
			if (Behaviour == TriggerBehaviour.STAY_AND_WAIT) Behaviour = Behaviour = TriggerBehaviour.INSTANTANEOUS;
		}

		switch (Behaviour) {
			case TriggerBehaviour.INSTANTANEOUS:
				if (triggerOnEnter) {
					BodyEntered += (node) => Trigger(new Dictionary<StringName, object> { { Instantaneous, true } });
				} else {
					BodyExited += (node) => Trigger(new Dictionary<StringName, object> { { Instantaneous, true } });
				}
				break;
			case TriggerBehaviour.STAY_AND_WAIT:
				waitTimer.Timeout += () => Trigger();
				break;
			case TriggerBehaviour.STAY_AND_PRESS_KEY: break;
			case TriggerBehaviour.STAY_AND_HOLD_KEY:
				holdTimer.Timeout += () => Trigger();
				break;
			case TriggerBehaviour.STAY_PRESS_AND_WAIT:
				waitTimer.Timeout += () => Trigger();
				break;
			case TriggerBehaviour.STAY_HOLD_AND_WAIT:
				holdTimer.Timeout += () => waitTimer.Start();
				waitTimer.Timeout += () => Trigger();
				break;

		}

	}

	// Si lo encuentra y es instantaneo, debe ser true.
	// Si no lo encuentra, debe ser true.
	// Si lo encuentra y no es instantaneo, debe ser false.
	public void Trigger(Dictionary<StringName, object> args = null) {
		object isInstantaneousObj = null;
		bool isInstantaneous = args?.TryGetValue(Instantaneous, out isInstantaneousObj) ?? false;
		if (isInstantaneous) {
			isInstantaneous = (bool)isInstantaneousObj;
		}

		if (!NodesInside.Contains(Target) && !isInstantaneous) {
			GD.Print($" [TriggerArea.Trigger()]: Target {Target.Name} is not inside TriggerArea {Name} anymore.");
			return;
		}

		if (triggerOnEnter) {
			NodesInside.Remove(Target);
		}

		if (!preassignedTarget && Behaviour == TriggerBehaviour.INSTANTANEOUS) {
			Target = null;
		}

		eventManager.SendMessage(this, triggeredNodes, Type, args);		
	}



	public void ProcessInstantaneous() {
		Trigger();
	}
	public void ProcessWait () {
		if (waitTimer.TimeLeft == 0) {
			waitTimer.Start();
		}
	}
	public void ProcessPress (bool isPressed) {
		if (isPressed) {
			Trigger();
		}
	}

	public void ProcessHold (bool isPressed) {
		bool timeRunning = holdTimer.TimeLeft > 0 && !holdTimer.IsStopped();

		if (isPressed && !timeRunning) {
			holdTimer.Start();
		}

		if (!isPressed) {
			if (timeRunning) {
				double timeLeft = holdTimer.TimeLeft;
				holdTimer.Stop();

				float holdPerc = (float)Mathf.Min(1, (HoldingTime - timeLeft) / HoldingTime);
				

				Trigger(new Dictionary<StringName, object> { { ITrigger.HoldPercentage, holdPerc } });

			}
		}
	}
	public void ProcessPressWait (bool isPressed) {
		if (isPressed && waitTimer.TimeLeft == 0) {
			waitTimer.Start();
		}
	}
	public void ProcessHoldWait (bool isPressed) {
		ProcessHold(isPressed);
	}

	#endregion Behaviour
}
