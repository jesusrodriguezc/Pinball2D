using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinball.Utils {
	public static class Mathx {
		public static float FuncSmooth(float x) {
			return (2 * x) / (x + 1);
		}

		public static float SlingshotDistribution(float x) {
			return -1 * (float)Math.Pow(x - 1, 4) + 1;
		}
	}
}
