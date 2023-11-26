using Godot;
using Pinball.Utils;

public partial class Slingshot : StaticBody2D {
	private AnimationPlayer _animationPlayer;
	private ScoreComponent _scoreComponent;

	private Area2D collisionArea;

	private Vector2 perfectDirection;
	private Vector2 horizontal;
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
		if (!PinballController.Instance.Balls.Contains(currBall)) {
			GD.PrintErr("La pelota no esta en la lista de pelotas.");
			return;
		}

		// Calculamos el vector entre la bola y el bumper.

		Vector2 dirImpulso = CalculateImpulseDirection(currBall);
		_animationPlayer?.Play("on_collision");

		EmitSignal(SignalName.Impulse, currBall, dirImpulso * HitPower);
		_scoreComponent?.AddScore();

	}

	private Vector2 CalculateImpulseDirection (Node2D hittedElement) {
		Vector2 hitVector = (hittedElement.GlobalPosition - collisionArea.GlobalPosition).Normalized();
		var vectorProximity = perfectDirection.Dot(hitVector); // [0, 1]

		Vector2 golpeRecto, golpeLateral;
		if (EvenlyDistributedPower) {
			golpeRecto = perfectDirection * 2f + 0f * hitVector * 0.25f;
			golpeLateral = Vector2.Zero;
		} 
		else {
			var forceDistributionPerc = Mathx.SlingshotDistribution(vectorProximity);
			golpeRecto = perfectDirection * forceDistributionPerc;
			golpeLateral = hitVector * (1 - forceDistributionPerc) * 0.25f;
		}

		return golpeRecto + golpeLateral;
	}

	private void _OnRelease (Node node) {
		// Replace with function body.

		if (node.GetType() != typeof(Ball)) {
			return;
		}

		Ball currBall = (Ball)node;
		if (!PinballController.Instance.Balls.Contains(currBall)) {
			GD.Print("La pelota no esta en la lista de pelotas.");
			return;
		}
		_animationPlayer?.Play("on_release");

	}

}

