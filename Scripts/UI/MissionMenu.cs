using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class MissionMenu : Control {
	private List<MissionCard> missionCards = new List<MissionCard>();
	private MissionController missionController;
	[Export] public PackedScene missionScene;
	private Timer rankChangedTimer;
	public override void _Ready () {
		base._Ready();

		missionController = GetNode<MissionController>("/root/Pinball/MissionController");
		missionController.SetMissionMenu(this);
		rankChangedTimer = new Timer();
		AddChild(rankChangedTimer);
	}

	public async void OnRankChanged(int rank) {

		//rankChangedTimer.Start(2f);
		//await ToSignal(rankChangedTimer, Timer.SignalName.Timeout);

		foreach (var card in missionCards) {
			if (card != null) {
				RemoveChild(card);
				card.QueueFree();
			}
		}

		missionCards = new List<MissionCard>();
		if (!Enum.IsDefined(typeof(RankId), rank)) {
			return;
		}

		foreach (Mission mission in missionController.GetMissionList((RankId)rank)) {

			var missionCard = missionScene.Instantiate<MissionCard>();
			missionCards.Add(missionCard);
			AddChild(missionCard);
			missionCard.SetMission(mission);
		}
	}
}

