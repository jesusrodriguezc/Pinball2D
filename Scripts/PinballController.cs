using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class PinballController : Node {

	#region Singleton
	// Singleton
	private static PinballController _instance = new PinballController();
	private PinballController () { }
	public static PinballController Instance {
		get { return _instance; }
	}
	#endregion Singleton

	[Export]
	public bool Debug { get; set; }

	[Export]
	public int LivesLeft { get; set; }
	public List<Ball> Balls { get; set; }
	public double Score {get; private set; }

	public override void _Ready () {
		_instance = this;

		Balls = GetChildren().OfType<Ball>().ToList();

		GD.Print($"Balls: {_instance.Balls.Count}");

		Balls.ForEach(ball => { ball.Score += (points) => AddScore(points); });
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {

	}

	public void ResetScore () {
		Score = 0;
	}

	public void AddScore(double points) {
		GD.Print($"Añadimos puntos (+{points})");
		Score += points;
	}
}

