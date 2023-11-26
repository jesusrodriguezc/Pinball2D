using Godot;

public partial class BonusLane : Area2D {
	private AnimationPlayer _animationPlayer;

	public bool Active { get; private set; }
	public bool Blocked { get; set; } = false;

	// Called when the node enters the scene tree for the first time.

	public override void _Ready () {

		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

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
		_animationPlayer.Play("Enabled");
	}

	public void Disable () {
		Active = false;
		_animationPlayer.Play("Disabled");
	}
	public void Reset () {
		Blocked = false;
		Disable();
	}
}
