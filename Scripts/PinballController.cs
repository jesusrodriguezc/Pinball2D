using Godot;
using System.Collections.Generic;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

public partial class PinballController : Node {

	public enum LevelZ {
		FONDO = -10,
		BASE = 0,
		RAMPA = 1,
		SHADER = 50,
		UI = 100
	}

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

	public ScoringController ScoreCtrl;
	private PackedScene gameOverMenu;

	public override void _Ready () {
		_instance = this;

		Balls = GetChildren().OfType<Ball>().ToList();

		ScoreCtrl = new ScoringController(GetTree().Root);

		Balls.ForEach(ball => { ball.Death += RemoveLive; });

		gameOverMenu = ResourceLoader.Load<PackedScene>("res://Escenas/GameOverMenu.tscn");




	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
		if (LivesLeft <= 0) {
			GD.Print("Hemos perdido");
			Balls.ForEach(ball => { ball.currentStatus = Ball.Status.GAMEOVER; });
			Gameover();


		}
	}

	public void RemoveLive (Ball ball) {
		if (LivesLeft > 0) {
			LivesLeft--;
		}
	}

	public double GetScore () {
		return ScoreCtrl?.Score ?? 0;
	}

	public void Gameover () {
		if (gameOverMenu == null) {
			return;
		}

		if (gameOverMenu == null) {
			return;
		}
		GetTree().ChangeSceneToPacked(gameOverMenu);
	}


}

