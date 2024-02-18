using Godot;
using Pinball.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public partial class MissionController : Node {
	private SaveManager saveManager; 
	private RankController rankController;
	private ScoringController scoringController;

	[Export] public string MissionFolderPath = "res://Missions/";
	private string MissionFolderAbsolutePath;

	private Dictionary<StringName, MissionTarget> missionTargetDict = new();
	private List<Mission> missionList = new ();

	private MissionMenu currentMissionMenu;

	public override void _Ready () {
		saveManager = GetNode<SaveManager>("/root/SaveManager");
		rankController = GetNode<RankController>("/root/Pinball/RankController");
		scoringController = GetNode<ScoringController>("/root/ScoringController");


		var missionTargets = Nodes.findByClass<MissionTarget>(GetTree().Root);
		missionTargetDict = missionTargets.ToDictionary(target => target.ItemId, target => target);

		MissionFolderAbsolutePath = ProjectSettings.GlobalizePath(MissionFolderPath);
		if (!Directory.Exists(MissionFolderAbsolutePath)) {
			GD.PrintErr($"Mission directory {MissionFolderAbsolutePath} does not exist");
			return;
		}

		foreach (string file in Directory.EnumerateFiles(MissionFolderAbsolutePath, "mission_*.json")) {
			GD.Print($"Loading {file}");
			saveManager.Load<Mission>(file, out var newMission);
			newMission.Name = newMission.Title + "_" + newMission.Rank;
			GD.Print($"Adding mission {newMission.Name}");
			missionList.Add(newMission);
			newMission.MissionReward += () => OnMissionCompleted(newMission);
			AddChild(newMission);
		}
	}

	public void OnMissionCompleted (Mission mission) {

		if (!mission.isCompleted) {
			return;
		}
		scoringController.AddScore(mission.ScoreReward);
		rankController.AddExperiencePoints(mission.XpReward);
	}

	public List<Mission> GetMissionList (RankId rank, bool excludeCompleted = true) {
		return missionList.Where(mission => mission.Rank == rank && (!excludeCompleted || !mission.isCompleted)).ToList();
	}

	public MissionTarget GetTarget (StringName name) {
		if (!missionTargetDict.TryGetValue(name, out var target)) {
			return null;
		}

		return target;
	}

	internal void SetMissionMenu (MissionMenu missionMenu) {
		currentMissionMenu = missionMenu;
	}

	public void OnRankChanged (int rank) {
		currentMissionMenu.OnRankChanged(rank);
		foreach(var mission in missionList.Where(mis => mis.Rank == (RankId)rank && !mis.isCompleted && !mis.hasStarted).ToList()) {
			mission.StartMission();
		}
	}

	public void Reset () {
		foreach (var mission in missionList) {
			mission.Reset();
		}
	}

}
