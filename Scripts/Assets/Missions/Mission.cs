using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public enum MissionStatus {
	READY,
	IN_PROGRESS,
	COMPLETED,
	FAILED,
	OUT_OF_RANK
}
public partial class Mission : Node{

	[Signal] public delegate void MissionCompletedEventHandler ();
	[Signal] public delegate void MissionRewardEventHandler ();
	[Signal] public delegate void MissionUpdatedEventHandler ();
	[Signal] public delegate void MissionFailedEventHandler ();

	private MissionController missionController;
	private RankController rankController;
	public int MissionId;
	public string Title { get; set; }
	public List<MissionStep> Items { get; set; }
	public bool Hidden { get; set; }
	private int CurrentStepId;
	public List<RankId> Ranks { get; set; }
	public int ScoreReward { get; set; }
	public int XpReward { get; set; }
	public int? Duration { get; set; }
	public MissionStatus Status = MissionStatus.OUT_OF_RANK;

	public Timer DurationTimer;

	public override void _Ready () {
		missionController = GetNode<MissionController>("/root/Pinball/MissionController");
		rankController = GetNode<RankController>("/root/Pinball/RankController");

		var titleSplit = Title.Split('_');
		if (titleSplit.Length != 3 || !int.TryParse(Title.Split('_')[2], out MissionId)) {
			MissionId = -1;
		}
		foreach (var item in Items) {
			if (item is MissionStep step) {
				step.RepetitionsLeft = step.Repetitions;
				step.MissionStepCompleted += () => UpdateMissionProgress(step);

				var target = missionController.GetTarget(step.Title);
				if (target == null) {
					GD.PrintErr($"Target {step.Title} does not exist");
					return;
				}
				target.MissionTargetReached += () => step.UpdateStep(target);
				step.MissionStepUpdated += () => OnMissionUpdate();
				step.MissionStepFailed += OnMissionFailed;
				step.Name = step.Title + "_" + step.Repetitions;
			}

			AddChild(item);
		}

		DurationTimer = new Timer() { OneShot = true, Autostart = false, WaitTime = Duration ?? 30f };
		AddChild(DurationTimer);
		DurationTimer.Timeout += Reset;
	}

	public void OnMissionFailed () {
		if (Status != MissionStatus.IN_PROGRESS) {
			return;
		}
		Status = MissionStatus.FAILED;
		EmitSignal(SignalName.MissionFailed);
		Reset();
	}

	private void OnMissionUpdate () {
		if (Status != MissionStatus.IN_PROGRESS) {
			return;
		}
		EmitSignal(SignalName.MissionUpdated);
	}

	public void StartMission () {

		if (Status != MissionStatus.READY) 
		{
			return;
		}

		if (!Ranks.Contains(rankController.CurrentRank)) {
			return;
		}

		GD.Print($"Mission \"{Title}\" started. Ranks: {Ranks}");

		Status = MissionStatus.IN_PROGRESS;

		Items.First().StartStep();
		DurationTimer.Start(Duration ?? 30f);
	}

	private void FinishMission () {
		var stepsUncompleted = Items.Where(step => step.Status != MissionStepStatus.COMPLETED);
		if (stepsUncompleted.Count() > 0) {
			GD.PrintErr($"There is steps to be completed at mission {Name}:");

			foreach(var step in stepsUncompleted) {
				GD.PrintErr($"- Step {step.Name} not completed ({step.RepetitionsLeft}/{step.Repetitions})");
				step.Reset();
			}
			stepsUncompleted.First().StartStep();
			return;
		}
		Status = MissionStatus.COMPLETED;

		EmitSignal(SignalName.MissionCompleted);

		DurationTimer.Stop();
		Reset();
	}

	private void UpdateMissionProgress(MissionStep step) {
		var index = Items.IndexOf(step);
		if (index == -1) {
			return;
		}

		if (step.Status != MissionStepStatus.COMPLETED) {
			return;
		}

		if (index >= Items.Count) {
			FinishMission();
			return;
		}

		var nextStep = Items.FirstOrDefault(step => step.Status != MissionStepStatus.COMPLETED);
		if (nextStep == null) {
			FinishMission();
			return;
		}
		EmitSignal(SignalName.MissionUpdated);
		nextStep.StartStep();
	}

	public void Reset () {
		DurationTimer.Stop();
		foreach(var step in Items) {
			step.Reset();
		}
		Status = MissionStatus.READY;
	}

	public void AddMissionTime (double extraTime) {
		GD.Print($"Tiempo antes {DurationTimer.WaitTime} / {DurationTimer.TimeLeft}");
		var timeLeft = DurationTimer.TimeLeft + extraTime;
		DurationTimer.Stop();

		if (timeLeft < 0) {
			timeLeft = 0.1f;
		}
		DurationTimer.Start(timeLeft);
		GD.Print($"Tiempo despues {DurationTimer.WaitTime} / {DurationTimer.TimeLeft}");
	}
}

