using Godot;
using System;

public partial class MissionStep : Node {

	[Signal] public delegate void MissionStepCompletedEventHandler ();
	[Signal] public delegate void MissionStepUpdatedEventHandler ();

	public string Type { get; set; }
	public string Description;
	public int Repetitions { get; set; }
	public int RepetitionsLeft;
	public int RepetitionsDone => Repetitions - RepetitionsLeft;
	public bool HasStarted = false;
	public bool IsCompleted = false;

	public override void _Ready () {
		Description = Tr(Type);
	}
	public void StartStep () {
		if (HasStarted || IsCompleted) {
			return;
		}
		HasStarted = true;
		RepetitionsLeft = Repetitions;
	}

	public void UpdateStep (MissionTarget target) {
		if (!HasStarted) {
			return;
		}

		if (IsCompleted) {
			return;
		}

		RepetitionsLeft--;

		if (RepetitionsLeft <= 0) {
			FinishStep();
		}
		else {
			EmitSignal(SignalName.MissionStepUpdated);
		}
	}

	public void FinishStep () {
		if (!HasStarted || IsCompleted) {
			return;
		}

		GD.Print($"Step {Name} completed. Moving to next step");
		IsCompleted = true;
		EmitSignal(SignalName.MissionStepCompleted);
	}

	public void Reset () {
		HasStarted = false;
		IsCompleted = false;
		RepetitionsLeft = Repetitions;
	}
}