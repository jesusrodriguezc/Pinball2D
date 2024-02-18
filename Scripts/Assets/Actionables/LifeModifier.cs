using Godot;
using System;

public partial class LifeModifier : Node2D, IActionable {
	[Signal] public delegate void ActionedEventHandler ();
	[Export] public int LivesModification;
	[Export] AudioStream Audio;
	private PinballController pinballController;
	private AudioComponent audioComponent;
	private static readonly string LIFE = "life";

	public bool IsCollisionEnabled { get; set; }

	public override void _Ready () {
		pinballController = GetNode<PinballController>("/root/Pinball");
		audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (Audio != null) audioComponent?.AddAudio(LIFE, Audio);
	}
	public void Action (EventData data) {

		if (LivesModification == 0) { 
			return; 
		}

		if (data.Parameters[TriggerBase.ACTIVATOR] is not Ball ball) {
			return;
		}

		pinballController.LivesLeft += LivesModification;
		if (LivesModification > 0) {
			audioComponent?.Play(LIFE, AudioComponent.SFX_BUS);
		} else {
			ball.Die();
		}

	}

	public void EnableCollision (bool enable) {
		return;
	}
}
