using Godot;
using System;

public partial class LifeModifier : Node2D, IActionable
{
	public bool IsCollisionEnabled { get; set; } = true;
	[Export] public int LivesModification;
	[Export] AudioStream Audio;
	private AudioComponent audioComponent;
	private static readonly string LIFE = "life";

	public override void _Ready () {
		audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (Audio != null) audioComponent?.AddAudio(LIFE, Audio);
	}
	public void Action (EventData data) {

		if (LivesModification == 0) { 
			return; 
		}

		if (data.Parameters[ITrigger.ACTIVATOR] is not Ball ball) {
			return;
		}

		PinballController.Instance.LivesLeft += LivesModification;
		if (LivesModification > 0) {
			audioComponent?.Play(LIFE, AudioComponent.SFX_BUS);
		} else {
			ball.Die();
		}

	}

	public void EnableCollision (bool enable) {
		IsCollisionEnabled = enable;
	}
}
