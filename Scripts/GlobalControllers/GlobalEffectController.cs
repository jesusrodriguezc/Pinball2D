using Godot;
using Pinball.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using static GroupTrigger;

/// <summary>
/// Manage all the global effects (e.j. bonus, ...)
/// </summary>
public partial class GlobalEffectController : Node {
	private PinballController pinballController;
	private List<GroupTrigger> groupTriggers = new();
	
	public override void _Ready () {
		//Prepare();
	}
	public void Prepare () {
		pinballController = GetNode<PinballController>("/root/Pinball");
		groupTriggers = Nodes.findByClass<GroupTrigger>(GetTree().Root);
		groupTriggers.ForEach(groupTrigger => {
			groupTrigger.GlobalEffect += (type, element) => ManageGlobalEffect(groupTrigger, type, element);
		});
	}

	public void ManageGlobalEffect(GroupTrigger group, string type, Node2D affectedNode) {

		switch (type) {
			case CHANGE_UPGRADE_LEVEL:
				if (affectedNode is ReboundBase rbOn) {
					ChangeUpgradeLevel(rbOn, group);
				}
				break;
			case SCORE_ADD:
				pinballController.scoreController.AddScore(group.ScoreAddition);
				break;
			case ACTION_DONE:
				// TODO: Completa una parte de una mision
				break;
		}
	}

	public void ChangeUpgradeLevel (ReboundBase element, GroupTrigger bonus) {
		if (element == null) return;

		element.scoreComponent?.ChangeUpgradeBonus(bonus.GroupName, bonus.CurrentUpgradeLevel);
		element.upgradeComponent?.ChangeLevel(bonus.CurrentUpgradeLevel);
	}
}

