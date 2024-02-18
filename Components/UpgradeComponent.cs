using Godot;
using System;

public partial class UpgradeComponent : Node
{
	private Sprite2D Sprite;
	public UPGRADE_LEVEL CurrentLevel = UPGRADE_LEVEL.NONE;
	private AnimationPlayer levelAnimationPlayer;
	public const string Level0 = "Level0";
	public const string Level0_1 = "Level0_to_1";
	public const string Level1_2 = "Level1_to_2";
	public const string Level2_3 = "Level2_to_3";

	public override void _Ready() {
		Owner = GetParent<Node2D>();
		levelAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void ChangeLevel (UPGRADE_LEVEL level) {
		if (level == CurrentLevel) {
			return;
		}

		bool upgrade = level > CurrentLevel;
		string animationName = null;
		switch (level) {
			case UPGRADE_LEVEL.BASIC:
				animationName = upgrade? Level0: Level0_1;
				break;
			case UPGRADE_LEVEL.FIRST:
				animationName = upgrade ? Level0_1 : Level1_2;
				break;
			case UPGRADE_LEVEL.SECOND:
				animationName = upgrade ? Level1_2 : Level2_3;
				break;
			case UPGRADE_LEVEL.THIRD:
				animationName = Level2_3;
				break;
			default:
				GD.PrintErr($"[ChangeLevel] Level {level} not defined");
				return;
		}

		if (upgrade) {
			levelAnimationPlayer.Play(animationName);
		} else {
			levelAnimationPlayer.PlayBackwards(animationName);
		}

		CurrentLevel = level;

	}


}
