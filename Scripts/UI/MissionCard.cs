using Godot;
using Pinball.Scripts.Utils;
using System;
using System.Linq;
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

	private Timer freeTimer;
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
		animationPlayer.AnimationFinished += (animationName) => { GD.Print(animationName, ".AnimationFinished");  if (animationName.ToString().IsIn("completed", "failed")) OnMissionClosed(); };

		freeTimer = new Timer();
		AddChild(freeTimer);
	}

	public void SetMission(Mission mission) {
		currentMission = mission;
		mission.MissionUpdated += UpdateCardSteps;
		mission.MissionFailed += OnMissionFailed;
		mission.MissionCompleted += OnMissionCompleted;

		ClearAllLabels();
		title.Text = mission.Name;

		UpdateCardSteps();

		SetRankSpriteFromAtlas(mission.Ranks.Min());
		if (mission.XpReward > 10) {
			xpReward.Text = string.Format("+{0:00}xp", mission.XpReward);
		}
		else {
			xpReward.Text = string.Format("+{0}lvl", mission.XpReward);
		}
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
		completedLabel.Text = Tr("COMPLETED");
		animationPlayer.Play("completed");
	}

	private void OnMissionFailed () {
		completedLabel.Text = Tr("FAILED");
		animationPlayer.Play("failed");
	}

	private void OnMissionClosed () {
		currentMission.EmitSignal(Mission.SignalName.MissionReward);
		Hide();
	}

	public void OnMissionStarted () {
		Show();
		animationPlayer.Play("started");
		UpdateCardSteps();
	}
	private void UpdateCardSteps () {
		if (currentMission == null) {
			return;
		}
		if (currentMission.Items.Count > 0 && currentMission.Items[0] is MissionStep step1) {
			firstStep.Text = Tr(step1.Title);
			firstStepCounter.Text = string.Format("({0}/{1})", currentMission.Items[0].RepetitionsDone, step1.Repetitions);
		}

		if (currentMission.Items.Count > 1 && currentMission.Items[1] is MissionStep step2) {
			secondStep.Text = Tr(step2.Title);
			secondStepCounter.Text = string.Format("({0}/{1})", step2.RepetitionsDone, step2.Repetitions);
		}	

		if (currentMission.Items.Count > 2 && currentMission.Items[2] is MissionStep step3) {
			thirdStep.Text = Tr(step3.Title);
			thirdStepCounter.Text = string.Format("({0}/{1})", step3.RepetitionsDone, step3.Repetitions);
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
		int rankInt = (int)rank - 1;
		int x = (rankInt % 4) * 16;
		int y = (rankInt / 4) * 16;

		rankAtlas.Region = new Rect2(x, y, rankAtlas.Region.Size.X, rankAtlas.Region.Size.Y);
	}

	internal async void RemoveSafely () {
		if (animationPlayer.IsPlaying()) {
			await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		}
		Hide();
		freeTimer.Start(0.5f);
		await ToSignal(freeTimer, Timer.SignalName.Timeout);
		QueueFree();

	}
}

