
using Godot;

public interface IActionable {
	public bool IsCollisionEnabled { get; set; }
	public abstract void Action (EventData data);
	public abstract void EnableCollision (bool enable);
}

//public abstract partial class Node2D, IActionable : Node2D, IActionable {
//	[Signal] public delegate void ActionedEventHandler ();
//	public bool IsCollisionEnabled { get; set; }

//	public abstract void Action (EventData data);
//	public virtual void EnableCollision (bool enable) { return; }
//}
//public abstract partial class ActionableBody : StaticBody2D, IActionable {
//	[Signal] public delegate void ActionedEventHandler ();
//	public bool IsCollisionEnabled { get; set; }

//	public abstract void Action (EventData data);
//	public virtual void EnableCollision (bool enable) { IsCollisionEnabled = enable; }

//}

//public abstract partial class ActionableArea : Area2D, IActionable {
//	[Signal] public delegate void ActionedEventHandler ();
//	public bool IsCollisionEnabled { get; set; }

//	public abstract void Action (EventData data);

//	public virtual void EnableCollision (bool enable) { IsCollisionEnabled = enable; }
//}
