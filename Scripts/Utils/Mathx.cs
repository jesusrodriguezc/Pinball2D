using Godot;
using System;

namespace Pinball.Utils {
	public static class Mathx {
		public static float FuncSmooth (float x) {
			return (2 * x) / (x + 1);
		}

		public static float SlingshotDistribution (float x) {
			return -1 * (float)Math.Pow(x - 1, 4) + 1;
		}

		public static bool IsAlmostEqualUnsigned (float x, float y, float maxDiff = 0.1f) {
			return Mathf.Abs(x - y) <= Mathf.Abs(maxDiff);
		}

		public static double Linear2Db(double value) {
			return 20d * Math.Log(value);
		}
		public static double Db2Linear (double value) {
			return Math.Pow(value, Math.E) / 20d;
		}

	}
}
