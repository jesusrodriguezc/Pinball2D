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
	public Ball Ball { get; set; }

	public ScoringController ScoreCtrl;

	public override void _Ready () {
		_instance = this;

		Ball = GetChildren().OfType<Ball>().ToList().First();

		ScoreCtrl = new ScoringController(GetTree().Root);

		Ball.Death += RemoveLive;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
		if (LivesLeft <= 0) {
			GD.Print("Hemos perdido");
			Ball.currentStatus = Ball.Status.GAMEOVER;
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
		var global = GetNode<SceneSwitcher>("/root/SceneSwitcher");
		global.GotoScene("res://Escenas/GameOverMenu.tscn");
	}


}

