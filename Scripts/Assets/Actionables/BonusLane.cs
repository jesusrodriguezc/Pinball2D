using Godot;

public partial class BonusLane : Area2D, IActionable, IGroupable {


	private AnimationPlayer _animationPlayer;
	private AudioComponent _audioComponent;

	public readonly StringName SWITCH = "Switch";

	public bool Active { get { return _active; } set { if (_active == value) return; _active = value; if (value) Enable(); else { Disable(); } } } 
	private bool _active = false;
	public bool Blocked { get; set; } = false;
	public bool IsCollisionEnabled { get; set; }

	// Called when the node enters the scene tree for the first time.

	public override void _Ready () {

		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(SWITCH, ResourceLoader.Load<AudioStream>("res://SFX/bonuslane_switch.wav"));
		}
		BodyEntered += (node) => Action(new EventData() { Sender = node});

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
	}

	public void Enable () {
		_animationPlayer?.Play("Enabled");
		_audioComponent?.Play(SWITCH, AudioComponent.SFX_BUS);
	}

	public void Disable () {
		_animationPlayer?.Play("Disabled");
		_audioComponent?.Play(SWITCH, AudioComponent.SFX_BUS);

	}
	public void Reset () {
		Blocked = false;
		Active = false;
	}

	public void Action (EventData data) {
		GD.Print(data.Sender.Name, Blocked, Active, IsCollisionEnabled);

		if (!IsCollisionEnabled) {
			return;
		}

		if (data.Sender is not IActor) {
			return;
		}

		if (Blocked) {
			return;
		}

		Active = !Active;

		//if (Active) {
		//	Disable();
		//} else {
		//	Enable();
		//}

		GD.Print(data.Sender.Name, Blocked, Active, IsCollisionEnabled);

	}

	public void EnableCollision (bool enable) {
		IsCollisionEnabled = enable;
	}
}
