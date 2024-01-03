using Godot;

public interface IActor 
{
	void Pause ();
	void Resume ();
	void Teleport (Vector2 position, Vector2 velocity);
}