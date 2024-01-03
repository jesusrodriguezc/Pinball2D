
using Godot;

public interface IActionable {
	public bool IsCollisionEnabled { get; set; }
	public void Action (EventData data);
	public void EnableCollision(bool enable);
}
