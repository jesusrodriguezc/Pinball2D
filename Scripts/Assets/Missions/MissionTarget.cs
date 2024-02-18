using Godot;
using System;
using System.Linq;

public partial class MissionTarget : Node {

	[Signal] public delegate void MissionTargetReachedEventHandler ();

	[Export] public StringName ItemId { get; set; }
	[Export] public Node2D[] Targets { get; set; }
	[Export] public bool ActionEachTarget { get; set; }
	[Export] public Node2D[] AvoidTargets { get; set; }

	public override void _Ready () {
		if (Targets == null || Targets.Length == 0) {
			throw new NullReferenceException($"Targets from {Name} not defined.");
		}

		
		if (string.IsNullOrWhiteSpace(ItemId)) {
			throw new NullReferenceException($"ItemId from {Name} not defined.");
		}

		foreach(var target in Targets ) {
			if (target is not TriggerBase && target is not IActionable) {
				throw new Exception($"Target {target.Name} from {Name} is not valid.");
			}

			if (target is TriggerBase trigger) {
				trigger.Triggered += () => TargetReached();
			}

			if (target is IActionable actionable ) {
				if (!target.HasSignal("Actioned")) {
					continue;
				}
				target.Connect("Actioned", Callable.From(TargetReached));
				//body.Actioned += () => TargetReached();
			}
		}

		foreach (var target in AvoidTargets) {
			if (target is not TriggerBase && target is not IActionable) {
				throw new Exception($"Avoid target {target.Name} from {Name} is not valid.");
			}
		}
	}

	private void TargetReached () {
		EmitSignal(SignalName.MissionTargetReached);
	}

	public override void _Process (double delta) {
		
	}
}
