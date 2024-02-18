using Godot;
using Pinball.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

public partial class PinballController : Node2D {

	#region Singleton
	// Singleton
	//private static PinballController _instance = new PinballController();
	private Random randomNumberGenerator;

	private PinballController () { }
	//public static PinballController Instance {
	//	get { return _instance; }
	//}
	#endregion Singleton

	[Export] public bool Debug { get; set; }

	[Export] public int LivesLeft { get; set; }
	[Export] public Vector2 ballInitialPosition { get; set; }
	public Ball Ball { get; set; }
	public Camera CurrentCamera { get; set; }

	#region Subcontrollers
	public ScoringController scoreController;
	public GlobalEffectController globalEffectController;
	private MissionController missionController;
	private RankController rankController;
	private SceneSwitcher sceneSwitcher;

	#endregion
	private PauseMenu pauseMenu;
	private GameOverMenu gameoverMenu;
	bool isPaused = false;

	private CanvasLayer userInterface;
	private GameUI gameUI;
	private MissionMenu missionMenu;

	public override void _Ready () {
		randomNumberGenerator = new Random(Guid.NewGuid().GetHashCode());
		LayerManager.UpdateActionables(this, 0, true);

		sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");

		CurrentCamera = GetChildren().OfType<Camera>().FirstOrDefault();
		scoreController = GetNode<ScoringController>("/root/ScoringController");
		scoreController.Prepare();
		globalEffectController = GetNode<GlobalEffectController>("/root/GlobalEffectController");
		globalEffectController.Prepare();

		userInterface = GetNode<CanvasLayer>("UserInterface");
		gameUI = GetNode<GameUI>("UserInterface/GameUI");
		missionMenu = GetNode<MissionMenu>("UserInterface/MissionMenu");
		pauseMenu = GetNodeOrNull<PauseMenu>("UserInterface/PauseMenu");
		gameoverMenu = GetNodeOrNull<GameOverMenu>("UserInterface/GameOverMenu");
		missionController = GetNodeOrNull<MissionController>("/root/Pinball/MissionController");

		rankController = GetNodeOrNull<RankController>("/root/Pinball/RankController");
		if (rankController != null) {
			rankController.RankChanged += OnRankChanged;
		}


		StartGame();
	}
	public void OnRankChanged (int nextRank) {
		missionController.OnRankChanged(nextRank);
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
		SetBall();

		if (LivesLeft <= 0) {
			Ball.currentStatus = Ball.Status.GAMEOVER;
			gameUI.Hide();
			missionMenu.Hide();
			gameoverMenu.Show();
			missionController.Reset();
		}
	}

	public double GetScore () {
		return scoreController?.Score ?? 0;
	}

	internal void PauseGame () {
		gameUI.Hide();
		missionMenu.Hide(); 
		pauseMenu.Show();
		GetTree().Paused = true;
		Ball.Pause();
		Engine.TimeScale = 0f;
	}

	internal void ResumeGame () {
		Engine.TimeScale = 1f;
		Ball.Unpause();
		pauseMenu.Hide();
		gameUI.Show();
		missionMenu.Show();
		GetTree().Paused = false;

	}

	public void Shake () {
		CurrentCamera.ApplyShake();
		Ball.ForceMove(new Vector2((float)randomNumberGenerator.NextDouble(), (float)randomNumberGenerator.NextDouble()), randomNumberGenerator.Next(100, 1000));
	}

	public void DisableAllFor<T> (double secondsDisabled) where T : Node2D, IActionable {
		var nodes = Nodes.findByClass<T>(GetTree().Root).ToList();
		foreach (var node in nodes) {
			var cooldownController = node.GetChildByType<CooldownComponent>();
			if (cooldownController == null) {
				continue;
			}

			cooldownController.ApplyCooldown(secondsDisabled);
		}
	}

	public void SetBall () {
		if (Ball != null) {
			RemoveChild(Ball);
			Ball.QueueFree();
			Ball = null;
		}

		Ball = GD.Load<PackedScene>("res://Assets/Ball/ball.tscn").Instantiate<Ball>();
		Ball.GlobalPosition = ballInitialPosition;
		CallDeferred("add_child", Ball);
		Ball.Death += OnLiveLost;
	}
	public async void StartGame () {
		GD.Print("Empezamos partida");

		await ToSignal(this, SignalName.Ready);

		SetBall();

		gameoverMenu?.Hide();
		scoreController?.ResetScore();
		rankController?.Reset();
		gameUI?.Show();
	}

	public void HideUI () {
		userInterface.Hide();
	}
}

