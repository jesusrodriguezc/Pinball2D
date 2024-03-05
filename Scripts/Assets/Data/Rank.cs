using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum RankId {
	NONE, LEVEL1, LEVEL2, LEVEL3, LEVEL4, LEVEL5, LEVEL6, LEVEL7, LEVEL8, LEVEL9, LEVEL10
}

public static class Rank {

	public static Dictionary<RankId, double> maxXpPerRank = new Dictionary<RankId, double>() { 
		{ RankId.LEVEL1, 100 },
		{ RankId.LEVEL2, 500 },
		{ RankId.LEVEL3, 1000 },
		{ RankId.LEVEL4, 2000 },
		{ RankId.LEVEL5, 5000 },
		{ RankId.LEVEL6, 8000 },
		{ RankId.LEVEL7, 10000 },
		{ RankId.LEVEL8, 15000 },
		{ RankId.LEVEL9, 25000 },
		{ RankId.LEVEL10, 99999999 }
	};
}

