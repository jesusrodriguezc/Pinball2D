using Godot;
using Pinball.Utils;
using System;

public partial class Slingshot : Node2D
{
	private Ball ball;
	private Area2D collisionArea;

	private Vector2 perfectDirection;
	private Vector2 horizontal;
	// Called when the node enters the scene tree for the first time.

	// Output signals


	[Signal]
	public delegate void CollisionEventHandler (Node node);
	[Signal]
	public delegate void ReleaseEventHandler (Node node);
	[Signal]
	public delegate void ImpulseEventHandler (Vector2 impulse);

	[Export]
	public float HitPower { get; set; }
	[Export]
	public double BaseScore { get; set; }
	public override void _Ready () {

		ball = GetNode<Ball>("/root/Main/Ball");
		collisionArea = GetNode<Area2D>("CollisionArea");

		collisionArea.BodyEntered += _OnCollision;

		perfectDirection = Transform.BasisXform(Vector2.Up);
		horizontal = Transform.BasisXform(Vector2.Left);
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

		/* 
		 * TO DO: Hay que buscar una funcion, similar a la normal, que, a partir del angulo entre la normal (Vector2.Up) y el vector de centros, permita generar
		 * un vector direccion. Por ejemplo: y = -16(x - 0.5)^4 + 1
		 */
		Vector2 dirImpulso = (ball.GlobalPosition - collisionArea.GlobalPosition).Normalized();
		var proximity = perfectDirection.Dot(dirImpulso); // [0, 1]
		var testingFunct = Mathx.SlingshotDistribution(proximity);

		Vector2 golpeRecto = perfectDirection * testingFunct;
		Vector2 golpeLateral = dirImpulso * (1 - testingFunct) * 0.25f;

		GD.Print($"La bola ha venido. Impulso: {dirImpulso} Dot: {proximity} Testing: {testingFunct}");

		// Ejercemos la fuerza sobre la bola.
		EmitSignal(SignalName.Impulse, (golpeRecto + golpeLateral) * HitPower);

		GD.Print($"Velocidad: {ball.LinearVelocity} - {ball.ConstantForce}");


	}

	private void _OnRelease (Node node) {
		// Replace with function body.

		if (node != ball) {
			return;
		}

		GD.Print("La bola se ha ido");

	}

}

