using Godot;
using Pinball.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public partial class MissionController : Node {
	private SaveManager saveManager; 
	private RankController rankController;
	private ScoringController scoringController;

	[Export] public string MissionFolderPath = "res://Missions/";
	[Export] public MissionItem missionItemAccepter;
	private string MissionFolderAbsolutePath;

	private Dictionary<StringName, MissionItem> missionItemDict = new();
	private List<Mission> missionList = new ();

	private MissionMenu currentMissionMenu;
	private List<Mission> currentMissionList;
	[Export] public GroupTrigger missionTargetGroup;
	private List<MissionTarget> missionTargets;

	private Mission currentSelectedMission;
	public Mission ActiveMission { get; private set; }

	public override void _Ready () {
		saveManager = GetNode<SaveManager>("/root/SaveManager");
		rankController = GetNode<RankController>("/root/Pinball/RankController");
		scoringController = GetNode<ScoringController>("/root/ScoringController");

		var missionItems = Nodes.findByClass<MissionItem>(GetTree().Root);
		missionItemDict = missionItems.ToDictionary(target => target.ItemId, target => target);

		MissionFolderAbsolutePath = ProjectSettings.GlobalizePath(MissionFolderPath);
		if (!Directory.Exists(MissionFolderAbsolutePath)) {
			GD.PrintErr($"Mission directory {MissionFolderAbsolutePath} does not exist");
			return;
		}

		foreach (string file in Directory.EnumerateFiles(MissionFolderAbsolutePath, "mission_*.json")) {
			GD.Print($"Loading {file}");
			saveManager.Load<Mission>(file, out var newMission);
			newMission.Name = newMission.Title;
			GD.Print($"Adding mission {newMission.Name}");
			missionList.Add(newMission);
			newMission.MissionReward += () => OnMissionCompleted(newMission);
			newMission.MissionFailed += () => OnMissionFailed(newMission);
			AddChild(newMission);
		}

		missionTargets = missionTargetGroup.GetChildren().OfType<MissionTarget>().OrderBy(target => target.MissionId).ToList();
		foreach (var missionTarget in missionTargets) {
			missionTarget.Actioned += () => OnMissionSelectedChanged(missionTarget.MissionId);
		}
		missionItemAccepter.MissionTargetReached += AcceptMission;
	}

	public override void _Input (InputEvent @event) {
		if (Input.IsActionJustPressed("Debug")) {
			ActiveMission?.AddMissionTime(20f);
		}
		if (Input.IsActionJustPressed("Mission1")) {
			OnMissionSelectedChanged(0);
			AcceptMission();
		}
		if (Input.IsActionJustPressed("Mission2")) {
			OnMissionSelectedChanged(1);
			AcceptMission();
		}
		if (Input.IsActionJustPressed("Mission3")) {
			OnMissionSelectedChanged(2);
			AcceptMission();
		}
		if (Input.IsActionJustPressed("Mission4")) {
			OnMissionSelectedChanged(3);
			AcceptMission();
		}
	}
	private void OnMissionSelectedChanged (int missionId) {
		if (missionId < 0) {
			GD.PushWarning($"Selection not available with missionId being less than 0 {missionId}");
			return;
		}

		if (currentMissionList.Any(mission => mission.Status == MissionStatus.IN_PROGRESS && !mission.Hidden)) {
			GD.PushWarning("There is a mission already in progress");
			return;
		}

		if (missionTargets.All(target => target.Active)) {
			currentSelectedMission = currentMissionList.Last();
			GD.Print($"All targets down. Mission {currentSelectedMission.Title} selected");
			return;
		}

		currentSelectedMission = currentMissionList.Where(mission => mission.MissionId == missionId).FirstOrDefault();
		if (currentSelectedMission == null) {
			GD.PushWarning($"Cannot select mission with missionId {missionId}. Check if the mission target is not well defined.");
			return;
		}

		GD.Print($"Last target down: {missionId}. Mission {currentSelectedMission.Title} selected");
	}

	private void OnMissionFailed (Mission mission) {
		if (mission.Status != MissionStatus.FAILED) {
			return;
		}
		if (mission == ActiveMission) {
			ActiveMission = null;
		}

		mission.Status = MissionStatus.READY;
	}

	public void OnMissionCompleted (Mission mission) {
		GD.Print("OnMissionCompleted", mission.Status, ActiveMission.Title);
		if (mission.Status != MissionStatus.COMPLETED) {
			return;
		}
		if (mission == ActiveMission) {
			ActiveMission = null;
		}
		scoringController.AddScore(mission.ScoreReward);
		if (Math.Abs(mission.XpReward) > 10f) {
			rankController.AddExperiencePoints(mission.XpReward);

		} else {
			rankController.AddRank(mission.XpReward);
		}
	}

	public List<Mission> GetMissionList (RankId rank, bool excludeCompleted = true) {
		return missionList.Where(mission => mission.Ranks.Contains(rank) && (!excludeCompleted || mission.Status.IsNotIn(MissionStatus.COMPLETED, MissionStatus.FAILED))).ToList();
	}

	public MissionItem GetTarget (StringName name) {
		if (!missionItemDict.TryGetValue(name, out var target)) {
			return null;
		}

		return target;
	}

	internal void SetMissionMenu (MissionMenu missionMenu) {
		currentMissionMenu = missionMenu;
	}

	public void OnRankChanged (int newRank) {
		StopCurrentMission();

		foreach (var currentMission in missionList.Where(mission => !mission.Ranks.Contains((RankId)newRank) && mission.Status.IsNotIn(MissionStatus.COMPLETED, MissionStatus.FAILED))) {
			GD.Print(currentMission.Title, "now is OUT_OF_RANK");
			currentMission.Status = MissionStatus.OUT_OF_RANK;
		}

		currentMissionMenu.OnRankChanged(newRank);

		currentMissionList = missionList.Where(mission => mission.Ranks.Contains((RankId)newRank) && mission.Status == MissionStatus.OUT_OF_RANK).ToList();
		foreach (var mission in currentMissionList) {
			GD.Print(mission.Title, "now is READY");

			mission.Status = MissionStatus.READY;

			if (mission.MissionId == -1) {
				mission.StartMission();
			}
		}	
	}

	public void AcceptMission () {
		if (currentMissionList.Any(mission => mission.Status == MissionStatus.IN_PROGRESS && !mission.Hidden)) {
			GD.PushWarning("There is a mission already in progress");
			return;
		}

		if (currentSelectedMission == null) {
			GD.PushWarning("There is no mission selected.");

		}
		ActiveMission = currentSelectedMission;
		currentSelectedMission = null;
		GD.Print($"{ActiveMission} going to start");
		ActiveMission.StartMission();
		var acceptedMissionCard = currentMissionMenu.GetMissionCard(ActiveMission);
		if (acceptedMissionCard == null) {
			GD.PushWarning($"There is no card for the mission {ActiveMission.Title}");
		}

		acceptedMissionCard.OnMissionStarted();

		missionTargetGroup.Enable();
	}

	public void Reset () {
		foreach (var mission in missionList) {
			mission.Reset();
		}
	}

	public void StopCurrentMission () {
		ActiveMission?.OnMissionFailed();
		ActiveMission = null;
	}

}
