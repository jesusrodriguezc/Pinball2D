using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public partial class Mission : Node{

	[Signal] public delegate void MissionCompletedEventHandler ();
	[Signal] public delegate void MissionRewardEventHandler ();
	[Signal] public delegate void MissionUpdatedEventHandler ();

	//private PinballController pinballController;
	private MissionController missionController;
	private RankController rankController;
	public string Title { get; set; }
	public string Description { get; set; }
	public List<MissionStep> Steps { get; set; }
	private int CurrentStepId;
	public RankId Rank { get; set; }
	public int XpReward { get; set; }
	public int ScoreReward { get; set; }

	public bool hasStarted = false;
	public bool isCompleted = false;

	public override void _Ready () {
		//pinballController = GetNode<PinballController>("/root/Pinball");
		missionController = GetNode<MissionController>("/root/Pinball/MissionController");
		rankController = GetNode<RankController>("/root/Pinball/RankController");

		foreach (var step in Steps) {
			step.RepetitionsLeft = step.Repetitions;
			step.MissionStepCompleted += () => UpdateMissionProgress(step);

			var target = missionController.GetTarget(step.Type);
			if (target == null) {
				GD.PrintErr($"Target {step.Type} does not exist");
				return;
			}
			target.MissionTargetReached += () => step.UpdateStep(target);
			step.MissionStepUpdated += () => EmitSignal(SignalName.MissionUpdated);

			step.Name = step.Type + "_" + step.Repetitions;
			AddChild(step);
		}
	}

	public void StartMission () {

		GD.Print($"Mission \"{Description}\" started. Was started? {hasStarted} Rank: {Rank} - Current Rank {rankController.CurrentRank} ");

		if (hasStarted) {
			return;
		}

		if (Rank != rankController.CurrentRank) {
			return;
		}
		hasStarted = true;

		Steps.First().StartStep();
	}

	private void FinishMission () {
		var stepsUncompleted = Steps.Where(step => !step.IsCompleted);
		if (stepsUncompleted.Count() > 0) {
			GD.PrintErr($"There is steps to be completed at mission {Name}:");

			foreach(var step in stepsUncompleted) {
				GD.PrintErr($"- Step {step.Name} not completed ({step.RepetitionsLeft}/{step.Repetitions})");
				step.HasStarted = false;
			}
			stepsUncompleted.First().StartStep();
		}
		isCompleted = true;
		EmitSignal(SignalName.MissionCompleted);
	}

	private void UpdateMissionProgress(MissionStep step) {
		var index = Steps.IndexOf(step);
		if (index == -1) {
			return;
		}

		if (!step.HasStarted) {
			return;
		}

		if (index >= Steps.Count) {
			FinishMission();
			return;
		}

		var nextStep = Steps.FirstOrDefault(step => !step.IsCompleted);
		if (nextStep == null) {
			FinishMission();
			return;
		}
		EmitSignal(SignalName.MissionUpdated);
		nextStep.StartStep();


	}

	public void Reset () {
		hasStarted = false;
		isCompleted = false;
		foreach(var step in Steps) {
			step.Reset();
		}
	}
}

