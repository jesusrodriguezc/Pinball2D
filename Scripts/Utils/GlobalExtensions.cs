using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinball.Scripts.Utils {
	public static class GlobalExtensions {

		public static bool IsIn<T>(this T obj, params T[] values) {
			if (obj == null) return false;
			if (values == null) return false;

			foreach (var value in values) {
				if (obj.Equals(value))
					return true;
			}

			return false;
		}

		public static bool IsNotIn<T> (this T obj, params T[] values) {
			if (obj == null) return true;
			if (values == null) return true;

			foreach (var value in values) {
				if (obj.Equals(value))
					return false;
			}

			return true;
		}
	}
}
