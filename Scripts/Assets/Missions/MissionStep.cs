using Godot;
using System;
using System.Collections.Generic;

public enum MissionStepStatus {
	READY,
	IN_PROGRESS,
	COMPLETED
}
public partial class MissionStep : Node {

	[Signal] public delegate void MissionStepCompletedEventHandler ();
	[Signal] public delegate void MissionStepUpdatedEventHandler ();
	[Signal] public delegate void MissionStepFailedEventHandler ();


	private MissionController missionController;

	public string Title { get; set; }
	public int Repetitions { get; set; }
	public int RepetitionsLeft;
	public int RepetitionsDone => Repetitions - RepetitionsLeft;

	//public List<MissionStep> ForbiddenSteps { get; set; }

	public MissionStepStatus Status = MissionStepStatus.READY;

	public override void _Ready () {
		missionController = GetNode<MissionController>("/root/Pinball/MissionController");
	}

	public void OnForbiddenStepReached (MissionStep forbiddenStep) {
		if (Status != MissionStepStatus.IN_PROGRESS) {
			return;
		}

		Status = MissionStepStatus.COMPLETED;
		EmitSignal(SignalName.MissionStepFailed);
	}
	public void StartStep () {
		if (Status != MissionStepStatus.READY) {
			return;
		}
		Status = MissionStepStatus.IN_PROGRESS;
		RepetitionsLeft = Repetitions;
	}

	public void UpdateStep (MissionItem target) {
		if (Status != MissionStepStatus.IN_PROGRESS) {
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
		if (Status != MissionStepStatus.IN_PROGRESS) {
			return;
		}

		GD.Print($"Step {Name} completed. Moving to next step");
		Status = MissionStepStatus.COMPLETED;
		EmitSignal(SignalName.MissionStepCompleted);
	}

	public void Reset () {
		Status = MissionStepStatus.READY;
		RepetitionsLeft = Repetitions;
	}
}