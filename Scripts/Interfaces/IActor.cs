using Godot;
using System.Collections.Generic;

public interface IActor 
{
	List<TriggerZone> CurrentTriggerZones { get; set; }

	void Pause ();
	void Unpause ();
	void Teleport (Vector2 position, Vector2 velocity);
}