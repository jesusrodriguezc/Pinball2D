using Godot;
using Pinball.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

public partial class ScoringController : Node {

	private Node _owner;
	public double Score { get; private set; } = 0;
	private List<ScoreComponent> scoreComponents = new();
	private List<BonusInfo> bonusInfo = new();
	public ScoringController (Node owner) {
		_owner = owner;

		// Necesito encontrar los scoreComponents (dentro de los componentes) y BonusInfo (dentro de los BonusLane).
		// Podría buscar todos los componentes y todas las bonusLane, obtener lo que necesito y aplicarles el comando cuando llegue la señal.

		scoreComponents = Nodes.findByClass<ScoreComponent>(_owner);
		scoreComponents.ForEach(scoreComponent => { scoreComponent.Score += (points) => AddScore(points); });

		bonusInfo = Nodes.findByClass<BonusInfo>(_owner);
		bonusInfo.ForEach(bonusInfo => {
			bonusInfo.Bonus += (element, active) => {
				if (active) AddBonus(element, bonusInfo); else RemoveBonus(element, bonusInfo);
			};
		});

	}

	public void ResetScore () {
		Score = 0;
	}

	public void AddScore (double points) {
		Score += points;
	}

	public void AddBonus (Node2D element, BonusInfo bonus) {
		if (element == null) return;

		var spriteManagerComp = Nodes.findByClass<SpriteManagerComponent>(element).FirstOrDefault();
		var scoreComponent = Nodes.findByClass<ScoreComponent>(element).FirstOrDefault();

		spriteManagerComp?.ChangeTexture(1);
		scoreComponent?.ApplyBonus(bonus.BName, bonus.BonusMultiplier);

	}

	public void RemoveBonus (Node2D element, BonusInfo bonus) {
		if (element == null) return;

		var spriteManagerComp = Nodes.findByClass<SpriteManagerComponent>(element).FirstOrDefault();
		var scoreComponent = Nodes.findByClass<ScoreComponent>(element).FirstOrDefault();

		spriteManagerComp?.ChangeTexture(0);
		scoreComponent?.ApplyBonus(bonus.BName, bonus.BonusMultiplier);
	}

}

