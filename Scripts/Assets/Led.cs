using Godot;
using System.Diagnostics;
using System;
using System.Linq;

public partial class Led : Node2D{

	private AnimationPlayer _animationPlayer;
	private AudioComponent _audioComponent;
	public readonly StringName SWITCH = "Switch";
	[Export] public Node2D[] Elements { get; set; }
	private TriggerBase[] Triggers;
	[Export] public bool Active { get; private set; }

	public override void _Ready () {

		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(SWITCH, ResourceLoader.Load<AudioStream>("res://SFX/bonuslane_switch.wav"));
		}

		Triggers = Elements?.OfType<TriggerBase>().ToArray();
	}

	public override void _Process (double delta) {
		bool allElementsTriggered = Triggers.All(trigger => trigger.IsTriggered);

		if (allElementsTriggered == Active) {
			return;
		}

		if (allElementsTriggered) {
			Enable();
		} else {
			Disable();
		}

	}

	public void Enable () {
		Active = true;
		_animationPlayer?.Play("Enabled");
		_audioComponent?.Play(SWITCH, AudioComponent.SFX_BUS);
	}

	public void Disable () {
		Active = false;
		_animationPlayer?.Play("Disabled");
		_audioComponent?.Play(SWITCH, AudioComponent.SFX_BUS);

	}
}

