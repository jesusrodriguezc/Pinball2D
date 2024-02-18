using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static EventManager;
using static TriggerBase;
using static TriggerBehaviour;

public partial class TriggerArea : TriggerBase {
	private const float STILL_ELEMENT_MAX_VELOCITY = 0.1f;
	[Export] public EventType Type;

	[ExportGroup("Input")]
	[Export] public Key inputButton;
	private bool isButtonHolded;

	[ExportGroup("Trigger")]
	[Export] public TriggerBehaviourId Behaviour;
	private TriggerBehaviour behaviourInfo;

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
	private CooldownComponent cooldownComponent;
	[Export] public double Cooldown { get; set; }

	// Called when the node enters the scene tree for the first time.

	public override void _Ready () {

		if (Type == EventType.ERROR) {
			throw new Exception($" TriggerArea with name {Name} of type ERROR");
		}
		preassignedTarget = Target != null;
		eventManager = GetNode<EventManager>("/root/EventManager");
		if (GetChildren().Count(child => (child is CollisionPolygon2D) || (child is CollisionShape2D)) == 0 ) {
			GD.PushWarning($"TriggerArea ({Name}) has no collision area defined");
		}

		if (triggeredNodes == null || triggeredNodes.Length == 0) {
			triggeredNodes = GetChildren().OfType<Node2D>().ToArray();
			if (triggeredNodes.Length == 0) {
				GD.PushWarning($"TriggerArea ({Name}) has no triggered elements defined");
			}

			triggeredNodes = triggeredNodes.Where(node => node is IActionable).ToArray();
			if (triggeredNodes.Length == 0) {
				GD.Print($"TriggerArea ({Name}) has no triggered actionables defined.");
			}
		}

		cooldownComponent = GetNode<CooldownComponent>("CooldownComponent");
		cooldownComponent.SetCooldown(Cooldown);

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
			case TriggerBehaviourId.STAY_AND_PRESS_KEY:
				ProcessPress(key.Pressed);
				break;
			case TriggerBehaviourId.STAY_AND_HOLD_KEY:
				ProcessHold(key.Pressed);
				break;
			case TriggerBehaviourId.STAY_PRESS_AND_WAIT:
				ProcessPressWait(key.Pressed);
				break;
			case TriggerBehaviourId.STAY_HOLD_AND_WAIT:
				ProcessHoldWait(key.Pressed);
				break;
			case TriggerBehaviourId.STAY_AND_WAIT:
				break;

			case TriggerBehaviourId.STILL_STAY_AND_WAIT:
				break;
			case TriggerBehaviourId.STILL_STAY_AND_PRESS_KEY:
				break;
			case TriggerBehaviourId.STILL_STAY_PRESS_AND_WAIT:
				break;
			case TriggerBehaviourId.STILL_STAY_AND_HOLD_KEY:
				break;
			case TriggerBehaviourId.STILL_STAY_HOLD_AND_WAIT:
				break;
			case TriggerBehaviourId.INSTANTANEOUS:
			default:
				GD.PrintErr($"Should not be here. BehaviourType {Behaviour} in _Input()");
				break;
		}
	}

	private void OnObjectEntered (Node2D node) {

		Target ??= node;

		if (Target != node) {
			return;
		}

		if (!triggerOnEnter) {
			IsTriggered = false;
			return;
		}

		if (node is not RigidBody2D) {
			return;
		}

		NodesInside.Add(node);

		switch (Behaviour) {
			case TriggerBehaviourId.INSTANTANEOUS:
				ProcessInstantaneous();
				break;
			case TriggerBehaviourId.STAY_AND_WAIT:
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

		NodesInside.Remove(node);

		if (!preassignedTarget && Behaviour != TriggerBehaviourId.INSTANTANEOUS) {
			Target = null;
		}

		if (triggerOnEnter) {
			IsTriggered = false;
			return;
		}

		switch (Behaviour) {
			case TriggerBehaviourId.INSTANTANEOUS:
				ProcessInstantaneous();
				break;
			case TriggerBehaviourId.STAY_AND_WAIT:
				ProcessWait();
				break;
		}
	}

	#endregion Inputs
	#region Behaviour
	public void PrepareEvents() {

		BodyEntered += OnObjectEntered;
		BodyExited += OnObjectExited;

		behaviourInfo = new TriggerBehaviour(Behaviour);

		if (HoldingTime > 0) {
			holdTimer = new Timer {
				OneShot = true,
				WaitTime = HoldingTime
			};
			AddChild(holdTimer);
		}
		else {
			behaviourInfo.HoldButton = false;
		}

		if (WaitingTime > 0) {
			waitTimer = new Timer() {
				OneShot = true,
				WaitTime = WaitingTime
			};
			AddChild(waitTimer);
		}
		else {
			behaviourInfo.WaitInArea = false;
		}

		if (behaviourInfo.WaitInArea) {
			waitTimer.Timeout += () => AfterWaiting();
		}

		if (behaviourInfo.HoldButton) {

			holdTimer.Timeout += () => AfterHolding();
		}
	}
	public void AfterWaiting() {
		if (!behaviourInfo.WaitInArea) {
			return;
		}

		Trigger(new EventData());
	}
	public void AfterHolding() {
		if (!behaviourInfo.HoldButton) {
			return;
		}

		if (behaviourInfo.WaitInArea) {
			waitTimer.Start();
			return;
		}

		Trigger(new EventData());
	}

	public void AfterStopping () {
		
		if (!behaviourInfo.Stopped) {
			return;
		}

		if (behaviourInfo.HoldButton) {
			holdTimer.Start();
			return;
		}

		if (behaviourInfo.WaitInArea) {
			waitTimer.Start();
			return;
		}

		if (!behaviourInfo.Stopped) {
			return;
		}
		Trigger(new EventData());
	}
	public override void Trigger(EventData data ) {

		if (cooldownComponent.IsOnCooldown) {
			return;
		}

		bool isInstantaneous = data?.Parameters?.ContainsKey(INSTANTANEOUS) ?? false;
		if (isInstantaneous) {
			isInstantaneous = (bool)data?.Parameters?[INSTANTANEOUS];
		}

		if (!NodesInside.Contains(Target) && !isInstantaneous) {
			GD.PushWarning($" [TriggerArea.Trigger()]: Target {Target.Name} is not inside TriggerArea {Name} anymore.");
			return;
		}

		base.Trigger(data);

		if (triggerOnEnter) {
			NodesInside.Remove(Target);
		}

		if (!preassignedTarget && Behaviour == TriggerBehaviourId.INSTANTANEOUS) {
			Target = null;
		}

		if (data.Parameters == null) {
			data.Parameters = new Dictionary<StringName, object>();
		}

		if (!data.Parameters.TryGetValue(ACTIVATOR, out var _))
		{
			data.Parameters[ACTIVATOR] = Target;
		}

		data.Parameters[ENTERING] = triggerOnEnter;

		eventManager.SendMessage(this, triggeredNodes, Type, data.Parameters);

		IsTriggered = true;
	}

	public void ProcessInstantaneous() {
		Trigger(new EventData() { Sender = Target, Parameters = new Dictionary<StringName, object>() { { INSTANTANEOUS, true }, { ACTIVATOR, Target } } });
	}
	public void ProcessWait () {
		if (waitTimer.TimeLeft == 0) {
			waitTimer.Start();
		}
	}
	public void ProcessPress (bool isPressed) {
		if (isPressed) {
			Trigger(new EventData());
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
				

				Trigger(new EventData() { Sender = Target, Parameters = new Dictionary<StringName, object> { { HOLD_PERCENTAGE, holdPerc } } });

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
