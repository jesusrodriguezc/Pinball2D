using Godot;

public partial class BonusLane : Area2D {
	private AnimationPlayer _animationPlayer;
	private AudioComponent _audioComponent;

	public readonly StringName SWITCH = "Switch";

	public bool Active { get; private set; }
	public bool Blocked { get; set; } = false;

	// Called when the node enters the scene tree for the first time.

	public override void _Ready () {

		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(SWITCH, ResourceLoader.Load<AudioStream>("res://SFX/bonuslane_switch.wav"));
		}
		BodyEntered += ChangeStatus;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
	}

	public void ChangeStatus (Node2D node) {
		if (!node.GetType().Equals(typeof(Ball))) {
			return;
		}

		if (Blocked) {
			return;
		}

		if (Active) {
			Disable();
		} else {
			Enable();
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
	public void Reset () {
		Blocked = false;
		Disable();
	}
}
