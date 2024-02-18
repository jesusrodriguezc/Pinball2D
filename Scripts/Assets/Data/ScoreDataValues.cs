using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum UPGRADE_LEVEL {
	NONE = -1,
	BASIC,
	FIRST,
	SECOND,
	THIRD
}

public static class ScoreDataValues {
	public static Dictionary<UPGRADE_LEVEL, double> upgradeLevelMultiplierDict = new Dictionary<UPGRADE_LEVEL, double>() {
		{UPGRADE_LEVEL.BASIC, 1.0 },
		{UPGRADE_LEVEL.FIRST, 2.0},
		{UPGRADE_LEVEL.SECOND, 3.0},
		{UPGRADE_LEVEL.THIRD, 4.0},
	};
}

