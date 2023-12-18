using Godot;
using Pinball.Utils;

[Tool]
public partial class Slingshot : StaticBody2D {
	private AnimationPlayer _animationPlayer;
	private ScoreComponent _scoreComponent;
	private AudioComponent _audioComponent;

	public readonly StringName HIT = "Hit";
	public readonly StringName CLICK = "Click";

	private Area2D collisionArea;

	private Vector2 perfectDirection;
	private Vector2 horizontal;
	private Vector2 lastDirImpulso;

	// Called when the node enters the scene tree for the first time.

	[Signal]
	public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);

	[Export]
	public float HitPower { get; set; }
	/// <summary>
	/// Determines if the hit power is distributed evenly over its surface or it hit harder at the center and softer at the ends.
	/// </summary>
	[Export]
	public bool EvenlyDistributedPower { get; set; }
	public override void _Ready () {
		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(CLICK, ResourceLoader.Load<AudioStream>("res://SFX/slingshot_click.wav"));
			_audioComponent.AddAudio(HIT, ResourceLoader.Load<AudioStream>("res://SFX/slingshot_hit_2.wav"));

		}

		collisionArea = GetNodeOrNull<Area2D>("CollisionArea");

		collisionArea.BodyEntered += _OnCollision;
		collisionArea.BodyExited += _OnRelease;

		perfectDirection = Transform.BasisXform(Vector2.Up);
		horizontal = Transform.BasisXform(Vector2.Left);
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

		// Calculamos el vector entre la bola y el bumper.

		lastDirImpulso = CalculateImpulseDirection(currBall);
		QueueRedraw();
		_animationPlayer?.Play("on_collision");
		_audioComponent?.Play(CLICK, AudioComponent.SFX_BUS);
		_audioComponent?.Play(HIT, 0.1f);
		EmitSignal(SignalName.Impulse, currBall, lastDirImpulso * HitPower);
		_scoreComponent?.AddScore();

	}

	private Vector2 CalculateImpulseDirection (Node2D hittedElement) {
		Vector2 hitVector = (hittedElement.GlobalPosition - collisionArea.GlobalPosition).Normalized();
		var vectorProximity = perfectDirection.Dot(hitVector); // [0, 1]

		Vector2 golpeRecto, golpeLateral;
		if (EvenlyDistributedPower) {
			golpeRecto = perfectDirection * 4f;
			golpeLateral = hitVector * 0.5f;
		} 
		else {
			var forceDistributionPerc = Mathx.SlingshotDistribution(vectorProximity);
			golpeRecto = perfectDirection * forceDistributionPerc;
			golpeLateral = hitVector * (1 - forceDistributionPerc) * 0.5f;
		}

		return (golpeRecto + golpeLateral) / 2f;
	}

	private void _OnRelease (Node node) {
		// Replace with function body.

		if (node.GetType() != typeof(Ball)) {
			return;
		}

		Ball currBall = (Ball)node;
		if (PinballController.Instance.Ball != currBall) {
			GD.Print("La pelota no esta en la lista de pelotas.");
			return;
		}
		_animationPlayer?.Play("on_release");

	}

	public override void _Draw () {
		GD.Print($"Dibujando pepinatso: {lastDirImpulso}");
		DrawLine(Vector2.Zero, lastDirImpulso.Rotated(-this.Rotation) * HitPower, Colors.Red, 4f);
	}

}


