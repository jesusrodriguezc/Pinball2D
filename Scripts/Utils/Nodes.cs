using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.HttpRequest;

namespace Pinball.Scripts.Utils {
	public static class Nodes {

		public static List<T> findByClass<T> (Node node) where T : Node {
			List<T> result = new List<T>();
			if (node is T nodeT) {
				result.Add(nodeT);
			}
			foreach (var child in node.GetChildren(true)) {
				result.AddRange(findByClass<T>(child));
			}

			return result;

		}
		//public static void findByClass<T>(Node node, ref List<T> result ) where T : Node {
		//	if (node is T nodeT) {
		//		result.Add(nodeT);
		//	}
		//	foreach (var child in node.GetChildren(true)) {
		//		findByClass(child, ref result);
		//	}
			
		//}

		public static Variant SecureCall(this Node node, StringName method, params Variant[] args) {
			if (node.HasMethod(method)) {
				return node.Call(method, args);
			}
			return new Variant();
		}
	}
}
