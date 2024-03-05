using Godot;
using System.Collections.Generic;

public partial class Flag: Area2D, IActionable {
	[Signal] public delegate void ActionedEventHandler ();

	private AnimationPlayer animationPlayer;
	private Timer animationTimer;
	private float currentSpinningTime;
	private bool? isLastMovementClockWise;
	private ScoreComponent scoreComponent;
	[Export] public float Score { get; set; }
	public bool IsCollisionEnabled { get; set; }

	public override void _Ready () {
		base._Ready ();
		BodyEntered += (node) => Action(new EventData() { Sender = node, Parameters = new Dictionary<StringName, object>() { { TriggerBase.ENTERING, true }, { TriggerBase.ACTIVATOR, node } } });
		BodyExited += (node) => Action(new EventData() { Sender = node, Parameters = new Dictionary<StringName, object>() { { TriggerBase.ENTERING, false }, { TriggerBase.ACTIVATOR, node } } });

		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.AnimationFinished += (animation) => { if (animation == "anticlockwise") OnTurnCompleted(); };

		animationTimer = new Timer() { OneShot = true, Autostart = false };
		AddChild(animationTimer);
		animationTimer.Timeout += Stop;

		scoreComponent = GetNodeOrNull<ScoreComponent>("ScoreComponent");

		if (scoreComponent != null) {
			scoreComponent.BaseScore = Score;
		}
	}

	private void Stop () {
		animationPlayer.Pause();
		currentSpinningTime = 0f;
		isLastMovementClockWise = null;
	}

	private void Release (Node2D body) {
		animationTimer.Start(currentSpinningTime);
		//animationPlayer.GetAnimation("anticlockwise").LoopMode = Animation.LoopModeEnum.Linear;
		animationPlayer.Play("anticlockwise");

		// TO_DO: Llamar actioned al acabar la animacion

	}
	
	private void OnTurnCompleted () {
		animationPlayer.Play("anticlockwise");
		EmitSignal(SignalName.Actioned);
		scoreComponent?.AddScore();
	}

	private void Collision (Node2D node) {
		if (node == null) {
			return;
		}

		if (node is not RigidBody2D body) {
			return;
		}

		if (animationTimer.TimeLeft > 0) {
			currentSpinningTime = (float)animationTimer.TimeLeft;
			animationTimer.Stop();
		}

		var direction = GlobalPosition.DirectionTo(body.GlobalPosition);
		var isMovementClockwise = direction.Dot(Transform.BasisXform(Vector2.Up)) > 0;

		//animationPlayer.GetAnimation("anticlockwise").LoopMode = Animation.LoopModeEnum.None;

		if (isLastMovementClockWise.HasValue && isLastMovementClockWise.Value != isMovementClockwise) {
			currentSpinningTime = -currentSpinningTime;
		}

		GD.Print(body.LinearVelocity.Length());

		if (isMovementClockwise) {
			animationPlayer.PlayBackwards("anticlockwise");
			currentSpinningTime += (body.LinearVelocity.Length() / 200f) * animationPlayer.GetAnimation("anticlockwise").Length;

		} else {
			animationPlayer.Play("anticlockwise");
			currentSpinningTime += (body.LinearVelocity.Length() / 200f) * animationPlayer.GetAnimation("anticlockwise").Length;
		}

		isLastMovementClockWise = isMovementClockwise;

	}

	public void Action (EventData data) {
		if (!IsCollisionEnabled) {
			return;
		}

		if (data.Sender is not IActor) {
			return;
		}
		bool isEntering = false;
		if (data.Parameters.TryGetValue(TriggerBase.ENTERING, out var entering)) {
			isEntering = (bool)entering;
		}
		if (isEntering) {
			Collision(data.Sender);
		} else {
			Release(data.Sender);
		}
	}

	public void EnableCollision (bool enable) {
		IsCollisionEnabled = enable;
	}
}

