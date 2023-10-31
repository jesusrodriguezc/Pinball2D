using Godot;
using System;

public partial class Bumper : StaticBody2D
{
	private AnimationPlayer _animationPlayer;
	private Ball ball;
	private Area2D collisionArea;
	// Called when the node enters the scene tree for the first time.

	// Output signals

	[Signal]
	public delegate void ImpulseEventHandler (Vector2 impulse);

	[Export]
	public float HitPower { get; set; }

	[Export]
	public double BaseScore { get; set; }
	public override void _Ready () {
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		ball = GetNode<Ball>("/root/Main/Ball");
		collisionArea = GetNode<Area2D>("CollisionArea");

		collisionArea.BodyEntered += _OnCollision;
		collisionArea.BodyExited += _OnRelease;

		collisionArea.CollisionLayer = CollisionLayer;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _OnCollision (Node node) {
		
		if (node != ball) {
			return;
		}


		// Calculamos el vector entre la bola y el bumper.
		Vector2 dirImpulso = (ball.GlobalPosition - GlobalPosition).Normalized();

		//GD.Print($"La bola ha venido. Impulso: {dirImpulso * HitPower}");

		_animationPlayer.Play("on_collision");

		// Ejercemos la fuerza sobre la bola.
		EmitSignal(SignalName.Impulse, dirImpulso * HitPower);

		//GD.Print($"Velocidad: {ball.LinearVelocity} - {ball.ConstantForce}");


	}

	private void _OnRelease (Node node) {
		// Replace with function body.

		if (node != ball) {
			return;
		}

		//GD.Print("La bola se ha ido");

		_animationPlayer.Play("on_release");
	}

}

