using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class RankController : Node{
	[Signal] public delegate void RankChangedEventHandler (int nextRank);

	public RankId CurrentRank;
	public double ExperiencePoints;
	public double MaxXpCurrentRank;

	public void AddExperiencePoints(double xp) {
		GD.Print("AddExperiencePoints -> ", ExperiencePoints, "+", xp, ">=", MaxXpCurrentRank);

		ExperiencePoints += xp;
		if (ExperiencePoints >= MaxXpCurrentRank) {
			ChangeRank(CurrentRank + 1);
			ExperiencePoints %= MaxXpCurrentRank;
		}

	}

	public void ChangeRank (RankId nextRank) {
		if (!Enum.IsDefined(nextRank)) {
			GD.PrintErr($"Rank {nextRank} is not defined.");
			return;
		}

		CurrentRank = nextRank;
		MaxXpCurrentRank = Rank.maxXpPerRank[CurrentRank];

		
		GD.Print($"Emitimos signal RankChanged({(int)CurrentRank})");
		EmitSignal(SignalName.RankChanged, (int)CurrentRank);
	}

	public void Reset () {
		ExperiencePoints = 0;
		ChangeRank(RankId.LEVEL1 );
	}

}

