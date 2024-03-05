using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

	public void OnRankChanged(int rank) {

		//rankChangedTimer.Start(2f);
		//await ToSignal(rankChangedTimer, Timer.SignalName.Timeout);

		//foreach (var card in missionCards) {
		//	if (card != null && !card.IsQueuedForDeletion()) {
		//		GD.Print($"Intentamos eliminar la tarjeta {card.Name}");
		//		card.RemoveSafely();
		//	}
		//}

		missionCards = new List<MissionCard>();
		if (!Enum.IsDefined(typeof(RankId), rank)) {
			return;
		}

		foreach (Mission mission in missionController.GetMissionList((RankId)rank)) {
			if (mission.Hidden) {
				continue;
			}

			var missionCard = missionScene.Instantiate<MissionCard>();
			missionCards.Add(missionCard);
			AddChild(missionCard);
			missionCard.SetMission(mission);
			missionCard.Hide();
		}
	}

	public MissionCard GetMissionCard(Mission mission) {
		return missionCards.Where(card => card.currentMission == mission).FirstOrDefault();
	}
}

