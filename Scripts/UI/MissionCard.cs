using Godot;
using System;
using System.Reflection;

/// <summary>
/// Class that manages the representation of a mission at the screen.
/// </summary>
public partial class MissionCard : Control {

	public Mission currentMission;

	// UI
	private Label title;

	private Label firstStep;
	private Label secondStep;
	private Label thirdStep;

	private Label firstStepCounter;
	private Label secondStepCounter;
	private Label thirdStepCounter;

	private TextureRect rankSprite;
	private AtlasTexture rankAtlas;
	private Label xpReward;

	private TextureRect scoreSprite;
	private Label scoreReward;

	private Label completedLabel;
	private AnimationPlayer animationPlayer;

	public override void _Ready () {
		base._Ready();

		title = GetNode<Label>("Margins/Card/MissionInfo/Title");

		firstStep = GetNode<Label>("Margins/Card/MissionInfo/Steps/DescriptionList/MissionStepDescription1");
		secondStep = GetNode<Label>("Margins/Card/MissionInfo/Steps/DescriptionList/MissionStepDescription2");
		thirdStep = GetNode<Label>("Margins/Card/MissionInfo/Steps/DescriptionList/MissionStepDescription3");

		firstStepCounter = GetNode<Label>("Margins/Card/MissionInfo/Steps/CounterList/MissionStepCounter1");
		secondStepCounter = GetNode<Label>("Margins/Card/MissionInfo/Steps/CounterList/MissionStepCounter2");
		thirdStepCounter = GetNode<Label>("Margins/Card/MissionInfo/Steps/CounterList/MissionStepCounter3");

		rankSprite = GetNode<TextureRect>("Margins/Card/Rewards/RewardXP/MarginContainer/RankIcon");
		rankAtlas = ((AtlasTexture)rankSprite.Texture);
		scoreSprite = GetNode<TextureRect>("Margins/Card/Rewards/RewardScore/MarginContainer/ScoreIcon");

		xpReward = GetNode<Label>("Margins/Card/Rewards/RewardXP/RewardXPAmount");
		scoreReward = GetNode<Label>("Margins/Card/Rewards/RewardScore/RewardScoreAmount");

		completedLabel = GetNode<Label>("CompletedLabel");
		completedLabel.Text = Tr("COMPLETED");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.AnimationFinished += (_) => OnMissionClosed();

	}

	public void SetMission(Mission mission) {
		currentMission = mission;
		mission.MissionUpdated += UpdateCardSteps;
		mission.MissionCompleted += OnMissionCompleted;

		ClearAllLabels();
		title.Text = mission.Name;

		UpdateCardSteps();

		SetRankSpriteFromAtlas(mission.Rank);
		xpReward.Text = string.Format("+{0:00}xp", mission.XpReward);
		if (mission.ScoreReward > 999999) {
			var scoreRewardInThousands = mission.ScoreReward / 1000000f;
			if (Math.Abs(scoreRewardInThousands % 1) <= (float.Epsilon * 100)) {
				scoreReward.Text = string.Format("+{0:0}kk", mission.ScoreReward / 1000000);
			} else {
				scoreReward.Text = string.Format("+{0:0.0}kk", mission.ScoreReward / 1000000f);
			}
		}
		else if (mission.ScoreReward > 999) {
			var scoreRewardInThousands = mission.ScoreReward / 1000f;
			if (Math.Abs(scoreRewardInThousands % 1) <= (float.Epsilon * 100)) {
				scoreReward.Text = string.Format("+{0:0}k", mission.ScoreReward / 1000);
			} else {
				scoreReward.Text = string.Format("+{0:0.0}k", mission.ScoreReward / 1000f);
			}
		} else {
			scoreReward.Text = string.Format("+{0}", mission.ScoreReward);
		}
	}

	private void OnMissionCompleted () {
		animationPlayer.Play("completed");
	}

	private void OnMissionClosed () {
		currentMission.EmitSignal(Mission.SignalName.MissionReward);
		Hide();
	}

	private void UpdateCardSteps () {
		if (currentMission.Steps.Count > 0) {
			firstStep.Text = Tr(currentMission.Steps[0].Type);
			firstStepCounter.Text = string.Format("({0}/{1})", currentMission.Steps[0].RepetitionsDone, currentMission.Steps[0].Repetitions);
		}

		if (currentMission.Steps.Count > 1) {
			secondStep.Text = Tr(currentMission.Steps[1].Type);
			secondStepCounter.Text = string.Format("({0}/{1})", currentMission.Steps[1].RepetitionsDone, currentMission.Steps[1].Repetitions);
		}	

		if (currentMission.Steps.Count > 2) {
			thirdStep.Text = Tr(currentMission.Steps[2].Type);
			thirdStepCounter.Text = string.Format("({0}/{1})", currentMission.Steps[2].RepetitionsDone, currentMission.Steps[2].Repetitions);
		}
	}
	private void ClearAllLabels () {
		title.Text = "";

		firstStep.Text = "";
		secondStep.Text = "";
		thirdStep.Text = "";

		firstStepCounter.Text = "";
		secondStepCounter.Text = "";
		thirdStepCounter.Text = "";

		xpReward.Text = "";
		scoreReward.Text = "";
	}

	private void SetRankSpriteFromAtlas (RankId rank) {
		int x = ((int)rank % 4) * 16;
		int y = ((int)rank / 4) * 16;

		rankAtlas.Region = new Rect2(x, y, rankAtlas.Region.Size.X, rankAtlas.Region.Size.Y);
	}
}

