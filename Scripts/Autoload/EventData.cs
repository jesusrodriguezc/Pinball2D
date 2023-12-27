using Godot;
using System.Collections.Generic;

public class EventData {
	public EventData () {
	}

	public Node2D Sender { get; set; }
	public Dictionary<StringName, object> Parameters { get; set; }
}