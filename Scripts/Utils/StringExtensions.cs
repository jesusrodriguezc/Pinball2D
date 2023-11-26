using Godot;
using System.Text.RegularExpressions;

namespace Pinball.Utils {
	public static class StringUtils {
		public static string GetTemplateName (Node node) {
			if (node == null) return null;

			string nodeName = node.Name;
			if (string.IsNullOrEmpty(nodeName)) {
				return nodeName;
			}

			string baseNodeName = Regex.Replace(nodeName, "[0-9@]", "");

			return baseNodeName;

		}
	}
}
