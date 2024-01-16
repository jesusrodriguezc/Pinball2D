using Godot;
using Pinball.Scripts.Utils;
using System;
using System.Linq;
using System.Reflection.Emit;
using static PinballController;

public partial class Ball : RigidBody2D, IActor{
	public enum Status {
		GAMEOVER = -2,
		DEAD = -1,
		IDLE = 0,
		MOVING = 1
	}

	[Signal]
	public delegate void DeathEventHandler (Ball ball);

	private EventManager eventManager;
	[Export]
	public int DebugPower { get; set; } = 3000;
	private bool DebugTeleport = false;

	[Export]
	public float MaxVelocity { get; set; }


	public Status currentStatus;
	[Export] private LayerId currentLayer;

	private Vector2 initialPosition;
	private AudioComponent _audioComponent;
	private readonly StringName DEAD = "Dead";

	internal Vector2 storedVelocity;
	private Vector2 storedPosition;

	public override void _Ready () {


		eventManager = GetNodeOrNull<EventManager>("/root/EventManager");

		InitSignalConnections();

		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		_audioComponent?.AddAudio(DEAD, ResourceLoader.Load<AudioStream>("res://SFX/deathzone_enter.wav"));
		
		initialPosition = GlobalPosition;
		currentStatus = Status.IDLE;

		SetLevel(currentLayer);

	}

	private void InitSignalConnections () {
		var rebounds = Nodes.findByClass<ReboundBase>(GetTree().Root);
		foreach (var rebound in rebounds) {
			rebound.Impulse += (nodeAffected, impulse) => { if (nodeAffected == this) ApplyImpulse(impulse); };
		}
	}

	public override void _Input (InputEvent @event) {
		if (currentStatus.IsNotIn(Status.IDLE, Status.MOVING)) {
			return;
		}

		if (@event is InputEventMouseButton mouseButton) {
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.IsPressed()) {
				Move();
			}
		}

		if (@event is InputEventKey key) {
			if (key.Keycode.Equals(Key.Enter) && key.IsPressed()) {
				Instance.Shake();
			}		
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess (double delta) {
		if (currentStatus == Status.DEAD && Instance.LivesLeft > 0) {
			Reset();
			currentStatus = Status.IDLE;
		}

		if (!GetCollisionMaskValue(5) && LinearVelocity.LengthSquared() > (MaxVelocity * MaxVelocity)) {
			LinearVelocity = LinearVelocity.LimitLength(MaxVelocity);
		}

		//if (LinearVelocity.LengthSquared() < 0.1f) {
		//	if (currentStatus == Status.MOVING) {
		//		currentStatus = Status.IDLE;
		//	}
		//} else {
		//	if (currentStatus == Status.IDLE) {
		//		currentStatus = Status.MOVING;
		//	}
		//}
	}

	private void ApplyImpulse (Vector2 impulse) {
		LinearVelocity = new Vector2(0f, 0f);
		ApplyCentralImpulse(impulse);
	}

	public void Move () {
		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 forceDirection = (mousePosition - GlobalPosition).Normalized();

		ApplyCentralImpulse(forceDirection * DebugPower); //, Position - (Vector2) result["position"]);

	}

	public void Die () {

		_audioComponent?.Play(DEAD, AudioComponent.SFX_BUS);

		currentStatus = Status.DEAD;

		EmitSignal(SignalName.Death, this);


	}

	private void Reset () {
		Teleport(initialPosition, Vector2.Zero);
	}

	public void Teleport(Vector2 position, Vector2 velocity) {
		LinearVelocity = velocity;
		GlobalPosition = position;
	}

	public void SetLevel (LayerId layerId) {

		int level = LayerManager.LayerId2Int(layerId);
		currentLayer = layerId;
		if (level == -1) { return; }

		CollisionMask &= 0b0011;

		SetCollisionMaskValue(level, true);

		ZIndex = LayerManager.GetZLevel(level);

		eventManager.SendMessage(this, LayerManager.GetActionablesFromLayer(layerId, GetTree().Root), EventManager.EventType.DISABLE, null);

	}

	#region IActor definitions
	public void Pause () {
		storedVelocity = LinearVelocity;
		storedPosition = GlobalPosition;

	}

	public void Resume () {
		LinearVelocity = storedVelocity;
		GlobalPosition = storedPosition;
	}
	#endregion
}
