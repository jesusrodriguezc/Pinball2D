using Godot;
using Pinball.Utils;
using System.Collections.Generic;

[Tool]
public partial class Slingshot : StaticBody2D, IActionable {
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

	[Export] public float HitPower { get; set; }
	[Export] public float Score { get; set; }

	/// <summary>
	/// Determines if the hit power is distributed evenly over its surface or it hit harder at the center and softer at the ends.
	/// </summary>
	[Export]
	public bool EvenlyDistributedPower { get; set; }
	public bool IsCollisionEnabled { get ; set; }

	public override void _Ready () {
		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(CLICK, ResourceLoader.Load<AudioStream>("res://SFX/slingshot_click.wav"));
			_audioComponent.AddAudio(HIT, ResourceLoader.Load<AudioStream>("res://SFX/slingshot_hit_2.wav"));

		}

		_scoreComponent = GetNodeOrNull<ScoreComponent>("ScoreComponent");

		if (_scoreComponent != null) {
			_scoreComponent.BaseScore = Score;
		}

		collisionArea = GetNodeOrNull<Area2D>("CollisionArea");

		collisionArea.BodyEntered += (node) => Action(new EventData() { Sender = node, Parameters = new Dictionary<StringName, object>() { { ITrigger.ENTERING, true }, {ITrigger.ACTIVATOR, node } } }); ;
		collisionArea.BodyExited += (node) => Action(new EventData() { Sender = node, Parameters = new Dictionary<StringName, object>() { { ITrigger.ENTERING, false }, {ITrigger.ACTIVATOR, node } } }); ;

		perfectDirection = Transform.BasisXform(Vector2.Up);
		horizontal = Transform.BasisXform(Vector2.Left);
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

	public void Action (EventData data) {
		if (!IsCollisionEnabled) {
			return;
		}

		bool isEntering = false;
		if (data.Parameters.TryGetValue(ITrigger.ENTERING, out var entering)) {
			isEntering = (bool)entering;
		}

		if (isEntering) {
			Collision(data.Sender);
		} else {
			Release(data.Sender);
		}
	}

	private void Collision (Node2D node) {

		if (node is not IActor) {
			return;
		}

		// Calculamos el vector entre la bola y el bumper.

		lastDirImpulso = CalculateImpulseDirection(node);
		_animationPlayer?.Play("on_collision");
		_audioComponent?.Play(CLICK, AudioComponent.SFX_BUS);
		_audioComponent?.Play(HIT, 0.1f);
		EmitSignal(SignalName.Impulse, node, lastDirImpulso * HitPower);
		_scoreComponent?.AddScore();

	}

	private void Release (Node2D node) {
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
	public void EnableCollision (bool enable) {
		IsCollisionEnabled = enable;
	}
}


