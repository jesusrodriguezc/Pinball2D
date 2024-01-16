using Godot;
using Pinball.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public partial class EventManager : Node {

	public List<StringName> CurrentEvents = new();
	/* ¿Cuál es mi intención aquí? La siguiente:
	 * 
	 *	Para facilitar la comunicación entre elementos, se ejecutará una función SendMessage(Node2D sender, Node2D receiver, EventType eventType, params object[])
	*/

	public enum EventType {
		TRIGGER,
		ERROR,
		ENABLE,
		DISABLE
	}

	public void SendMessage(Node2D sender, Node2D[] receivers, EventType eventType, Dictionary<StringName, object> args) {
		if (sender == null) {
			GD.PrintErr("EventManager.SendMessage() without sender.");
			return;
		}

		if (receivers == null || receivers.Length == 0) {
			GD.PrintErr("EventManager.SendMessage() without receiver.");
			return;
		}

		switch (eventType) {
			case EventType.TRIGGER:

				foreach (var receiver in receivers) {
					if (receiver is not IActionable actionable) {
						continue;
					}

					actionable.Action(new EventData() { Sender = sender, Parameters = args });
				}
				break;
			case EventType.ENABLE:
				foreach (var receiver in receivers) {
					if (receiver is not CollisionObject2D collisionObj) {
						continue;
					}

					if (!collisionObj.HasMethod("EnableCollision")) {
						continue;
					}

					Nodes.SecureCall(collisionObj, "EnableCollision", true);
				}
				break;
			case EventType.DISABLE:
				foreach (var receiver in receivers) {
					if (receiver is not CollisionObject2D collisionObj) {
						continue;
					}

					if (!collisionObj.HasMethod("EnableCollision")) {
						continue;
					}

					Nodes.SecureCall(collisionObj, "EnableCollision", false);
				}
				break;
			case EventType.ERROR:
			default:
				GD.PrintErr($"{sender.Name} send a message of type {eventType}. Event type not provided.");
				break;


		}



	}
}

