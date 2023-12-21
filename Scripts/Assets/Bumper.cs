using Godot;

public partial class Bumper : StaticBody2D {
	private Area2D _collisionArea;
	private AnimationPlayer _animationPlayer;

	#region Components
	private SpriteManagerComponent _spriteManager;
	private ScoreComponent _scoreComponent;
	private AudioComponent _audioComponent;
	#endregion

	public readonly StringName HIT = "Hit";


	[Signal]
	public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);

	[Export] public float HitPower { get; set; }
	[Export] public float Score { get; set; }

	public override void _Ready () {
		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

		_collisionArea = GetNode<Area2D>("CollisionArea");
		_collisionArea.BodyEntered += _OnCollision;
		_collisionArea.BodyExited += _OnRelease;
		_collisionArea.CollisionLayer = CollisionLayer;

		_spriteManager = GetNodeOrNull<SpriteManagerComponent>("SpriteManagerComponent");
		_spriteManager.ChangeTexture(0);

		_scoreComponent = GetNodeOrNull<ScoreComponent>("ScoreComponent");

		if (_scoreComponent != null) {
			_scoreComponent.BaseScore = Score;
		}
		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(HIT, ResourceLoader.Load<AudioStream>("res://SFX/bumper_hit.wav"));
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
	}

	private void _OnCollision (Node node) {

		if (node.GetType() != typeof(Ball)) {
			return;
		}

		Ball currBall = (Ball)node;
		if (PinballController.Instance.Ball != currBall) {
			GD.PrintErr("La pelota no esta en la lista de pelotas.");
			return;
		}

		Vector2 dirImpulso = (currBall.GlobalPosition - GlobalPosition).Normalized();

		_animationPlayer?.Play("on_collision");
		_audioComponent?.Play(HIT, AudioComponent.SFX_BUS);
		EmitSignal(SignalName.Impulse, currBall, dirImpulso * HitPower);
		AddScore();
	}

	public void AddScore() {
		_scoreComponent?.AddScore();
	}

	private void _OnRelease (Node node) {
		// Replace with function body.

		if (node.GetType() != typeof(Ball)) {
			return;
		}

		Ball currBall = (Ball)node;
		if (PinballController.Instance.Ball != currBall) {
			GD.Print("La pelota no esta en la lista de pelotas.");
		}

		_animationPlayer?.Play("on_release");
	}



}

