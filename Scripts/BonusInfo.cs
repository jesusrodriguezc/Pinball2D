using Godot;
using Pinball.Scripts.Utils;
using System.Linq;

public partial class BonusInfo : Node2D {

	[Signal]
	public delegate void BonusEventHandler (Node2D element, bool active);

	[Export]
	public StringName BName;
	[Export]
	public double BonusMultiplier { get; set; }
	[Export]
	public double Duration { get; set; }
	public bool Active { get; set; }
	private bool allActive = false;

	#region Elements
	[Export]
	private Node[] elements;
	private BonusLane[] lanes;
	public Timer timer { get; set; }

	private AudioComponent _audioComponent;

	public readonly StringName POWERUP = "Powerup";
	#endregion

	public override void _Ready () {

		lanes = GetChildren().OfType<BonusLane>().ToArray();

		timer = GetNodeOrNull<Timer>("Timer");
		timer.Timeout += Off;

		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(POWERUP, ResourceLoader.Load<AudioStream>("res://SFX/powerup.wav"));
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
		allActive = lanes.All(l => l.Active);


		if (allActive && timer.TimeLeft == 0) {

			On();
		}
	}

	public void On (double? duration = null) {
		foreach (var lane in lanes) {
			lane.Blocked = true;
		}
		foreach (var element in elements) {
			EmitSignal(SignalName.Bonus, element, true);

		}
		_audioComponent.Play(POWERUP, AudioComponent.SFX_BUS);
		timer.Start(duration ?? Duration);
	}

	public void Off () {
		foreach (var element in elements) {
			EmitSignal(SignalName.Bonus, element, false);
		}
		foreach (var lane in lanes) {
			lane.Reset();
		}
		Active = false;
		timer.Stop();
	}
}
