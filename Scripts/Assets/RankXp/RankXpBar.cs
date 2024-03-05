using Godot;

public partial class RankXpBar : Node2D {

	public TextureProgressBar xpBar;
	public TextureProgressBar rankBar;
	public Label missionTimeLeft;

	private RankController rankController;
	private MissionController missionController;

	public override void _Ready () {
		xpBar = GetNodeOrNull<TextureProgressBar>("CenterContainer/XpBar");
		rankBar = GetNodeOrNull<TextureProgressBar>("CenterContainer/RankBar");
		missionTimeLeft = GetNodeOrNull<Label>("CenterContainer/MissionTimeLeft");

		rankController = GetNodeOrNull<RankController>("/root/Pinball/RankController");
		if (rankController != null) {
			rankController.RankChanged += SetRank;
			rankController.XpChanged += SetExperience;
		}

		missionController = GetNodeOrNull<MissionController>("/root/Pinball/MissionController");
		if (missionController?.ActiveMission != null) {
			missionTimeLeft.Text = string.Format("{0}:{1:00}", 
				(int)(missionController.ActiveMission.DurationTimer.TimeLeft / 60), 
				missionController.ActiveMission.DurationTimer.TimeLeft % 60);
		}

	}

	public override void _Process (double delta) {
		if (missionController?.ActiveMission != null) {
			missionTimeLeft.Text = string.Format("{0}:{1:00}",
				(int)(missionController.ActiveMission.DurationTimer.TimeLeft / 60),
				missionController.ActiveMission.DurationTimer.TimeLeft % 60);
		}
		else {
			missionTimeLeft.Text = "";
		}
	}

	public void SetExperience (double xpPercent) {
		xpBar.Value = xpPercent * xpBar.MaxValue;
	}

	public void SetRank(int rank) {
		rankBar.Value = rank;
	}


}

