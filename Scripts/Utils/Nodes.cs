using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.HttpRequest;

namespace Pinball.Scripts.Utils {
	public static class Nodes {

		public static List<T> findByClass<T> (Node node){
			List<T> result = new List<T>();
			if (node is T nodeT) {
				result.Add(nodeT);
			}
			foreach (var child in node.GetChildren(true)) {
				result.AddRange(findByClass<T>(child));
			}

			return result;

		}

		public static Variant SecureCall(this Node node, StringName method, params Variant[] args) {
			if (node.HasMethod(method)) {
				return node.Call(method, args);
			}
			return new Variant();
		}

		public static T GetChildByType<T> (this Node node, bool recursive = true)
		where T : Node {
			int childCount = node.GetChildCount();

			for (int i = 0; i < childCount; i++) {
				Node child = node.GetChild(i);
				if (child is T childT)
					return childT;

				if (recursive && child.GetChildCount() > 0) {
					T recursiveResult = child.GetChildByType<T>(true);
					if (recursiveResult != null)
						return recursiveResult;
				}
			}

			return null;
		}

		public static T GetParentByType<T> (this Node node)
			where T : Node {
			Node parent = node.GetParent();
			if (parent != null) {
				if (parent is T parentT) {
					return parentT;
				} else {
					return parent.GetParentByType<T>();
				}
			}

			return null;
		}
	}
}
