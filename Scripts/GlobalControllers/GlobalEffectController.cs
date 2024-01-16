using Godot;
using Pinball.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using static GroupTrigger;

/// <summary>
/// Manage all the global effects (e.j. bonus, ...)
/// </summary>
public partial class GlobalEffectController : Node {


	private List<GroupTrigger> groupTriggers = new();
	
	public override void _Ready () {
		groupTriggers = Nodes.findByClass<GroupTrigger>(GetTree().Root);
		groupTriggers.ForEach(groupTrigger => {
			GD.Print($"GroupTrigger {groupTrigger.Name}");
			groupTrigger.GlobalEffect += (type, element) => ManageGlobalEffect(groupTrigger, type, element);
		});
	}

	public void ManageGlobalEffect(GroupTrigger group, string type, Node2D affectedNode) {

		GD.Print($"[{group.GroupName} COMPLETED] Effect {type} applied.");
		switch (type) {
			case BONUS_ON:
				if (affectedNode is ReboundBase rbOn) {
					AddBonus(rbOn, group);
				}
				break;
			case BONUS_OFF:
				if (affectedNode is ReboundBase rbOff) {
					RemoveBonus(rbOff, group);
				}
				break;
			case SCORE_ADD:
				PinballController.Instance.scoreController.AddScore(group.ScoreAddition);
				break;
		}
	}

	public void AddBonus (ReboundBase element, GroupTrigger bonus) {
		if (element == null) return;

		element.scoreComponent?.ApplyBonus(bonus.GroupName, bonus.BonusMultiplier);
		element.spriteManagerComponent?.ChangeTexture(1);
	}

	public void RemoveBonus (ReboundBase element, GroupTrigger bonus) {
		if (element == null) return;

		element.scoreComponent?.ApplyBonus(bonus.GroupName, bonus.BonusMultiplier);
		element.spriteManagerComponent?.ChangeTexture(0);

	}
}

