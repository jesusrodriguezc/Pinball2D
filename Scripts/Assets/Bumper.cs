using Godot;

public partial class Bumper : StaticBody2D {
	private AnimationPlayer _animationPlayer;
	private SpriteManagerComponent _spriteManager;
	private ScoreComponent _scoreComponent;
	private Area2D _collisionArea;

	[Signal]
	public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);

	[Export]
	public float HitPower { get; set; }

	public override void _Ready () {
		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		_spriteManager = GetNodeOrNull<SpriteManagerComponent>("SpriteManagerComponent");
		_scoreComponent = GetNodeOrNull<ScoreComponent>("ScoreComponent");
		_collisionArea = GetNodeOrNull<Area2D>("CollisionArea");

		_collisionArea.BodyEntered += _OnCollision;
		_collisionArea.BodyExited += _OnRelease;

		_collisionArea.CollisionLayer = CollisionLayer;

		_spriteManager.ChangeTexture(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
	}

	private void _OnCollision (Node node) {

		if (node.GetType() != typeof(Ball)) {
			return;
		}

		Ball currBall = (Ball)node;
		if (!PinballController.Instance.Balls.Contains(currBall)) {
			GD.PrintErr("La pelota no esta en la lista de pelotas.");
			return;
		}

		Vector2 dirImpulso = (currBall.GlobalPosition - GlobalPosition).Normalized();

		_animationPlayer.Play("on_collision");

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
		if (!PinballController.Instance.Balls.Contains(currBall)) {
			GD.Print("La pelota no esta en la lista de pelotas.");
		}

		_animationPlayer.Play("on_release");
	}



}

