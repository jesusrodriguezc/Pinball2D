using Godot;
using Pinball.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

public partial class PinballController : Node2D {

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
	private Random randomNumberGenerator;

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
	public Camera CurrentCamera { get; set; }

	public ScoringController scoreController;
	public GlobalEffectController globalEffectController;
	private PauseMenu pauseMenu;
	bool isPaused = false;
	private SceneSwitcher sceneSwitcher;

	public override void _Ready () {
		_instance = this;
		randomNumberGenerator = new Random(Guid.NewGuid().GetHashCode());
		LayerManager.UpdateActionables(this, 0, true);

		sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");

		Ball = GetChildren().OfType<Ball>().ToList().First();
		CurrentCamera = GetChildren().OfType<Camera>().FirstOrDefault();
		scoreController = GetNode<ScoringController>("ScoringController");
		globalEffectController = GetNode<GlobalEffectController>("GlobalEffectController");

		pauseMenu = GetNodeOrNull<PauseMenu>("PauseMenu");

		Ball.Death += OnLiveLost;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
		if (LivesLeft <= 0 && Ball.currentStatus.IsNotIn(Ball.Status.DEAD, Ball.Status.GAMEOVER)) {
			GD.Print("Este mensaje es imposible, hacker.");
			sceneSwitcher.GotoScene("res://Escenas/MainMenu.tscn");
		}
	}

	public override void _Input (InputEvent @event) {
		if (Input.IsActionJustPressed("Pause"))
		{
			isPaused = !isPaused;
			if (isPaused) {
				PauseGame();
			}
			else {
				ResumeGame();
			}

		}
	}

	public void OnLiveLost(Ball ball) {

		if (LivesLeft <= 0) {
			GD.Print("Hemos perdido");
			Ball.currentStatus = Ball.Status.GAMEOVER;
			sceneSwitcher.GotoScene("res://Escenas/GameOverMenu.tscn");
		}
	}

	public double GetScore () {
		return scoreController?.Score ?? 0;
	}

	internal void PauseGame () {
		pauseMenu.Show();
		GetTree().Paused = true;
		Ball.Pause();
		//Engine.TimeScale = 0f;
	}

	internal void ResumeGame () {
		//Engine.TimeScale = 1f;
		Ball.Resume();
		pauseMenu.Hide();
		GetTree().Paused = false;

	}

	public void Shake () {
		// Should shake the camera and the ball.
		CurrentCamera.ApplyShake();
		Ball.ForceMove(new Vector2((float)randomNumberGenerator.NextDouble(), (float)randomNumberGenerator.NextDouble()), randomNumberGenerator.Next(100, 1000));
	}

	public async void DisableAll<T> (double secondsDisabled) where T : Node2D, IActionable {
		var nodes = Nodes.findByClass<T>(GetTree().Root).Where(node => node.IsCollisionEnabled).ToList();
		foreach (var node in nodes) {
			node.EnableCollision(false);
		}

		Timer disableTimer = new() {
			Autostart = false,
			OneShot = true,
			WaitTime = secondsDisabled
		};
		AddChild(disableTimer);

		disableTimer.Start(secondsDisabled);
		await ToSignal(disableTimer, "timeout");
		foreach (var node in nodes) {
			node.EnableCollision(true);
		}

		RemoveChild(disableTimer);
		disableTimer.QueueFree();


	}
}

