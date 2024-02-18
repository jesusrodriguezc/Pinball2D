using Godot;

public partial class BonusLane : Area2D, IActionable, IGroupable {

	[Signal] public delegate void ActionedEventHandler ();
	private AnimationPlayer _animationPlayer;
	private AudioComponent _audioComponent;

	public readonly StringName SWITCH = "Switch";

	public bool Active { get { return _active; } set { if (_active == value) return; _active = value; if (value) Enable(); else { Disable(); } } } 
	private bool _active = false;
	private CooldownComponent cooldownComponent;

	public bool Blocked { get; set; } = false;
	[Export] public double Cooldown { get; set; }
	public bool IsCollisionEnabled { get; set; }

	private Timer DisableTimer;

	// Called when the node enters the scene tree for the first time.

	public override void _Ready () {
		base._Ready();

		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		_animationPlayer.GetAnimation("Completed").LoopMode = Animation.LoopModeEnum.Linear;

		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(SWITCH, ResourceLoader.Load<AudioStream>("res://SFX/bonuslane_switch.wav"));
		}
		BodyEntered += (node) => Action(new EventData() { Sender = node });

		cooldownComponent = GetNodeOrNull<CooldownComponent>("CooldownComponent");
		cooldownComponent.SetCooldown(Cooldown);

		DisableTimer = new Timer() {
			Autostart = false,
			OneShot = true
		};

		AddChild(DisableTimer);
		DisableTimer.Timeout += () => {
			_animationPlayer.Stop();
			Active = false;
			Blocked = false;
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
	}

	public void Enable () {
		_animationPlayer?.Play("Enabled");
	}

	public void Disable () {
		_animationPlayer?.Play("Disabled");
	}
	public void Reset () {
		Blocked = false;
		Active = false;
	}

	public void Action (EventData data) {

		if (!IsCollisionEnabled) {
			return;
		}

		if (cooldownComponent.IsOnCooldown) {
			return;
		}

		if (data.Sender is not IActor) {
			return;
		}

		if (Blocked) {
			return;
		}

		Active = !Active;

		_audioComponent?.Play(SWITCH, AudioComponent.SFX_BUS);
	}

	public void OnCompleted (double duration) {

		_animationPlayer.Play("Completed");
		DisableTimer.Start(duration);
	}

	public void EnableCollision (bool enable) {
		IsCollisionEnabled = enable;
	}
}
